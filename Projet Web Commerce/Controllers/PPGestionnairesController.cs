using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;
using Projet_Web_Commerce.Models;

namespace Projet_Web_Commerce.Controllers
{
    public class PPGestionnairesController : Controller
    {
        public readonly AuthDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<Utilisateur> _userManager;

        public PPGestionnairesController(AuthDbContext context, Microsoft.AspNetCore.Identity.UserManager<Utilisateur> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Gestionnaire")]
        public ActionResult ListeVendeurs()
        {
            //var groupedVendeurs = _context.PPVendeurs
            //    .Where(v => v.Statut == 1)
            //    .GroupBy(v => new List<int>{ v.DateCreation.Month, v.DateCreation.Year })
            //    .SelectMany(group => group.OrderBy(v => v.NomAffaires))
            //    .ToDictionary(
            //        group => new List<int>{ group.DateCreation.Month, group.DateCreation.Year });

            var vendeurs = _context.PPVendeurs
                .Where(v => v.Statut == 1)
                .OrderByDescending(v => v.NomAffaires)
                .ToList();

            //var flattenedList = groupedVendeurs
            //    .SelectMany(group => group.OrderBy(v => v.NomAffaires))
            //    .ToList();

            var lstMoisAnneesDistincts = _context.PPVendeurs
                .Where(v => v.Statut == 1)
                .Select(v => new ModelMoisAnnees { Mois = v.DateCreation.Month, Annee = v.DateCreation.Year })
                //.Select(v => new { Mois = v.DateCreation.Month, Annee = v.DateCreation.Year })
                .Distinct()
                .OrderByDescending(item => item.Annee)
                .ThenByDescending(item => item.Mois)
                .ToList();

            var ProduitsList = _context.PPProduits.ToList();

            var nbProduits = _context.PPProduits
                .GroupBy(p => p.NoVendeur)
                .Count();

            ModelListeVendeurs modelListeVendeurs = new ModelListeVendeurs()
            {
                VendeursList = vendeurs,
                ProduitsList = ProduitsList,
                MoisAnneesDistinctsList = lstMoisAnneesDistincts
            };

            return View(modelListeVendeurs);
        }

        [HttpGet]
        [Authorize(Roles = "Gestionnaire")]
        public ActionResult ListeVendeursAConfirmer()
        {
            var vendeursStatutZero = _context.PPVendeurs
                .Where(v => v.Statut == 0)
                .OrderBy(v => v.DateCreation)
                .ToList();

            return View(vendeursStatutZero);
        }

        [Route("/PPGestionnairesController/ValiderAsync")]
        [Authorize(Roles = "Gestionnaire")]
        public async Task<IActionResult> ValiderAsync(int id, string sujet, string message, bool vendeurAccepte, int pourcentage)
        {
            var vendeurAUpdate = _context.PPVendeurs.FirstOrDefault(v => v.NoVendeur == id);

            var result = new
            {
                Success = false,
                Message = $"Erreur%"
            };

            if (vendeurAUpdate != null)
            {
                var user = await _userManager.FindByEmailAsync(vendeurAUpdate.AdresseEmail);
                if (user != null)
                {
                    await Methodes.envoyerCourriel(vendeurAUpdate.AdresseEmail, sujet, message);
                    if (vendeurAccepte)
                    {
                        vendeurAUpdate.Statut = 1;
                        vendeurAUpdate.Pourcentage = Convert.ToDecimal(pourcentage, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        _context.PPVendeurs.Remove(vendeurAUpdate);
                        await _userManager.DeleteAsync(user);
                    }
                    _context.SaveChanges();

                    var vendeursStatutZero = _context.PPVendeurs
                            .Where(v => v.Statut == 0)
                            .OrderBy(v => v.DateCreation)
                            .ToList();

                    View(vendeursStatutZero);

                    result = new
                    {
                        Success = true,
                        Message = $"Courriel envoyé%."
                    };
                }
            }

            return Json(result);
        }

        [HttpGet]
        [Authorize(Roles = "Gestionnaire")]
        public ActionResult ListeVendeursInactifs()
        {
            var vendeursStatutZero = _context.PPVendeurs
                .Where(v => v.Statut == 0)
                .OrderBy(v => v.DateCreation)
                .ToList();

            return View();
        }


        // GET: PPGestionnaires
        public async Task<IActionResult> Index()
        {
            var authDbContext = _context.PPGestionnaire.Include(p => p.Utilisateur);
            return View(await authDbContext.ToListAsync());
        }

        // GET: PPGestionnaires/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pPGestionnaire = await _context.PPGestionnaire
                .Include(p => p.Utilisateur)
                .FirstOrDefaultAsync(m => m.NoGestionnaire == id);
            if (pPGestionnaire == null)
            {
                return NotFound();
            }

            return View(pPGestionnaire);
        }

        // GET: PPGestionnaires/Create
        public IActionResult Create()
        {
            ViewData["IdUtilisateur"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: PPGestionnaires/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NoGestionnaire,DateCreation,AdresseEmail,MotDePasse,Nom,Prenom,IdUtilisateur")] PPGestionnaire pPGestionnaire)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pPGestionnaire);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdUtilisateur"] = new SelectList(_context.Users, "Id", "Id", pPGestionnaire.IdUtilisateur);
            return View(pPGestionnaire);
        }

        // GET: PPGestionnaires/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pPGestionnaire = await _context.PPGestionnaire.FindAsync(id);
            if (pPGestionnaire == null)
            {
                return NotFound();
            }
            ViewData["IdUtilisateur"] = new SelectList(_context.Users, "Id", "Id", pPGestionnaire.IdUtilisateur);
            return View(pPGestionnaire);
        }

        // POST: PPGestionnaires/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NoGestionnaire,DateCreation,AdresseEmail,MotDePasse,Nom,Prenom,IdUtilisateur")] PPGestionnaire pPGestionnaire)
        {
            if (id != pPGestionnaire.NoGestionnaire)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pPGestionnaire);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PPGestionnaireExists(pPGestionnaire.NoGestionnaire))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdUtilisateur"] = new SelectList(_context.Users, "Id", "Id", pPGestionnaire.IdUtilisateur);
            return View(pPGestionnaire);
        }

        // GET: PPGestionnaires/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pPGestionnaire = await _context.PPGestionnaire
                .Include(p => p.Utilisateur)
                .FirstOrDefaultAsync(m => m.NoGestionnaire == id);
            if (pPGestionnaire == null)
            {
                return NotFound();
            }

            return View(pPGestionnaire);
        }

        // POST: PPGestionnaires/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pPGestionnaire = await _context.PPGestionnaire.FindAsync(id);
            if (pPGestionnaire != null)
            {
                _context.PPGestionnaire.Remove(pPGestionnaire);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PPGestionnaireExists(int id)
        {
            return _context.PPGestionnaire.Any(e => e.NoGestionnaire == id);
        }
    }
}
