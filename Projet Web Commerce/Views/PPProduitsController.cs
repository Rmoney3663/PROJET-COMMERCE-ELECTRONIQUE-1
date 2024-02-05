using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projet_Web_Commerce.Data;
using Projet_Web_Commerce.Models;

namespace Projet_Web_Commerce.Views
{
    public class PPProduitsController : Controller
    {
        private readonly AuthDbContext _context;

        public PPProduitsController(AuthDbContext context)
        {
            _context = context;
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
            ViewData["NoCategorie"] = new SelectList(_context.PPCategories, "NoCategorie", "NoCategorie");
            ViewData["NoVendeur"] = new SelectList(_context.PPVendeurs, "NoVendeur", "NoVendeur");
            return View();
        }

        // POST: PPProduits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NoProduit,NoVendeur,NoCategorie,DateCreation,DateMAJ,Nom,Description,Photo,PrixDemande,Disponibilite,Poids,DateVente,PrixVente")] PPProduits pPProduits)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pPProduits);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NoCategorie"] = new SelectList(_context.PPCategories, "NoCategorie", "NoCategorie", pPProduits.NoCategorie);
            ViewData["NoVendeur"] = new SelectList(_context.PPVendeurs, "NoVendeur", "NoVendeur", pPProduits.NoVendeur);
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
