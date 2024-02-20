using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;
using Projet_Web_Commerce.Models;

namespace Projet_Web_Commerce.Controllers
{
    [Authorize(Roles = "Gestionnaire")]
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
        public ActionResult Statistiques()
        {
            var vendeurs = _context.PPVendeurs
                .Where(v => v.Statut == 1)
                .ToList();

            var vendeurdate = _context.PPVendeurs
                .Select(v => new ModelMoisAnneVendeur
                {
                    Mois = v.DateCreation.Month,
                    Annee = v.DateCreation.Year
                })
                .ToList();

            var visitesCountData = _context.PPVendeursClients
                .Include(p => p.PPClients)
                .Include(p => p.PPVendeurs)
                .GroupBy(p => new { p.NoClient, p.NoVendeur })
                .Select(g => new ModelVisite
                {
                    ClientName = (string.IsNullOrEmpty(g.FirstOrDefault().PPClients.Prenom) && string.IsNullOrEmpty(g.FirstOrDefault().PPClients.Nom))
                        ? g.FirstOrDefault().PPClients.AdresseEmail
                        : (g.FirstOrDefault().PPClients.Prenom + " " + g.FirstOrDefault().PPClients.Nom).Trim(),
                    VendeurName = (string.IsNullOrEmpty(g.FirstOrDefault().PPVendeurs.Prenom) && string.IsNullOrEmpty(g.FirstOrDefault().PPVendeurs.Nom))
                        ? g.FirstOrDefault().PPVendeurs.AdresseEmail
                        : (g.FirstOrDefault().PPVendeurs.Prenom + " " + g.FirstOrDefault().PPVendeurs.Nom).Trim(),
                    VisitCount = g.Count()
                })
                .ToList();

            var clients = _context.PPClients.ToList();

            var ProduitsList = _context.PPProduits.ToList();

            var CommandesList = _context.PPCommandes
                .GroupBy(c => c.NoVendeur)
                .Select(group => group.OrderByDescending(c => c.DateCommande).FirstOrDefault())
                .ToList();

            var orderCountsByVendeur = _context.PPCommandes
                .Where(c => c.PPDetailsCommandes.Any())
                .GroupBy(o => o.NoVendeur)
                .Select(g => new
                {
                    VendeurId = g.Key,
                    OrderCount = g.SelectMany(c => c.PPDetailsCommandes).Sum(d => d.Quantité)
                })
                .ToList();

            var totalOrders = orderCountsByVendeur.Sum(o => o.OrderCount);

            var orderPercentagesByVendeur = orderCountsByVendeur
                .Select(o => new OrderPercentage
                {
                    VendeurId = o.VendeurId,
                    VendeurName = _context.PPVendeurs.FirstOrDefault(v => v.NoVendeur == o.VendeurId)?.Prenom + " " + _context.PPVendeurs.FirstOrDefault(v => v.NoVendeur == o.VendeurId)?.Nom,
                    Percentage = Math.Round((decimal)o.OrderCount / totalOrders * 100, 2)
                })
                .ToList();

            var currentDate = DateTime.Now;
            var startDate = currentDate.AddMonths(-12);
            var monthlySales = _context.PPCommandes
                .Where(c => c.DateCommande >= startDate && c.DateCommande <= currentDate)
                .GroupBy(c => new { Year = c.DateCommande.Year, Month = c.DateCommande.Month })
                .Select(g => new { Year = g.Key.Year, Month = g.Key.Month, TotalMoney = g.Sum(c => c.MontantTotAvantTaxes) })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToList();

            var labels = monthlySales.Select(s => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(s.Month)} {s.Year}").ToList();
            var data = monthlySales.Select(s => s.TotalMoney).ToList();

            var ordersWithDetails = _context.PPCommandes
            .Include(o => o.PPDetailsCommandes)
            .Include(o => o.PPClients)
            .Include(o => o.PPVendeurs)
            .OrderByDescending(o => o.DateCommande)
            .ToList();

            var ordersByClientAndVendeur = ordersWithDetails
                .GroupBy(o => new { o.NoClient, o.NoVendeur })
                .Select(g => new ModelordersByClientAndVendeur
                {
                    NoClient = g.Key.NoClient,
                    NomPrenomClient = (string.IsNullOrEmpty(g.FirstOrDefault().PPClients.Nom) && string.IsNullOrEmpty(g.FirstOrDefault().PPClients.Prenom))
                        ? g.FirstOrDefault().PPClients.AdresseEmail
                        : (g.FirstOrDefault().PPClients.Nom + " " + g.FirstOrDefault().PPClients.Prenom).Trim(),
                    TotalCommandeAT = g.Sum(o => o.MontantTotAvantTaxes),
                    CoutLivraison = g.Sum(o => o.CoutLivraison),
                    DateDerniereCommande = g.Max(o => o.DateCommande),
                    NoVendeur = g.Key.NoVendeur,
                    NomPrenomVendeur = (g.FirstOrDefault().PPVendeurs.Nom + " " + g.FirstOrDefault().PPVendeurs.Prenom).Trim(),
                    ProvinceVendeur = g.FirstOrDefault().PPVendeurs.NoProvince,
                    PourcentageTaxeVendeur = g.FirstOrDefault().PPVendeurs.PourcentageTaxe,
                    TaxesVendeur = g.FirstOrDefault().PPVendeurs.Taxes
                })
                .OrderByDescending(o => o.DateDerniereCommande)
                .ToList();


            var modelListeStat = new ModelListeStat()
            {
                ClientsList = clients,
                VendeursList = vendeurs,
                ProduitsList = ProduitsList,
                CommandesList = CommandesList,
                VendeurDate = vendeurdate,
                OrderPercentages = orderPercentagesByVendeur,
                VisitesCountData = visitesCountData,
                Labels = labels,
                Data = data,
                OrdersByClientAndVendeurList = ordersByClientAndVendeur
            };

            return View(modelListeStat);
        }


        [HttpGet]
        [Authorize(Roles = "Gestionnaire")]
        public ActionResult ListeVendeurs()
        {
            var vendeurs = _context.PPVendeurs
                .Where(v => v.Statut == 1)
                .OrderByDescending(v => v.NomAffaires)
                .ToList();

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

            var CommandesList = _context.PPCommandes
             .GroupBy(c => c.NoVendeur)
             .Select(group => group.OrderByDescending(c => c.DateCommande).FirstOrDefault())
             .ToList();

            var utilisateurList = _context.Users.ToList();

            ModelListeVendeurs modelListeVendeurs = new ModelListeVendeurs()
            {
                VendeursList = vendeurs,
                ProduitsList = ProduitsList,
                MoisAnneesDistinctsList = lstMoisAnneesDistincts,
                CommandesList = CommandesList,
                UtilisateurList = utilisateurList

            };

            return View(modelListeVendeurs);
        }

        [HttpGet]
        [Authorize(Roles = "Gestionnaire")]
        public ActionResult ListeClients()
        {

            var vendeurs = _context.PPVendeurs
                .Where(v => v.Statut == 1)
                .OrderByDescending(v => v.NomAffaires)
                .ToList();

            var lstMoisAnneesDistincts = _context.PPClients
                .Where(v => v.Statut == 1)
                .Select(v => new ModelMoisAnnees { Mois = v.DateCreation.Month, Annee = v.DateCreation.Year })
                .Distinct()
                .OrderByDescending(item => item.Annee)
                .ThenByDescending(item => item.Mois)
                .ToList();

            var ProduitsList = _context.PPProduits.ToList();
            var CommandesList = _context.PPCommandes.Where(v => v.Statut != "E").ToList();

            var VendeursClientsList = _context.PPVendeursClients
            .GroupBy(vc => vc.NoClient)
            .Select(group => group.OrderByDescending(vc => vc.DateVisite).FirstOrDefault())
            .ToList();

            var ClientsList = _context.PPClients.Where(v => v.Statut == 1).ToList();

            var ClientsList2 = _context.PPClients.Where(v => v.Statut == 1).Select(c => c.NoClient).ToList();

            var clientPanierList = _context.PPArticlesEnPanier.Where(v => ClientsList2.Contains(v.NoClient)).ToList();

            var nbProduits = _context.PPProduits
                .GroupBy(p => p.NoVendeur)
                .Count();

            ModelListeClients modelListeClients = new ModelListeClients()
            {
                VendeursList = vendeurs,
                ProduitsList = ProduitsList,
                MoisAnneesDistinctsList = lstMoisAnneesDistincts,
                ClientsList = ClientsList,
                CommandesList = CommandesList,
                VendeursClientsList = VendeursClientsList,
                ClientPanierList = clientPanierList
            };

            return View(modelListeClients);
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
        public async Task<IActionResult> ValiderAsync(int id, string sujet, string message, string infosSupp, bool vendeurAccepte, decimal pourcentageRedevence)
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
                    if (infosSupp != null)
                        message += "\n\n" + infosSupp;
                    await Methodes.envoyerCourriel(sujet, message, vendeurAUpdate.AdresseEmail);
                    if (vendeurAccepte)
                    {
                        vendeurAUpdate.Statut = 1;
                        vendeurAUpdate.Pourcentage = pourcentageRedevence;
                        _context.Update(vendeurAUpdate);
                        await _context.SaveChangesAsync();
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
        public ActionResult ListeClientsInactifs()
        {
            var vendeurs = _context.PPVendeurs
              .Where(v => v.Statut == 1)
              .OrderByDescending(v => v.NomAffaires)
              .ToList();

            var lstMoisAnneesDistincts = _context.PPClients
                .Where(v => v.Statut == 2)
                .Select(v => new ModelMoisAnnees { Mois = v.DateCreation.Month, Annee = v.DateCreation.Year })
                .Distinct()
                .OrderByDescending(item => item.Annee)
                .ThenByDescending(item => item.Mois)
                .ToList();

            var ProduitsList = _context.PPProduits.ToList();

            var CommandesList = _context.PPCommandes.ToList();

            var VendeursClientsList = _context.PPVendeursClients
            .GroupBy(vc => vc.NoClient)
            .Select(group => group.OrderByDescending(vc => vc.DateVisite).FirstOrDefault())
            .ToList();


            var ClientsList = _context.PPClients.Where(v => v.Statut == 2).ToList();

            var nbProduits = _context.PPProduits
                .GroupBy(p => p.NoVendeur)
                .Count();

            ModelListeClients modelListeClients = new ModelListeClients()
            {
                VendeursList = vendeurs,
                ProduitsList = ProduitsList,
                MoisAnneesDistinctsList = lstMoisAnneesDistincts,
                ClientsList = ClientsList,
                CommandesList = CommandesList,
                VendeursClientsList = VendeursClientsList
            };

            return View(modelListeClients);
        }

        [HttpGet]
        [Authorize(Roles = "Gestionnaire")]
        public ActionResult ListeVendeursInactifs()
        {
            var vendeurs = _context.PPVendeurs
                .Where(v => v.Statut == 2)
                .OrderByDescending(v => v.NomAffaires)
                .ToList();

            var lstMoisAnneesDistincts = _context.PPVendeurs
                .Where(v => v.Statut == 2)
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

            var CommandesList = _context.PPCommandes
             .GroupBy(c => c.NoVendeur)
             .Select(group => group.OrderByDescending(c => c.DateCommande).FirstOrDefault())
             .ToList();

            var utilisateurList = _context.Users.ToList();

            ModelListeVendeurs modelListeVendeurs = new ModelListeVendeurs()
            {
                VendeursList = vendeurs,
                ProduitsList = ProduitsList,
                MoisAnneesDistinctsList = lstMoisAnneesDistincts,
                CommandesList = CommandesList,
                UtilisateurList = utilisateurList

            };

            return View(modelListeVendeurs);
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


        public async Task<IActionResult> FraudeC(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.PPClients
                .Include(p => p.Utilisateur)
                .FirstOrDefaultAsync(m => m.NoClient == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost, ActionName("FraudeC")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FraudeC(int id)
        {
            var item = await _context.PPClients.FindAsync(id);
            if (item != null)
            {
                item.Statut = 0;
                var email = item.AdresseEmail;
                var idUSER = item.IdUtilisateur;
                var use = await _context.Users.FindAsync(idUSER);
                use.EmailConfirmed = false;
                Console.WriteLine("USER : ================================================== " + use.Email);
                _context.Update(item);
                _context.Update(use);

                string returnUrl = Url.Content("~/");
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(use);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                       "/Account/ConfirmEmail",
                        pageHandler: null,
                       values: new { area = "Identity", userId = use.Id, code = code, returnUrl = returnUrl },
                       protocol: Request.Scheme);
                await Methodes.envoyerCourriel(
                       "Vous devez reconfirmer votre adresse courriel",
                       $"Veuillez reconfirmer votre compte en <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>cliquant ici</a>. Votre numéro de client est {item.NoClient}",
                       email);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("ListeClients");
        }

        public async Task<IActionResult> FraudeV(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.PPVendeurs
                .Include(p => p.Utilisateur)
                .FirstOrDefaultAsync(m => m.NoVendeur == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost, ActionName("FraudeV")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FraudeV(int id)
        {
            var item = await _context.PPVendeurs.FindAsync(id);
            if (item != null)
            {
                // item.Statut = 0;
                var email = item.AdresseEmail;
                var idUSER = item.IdUtilisateur;
                var use = await _context.Users.FindAsync(idUSER);
                use.EmailConfirmed = false;
                Console.WriteLine("USER : ================================================== " + use.Email);
                // _context.Update(item);
                _context.Update(use);

                string returnUrl = Url.Content("~/");
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(use);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                       "/Account/ConfirmEmail",
                        pageHandler: null,
                       values: new { area = "Identity", userId = use.Id, code = code, returnUrl = returnUrl },
                       protocol: Request.Scheme);
                await Methodes.envoyerCourriel(
                       "Vous devez reconfirmer votre adresse courriel",
                       $"Veuillez reconfirmer votre compte en <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>cliquant ici</a>. Votre numéro de vendeur est {item.NoVendeur}",
                       email);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("ListeVendeurs");
        }

        private bool PPGestionnaireExists(int id)
        {
            return _context.PPGestionnaire.Any(e => e.NoGestionnaire == id);
        }

        public async Task<IActionResult> Taux(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.PPVendeurs
                .Include(p => p.Utilisateur)
                .FirstOrDefaultAsync(m => m.NoVendeur == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Taux([Bind("Pourcentage")] PPVendeurs pPVendeurs, int id)
        {
            var keysToRemove = ModelState.Keys.Where(k => k != "Pourcentage").ToList();
            foreach (var key in keysToRemove)
            {
                ModelState.Remove(key);
            }
            foreach (var m in ModelState)
            {
                foreach (var er in m.Value.Errors)
                {
                    Console.WriteLine(m.Key);
                    Console.WriteLine(er.ErrorMessage);
                }
            }
            Console.WriteLine(pPVendeurs.Pourcentage);
            var item = await _context.PPVendeurs.FindAsync(id);
            if (ModelState.IsValid && item != null)
            {
                try
                {
                    item.Pourcentage = pPVendeurs.Pourcentage;
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PPVendeurExists(pPVendeurs.NoVendeur))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ListeVendeurs");
            }

            return View(pPVendeurs);
        }

        private bool PPVendeurExists(int id)
        {
            return _context.PPVendeurs.Any(e => e.NoVendeur == id);
        }

        public async Task<IActionResult> InactifV(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.PPVendeurs
                .Include(p => p.Utilisateur)
                .FirstOrDefaultAsync(m => m.NoVendeur == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        public async Task<IActionResult> InactifC(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.PPClients
                .Include(p => p.Utilisateur)
                .FirstOrDefaultAsync(m => m.NoClient == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost, ActionName("InactifV")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InactifV(int id)
        {
            var item = await _context.PPVendeurs.FindAsync(id);
            if (item != null)
            {
                item.Statut = 2;
                _context.Update(item);

            }

            await _context.SaveChangesAsync();
            return RedirectToAction("ListeVendeurs");
        }

        [HttpPost, ActionName("InactifC")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InactifC(int id)
        {
            var item = await _context.PPClients.FindAsync(id);
            if (item != null)
            {
                item.Statut = 2;
                _context.Update(item);

            }

            await _context.SaveChangesAsync();
            return RedirectToAction("ListeClients");
        }

        [HttpPost]
        public async Task<IActionResult> SupprimerClient()
        {
            var clientsToDelete = Request.Form["clientsToDelete"].ToString().Split(',').Select(int.Parse).ToList();
            if (clientsToDelete == null || !clientsToDelete.Any())
            {
                return BadRequest(new { error = "Aucun client à supprimer." });
            }

            try
            {
                var vendeursClients = _context.PPVendeursClients.Where(vc => clientsToDelete.Contains(vc.NoClient));
                _context.PPVendeursClients.RemoveRange(vendeursClients);

                var articlesEnPanier = _context.PPArticlesEnPanier.Where(ap => clientsToDelete.Contains(ap.NoClient));
                _context.PPArticlesEnPanier.RemoveRange(articlesEnPanier);

                var evaluation = _context.PPEvaluations.Where(ap => clientsToDelete.Contains(ap.NoClient));
                _context.PPEvaluations.RemoveRange(evaluation);

                var commandes = _context.PPCommandes.Where(cmd => clientsToDelete.Contains(cmd.NoClient));
                foreach (var commande in commandes)
                {
                    var detailsCommandes = _context.PPDetailsCommandes.Where(dc => dc.NoCommande == commande.NoCommande);
                    _context.PPDetailsCommandes.RemoveRange(detailsCommandes);
                }
                _context.PPCommandes.RemoveRange(commandes);

                var emailDelete = _context.PPClients
                    .Where(c => clientsToDelete.Contains(c.NoClient))
                    .Select(c => c.IdUtilisateur);

                var email = _context.PPDestinatairesMessage
                    .Where(u => emailDelete.Contains(u.Destinataire));

                var messageIds = _context.PPMessages
                    .Where(u => emailDelete.Contains(u.Auteur))
                    .Select(u => u.NoMessage)
                    .ToList();

                var messagesToDelete = _context.PPMessages
                    .Where(m => messageIds.Contains(m.NoMessage));

                var destinatairesToDelete = _context.PPDestinatairesMessage
                    .Where(u => messageIds.Contains(u.NoMessage));

                _context.PPDestinatairesMessage.RemoveRange(destinatairesToDelete);
                _context.PPMessages.RemoveRange(messagesToDelete);
                _context.PPDestinatairesMessage.RemoveRange(email);



                var usersToDelete = _context.PPClients.Where(c => clientsToDelete.Contains(c.NoClient)).Select(c => c.AdresseEmail);
                var users = _context.Users.Where(u => usersToDelete.Contains(u.Email));
                _context.Users.RemoveRange(users);

                var clients = _context.PPClients.Where(c => clientsToDelete.Contains(c.NoClient));
                _context.PPClients.RemoveRange(clients);

                await _context.SaveChangesAsync();
                return Ok(new { message = "Les clients et les informations associées ont été supprimés avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Une erreur s'est produite lors de la suppression de clients et des informations associées : {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SupprimerPanier()
        {
            var panierToDelete = Request.Form["panierToDelete"].ToString().Split(',').Select(int.Parse).ToList();
            if (panierToDelete == null || !panierToDelete.Any())
            {
                return BadRequest(new { error = "Aucun panier à supprimer." });
            }

            try
            {
                var articlesEnPanier = _context.PPArticlesEnPanier.Where(ap => panierToDelete.Contains(ap.NoClient));
                _context.PPArticlesEnPanier.RemoveRange(articlesEnPanier);

                await _context.SaveChangesAsync();
                return Ok(new { message = "Les paniers ont été supprimés avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Une erreur s'est produite lors des paniers : {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SupprimerVPanier()
        {
            var panierToDelete = Request.Form["panierToDelete"].ToString().Split(',').Select(int.Parse).ToList();
            if (panierToDelete == null || !panierToDelete.Any())
            {
                return BadRequest(new { error = "Aucun panier à supprimer." });
            }

            try
            {
                var articlesEnPanier = _context.PPArticlesEnPanier.Where(ap => panierToDelete.Contains(ap.NoVendeur));
                _context.PPArticlesEnPanier.RemoveRange(articlesEnPanier);



                await _context.SaveChangesAsync();
                return Ok(new { message = "Les paniers ont été supprimés avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Une erreur s'est produite lors des paniers : {ex.Message}" });
            }
        }


        [HttpPost]
        public async Task<IActionResult> SupprimerVendeur()
        {
            var vendeursToDelete = Request.Form["vendeursToDelete"].ToString().Split(',').Select(int.Parse).ToList();
            if (vendeursToDelete == null || !vendeursToDelete.Any())
            {
                return BadRequest(new { error = "Aucun vendeur à supprimer." });
            }

            try
            {
                var vendeursClients = _context.PPVendeursClients.Where(vc => vendeursToDelete.Contains(vc.NoVendeur));
                _context.PPVendeursClients.RemoveRange(vendeursClients);

                var articlesEnPanier = _context.PPArticlesEnPanier.Where(ap => vendeursToDelete.Contains(ap.NoVendeur));
                _context.PPArticlesEnPanier.RemoveRange(articlesEnPanier);

                var commandes = _context.PPCommandes.Where(cmd => vendeursToDelete.Contains(cmd.NoVendeur));
                foreach (var commande in commandes)
                {
                    var detailsCommandes = _context.PPDetailsCommandes.Where(dc => dc.NoCommande == commande.NoCommande);
                    _context.PPDetailsCommandes.RemoveRange(detailsCommandes);
                }
                _context.PPCommandes.RemoveRange(commandes);

                var emailDelete = _context.PPVendeurs
                    .Where(c => vendeursToDelete.Contains(c.NoVendeur))
                    .Select(c => c.IdUtilisateur);

                var email = _context.PPDestinatairesMessage
                    .Where(u => emailDelete.Contains(u.Destinataire));

                var messageIds = _context.PPMessages
                    .Where(u => emailDelete.Contains(u.Auteur))
                    .Select(u => u.NoMessage)
                    .ToList();

                var messagesToDelete = _context.PPMessages
                    .Where(m => messageIds.Contains(m.NoMessage));

                var destinatairesToDelete = _context.PPDestinatairesMessage
                    .Where(u => messageIds.Contains(u.NoMessage));

                _context.PPDestinatairesMessage.RemoveRange(destinatairesToDelete);
                _context.PPMessages.RemoveRange(messagesToDelete);
                _context.PPDestinatairesMessage.RemoveRange(email);



                var usersToDelete = _context.PPVendeurs.Where(c => vendeursToDelete.Contains(c.NoVendeur)).Select(c => c.AdresseEmail);
                var users = _context.Users.Where(u => usersToDelete.Contains(u.Email));
                _context.Users.RemoveRange(users);

                var vendeur = _context.PPVendeurs.Where(c => vendeursToDelete.Contains(c.NoVendeur));
                _context.PPVendeurs.RemoveRange(vendeur);

                await _context.SaveChangesAsync();
                return Ok(new { message = "Les vendeur ont été supprimés avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Une erreur s'est produite lors des vendeurs : {ex.Message}" });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Gestionnaire")]
        public ActionResult ListeRedevances()
        {
            var vendeurs = _context.PPVendeurs
                .Where(v => v.Statut != 0)
                .ToList();

            var lstMoisAnneesDistincts = _context.PPCommandes
                .Where(v => v.Statut != "E")
                .Select(v => new ModelMoisAnnees { Mois = v.DateCommande.Month, Annee = v.DateCommande.Year })
                //.Select(v => new { Mois = v.DateCreation.Month, Annee = v.DateCreation.Year })
                .Distinct()
                .OrderByDescending(item => item.Annee)
                .ThenByDescending(item => item.Mois)
                .ToList();

            var ProduitsList = _context.PPProduits.ToList();

            var nbProduits = _context.PPProduits
                .GroupBy(p => p.NoVendeur)
                .Count();

            var CommandesList = _context.PPCommandes
             .GroupBy(c => c.NoVendeur)
             .Select(group => group.OrderByDescending(c => c.DateCommande).FirstOrDefault())
             .ToList();

            var utilisateurList = _context.Users.ToList();

            ModelListeVendeurs modelListeVendeurs = new ModelListeVendeurs()
            {
                VendeursList = vendeurs,
                ProduitsList = ProduitsList,
                MoisAnneesDistinctsList = lstMoisAnneesDistincts,
                CommandesList = CommandesList,
                UtilisateurList = utilisateurList

            };

            return View(modelListeVendeurs);
        }

    }
}
