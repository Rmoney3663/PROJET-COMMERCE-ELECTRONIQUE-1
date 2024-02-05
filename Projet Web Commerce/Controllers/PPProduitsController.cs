using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;
using Projet_Web_Commerce.Models;

namespace Projet_Web_Commerce.Controllers
{
    public class PPProduitsController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<Utilisateur> _userManager;

        public PPProduitsController(AuthDbContext context, UserManager<Utilisateur> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: PPProduits
        public async Task<IActionResult> Index()
        {
            var authDbContext = _context.PPProduits.Include(p => p.PPCategories).Include(p => p.PPVendeurs);
            return View(await authDbContext.ToListAsync());
        }

        // GET: PPProduits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pPProduits = await _context.PPProduits
                .Include(p => p.PPCategories)
                .Include(p => p.PPVendeurs)
                .FirstOrDefaultAsync(m => m.NoProduit == id);
            if (pPProduits == null)
            {
                return NotFound();
            }

            return View(pPProduits);
        }

        // GET: PPProduits/Create
        public IActionResult Create()
        {
            ViewData["NoCategorie"] = new SelectList(_context.PPCategories, "NoCategorie", "Description");
            ViewData["NoVendeur"] = new SelectList(_context.PPVendeurs, "NoVendeur", "Nom");
            return View();
        }

        // POST: PPProduits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NoVendeur,NoCategorie,Nom,Description,PrixDemande,Disponibilite,Poids,PrixVente, NombreItems")] PPProduits pPProduits,
            IFormFile? file)
        {
            var user = await _userManager.GetUserAsync(User);
            var vendeur = _context.PPVendeurs.FirstOrDefault(v => v.IdUtilisateur == user.Id);
            pPProduits.NoVendeur = vendeur.NoVendeur;
            pPProduits.DateCreation = DateTime.Now;
            pPProduits.DateVente = DateTime.Now;
            pPProduits.DateMAJ = DateTime.Now;
           
            Console.WriteLine($"User Id: {user?.Id}, User Name: {user?.UserName}");
            Console.WriteLine($"Photo: {file.FileName}");
            ModelState.Remove("file");
            ModelState.Remove("Photo");
            foreach (var m in ModelState)
            {
                foreach (var er in m.Value.Errors)
                {
                    Console.WriteLine(m.Key);
                    Console.WriteLine(er.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {
                int highestNoProduit = _context.PPProduits.Max(p => (int?)p.NoProduit) ?? -1;
                pPProduits.NoProduit = highestNoProduit + 1;

                pPProduits.Photo = file.FileName;
                string tempFilePath = Path.Combine("wwwroot/Logo", file.FileName);

                using (Stream fileStream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                _context.Add(pPProduits);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NoCategorie"] = new SelectList(_context.PPCategories, "NoCategorie", "Description", pPProduits.NoCategorie);
            ViewData["NoVendeur"] = new SelectList(_context.PPVendeurs, "NoVendeur", "Nom", pPProduits.NoVendeur);
            return View(pPProduits);
        }

        // GET: PPProduits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pPProduits = await _context.PPProduits.FindAsync(id);
            if (pPProduits == null)
            {
                return NotFound();
            }
            ViewData["NoCategorie"] = new SelectList(_context.PPCategories, "NoCategorie", "NoCategorie", pPProduits.NoCategorie);
            ViewData["NoVendeur"] = new SelectList(_context.PPVendeurs, "NoVendeur", "NoVendeur", pPProduits.NoVendeur);
            return View(pPProduits);
        }

        // POST: PPProduits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NoProduit,NoVendeur,NoCategorie,DateCreation,DateMAJ,Nom,Description,Photo,PrixDemande,Disponibilite,Poids,DateVente,PrixVente")] PPProduits pPProduits)
        {
            if (id != pPProduits.NoProduit)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pPProduits);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PPProduitsExists(pPProduits.NoProduit))
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
            ViewData["NoCategorie"] = new SelectList(_context.PPCategories, "NoCategorie", "NoCategorie", pPProduits.NoCategorie);
            ViewData["NoVendeur"] = new SelectList(_context.PPVendeurs, "NoVendeur", "NoVendeur", pPProduits.NoVendeur);
            return View(pPProduits);
        }

        // GET: PPProduits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pPProduits = await _context.PPProduits
                .Include(p => p.PPCategories)
                .Include(p => p.PPVendeurs)
                .FirstOrDefaultAsync(m => m.NoProduit == id);
            if (pPProduits == null)
            {
                return NotFound();
            }

            return View(pPProduits);
        }

        // POST: PPProduits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pPProduits = await _context.PPProduits.FindAsync(id);
            if (pPProduits != null)
            {
                _context.PPProduits.Remove(pPProduits);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PPProduitsExists(int id)
        {
            return _context.PPProduits.Any(e => e.NoProduit == id);
        }
    }
}
