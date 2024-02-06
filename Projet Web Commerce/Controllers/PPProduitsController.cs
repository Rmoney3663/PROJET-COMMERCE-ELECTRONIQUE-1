using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using NuGet.Protocol.Plugins;
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

            if (pPProduits.PrixVente > pPProduits.PrixDemande)
            {
                ModelState.AddModelError("PrixVente", "Le prix de vente ne peut pas être supérieur au prix demandé.");
            }

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
                int produitCount = _context.PPProduits.Count();
                var id = produitCount + 1;
                int NoVendeur = pPProduits.NoVendeur;
                string combined = NoVendeur.ToString() + id.ToString();
                pPProduits.NoProduit = int.Parse(combined);
                var tempFilename = "temp_filename";
                string extension = Path.GetExtension(file.FileName);
                pPProduits.Photo = file.FileName;
                string tempFilePath = Path.Combine("wwwroot/Logo", tempFilename + extension);

                using (Stream fileStream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                _context.Add(pPProduits);
                await _context.SaveChangesAsync();
                var finalFilename = $"{int.Parse(combined)}{extension}";
                var finalFilePath = Path.Combine("wwwroot/Logo", finalFilename);
                System.IO.File.Move(tempFilePath, finalFilePath);

                pPProduits.Photo = finalFilename;
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
            ViewData["NoCategorie"] = new SelectList(_context.PPCategories, "NoCategorie", "Description", pPProduits.NoCategorie);
            ViewData["NoVendeur"] = new SelectList(_context.PPVendeurs, "NoVendeur", "Nom", pPProduits.NoVendeur);
            return View(pPProduits);
        }

        // POST: PPProduits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( [Bind("NoProduit,NoVendeur,NoCategorie,DateCreation,DateMAJ,Nom,Description,Photo,PrixDemande,Disponibilite,Poids,DateVente,PrixVente, NombreItems")] PPProduits pPProduits,
            IFormFile? file)
        {
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
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    pPProduits.DateMAJ = DateTime.Now;
                    
                    if (file != null && file.Length > 0)
                    {                        
                        System.IO.File.Delete("wwwroot/Logo/" + pPProduits.Photo);

                        string extension = Path.GetExtension(file.FileName);
                        pPProduits.Photo = pPProduits.NoProduit.ToString() + extension;
                        string tempFilePath = Path.Combine("wwwroot/Logo", pPProduits.NoProduit + extension);
                        using (Stream fileStream = new FileStream(tempFilePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                    }                    

                    _context.Update(pPProduits);
                    Console.WriteLine(" pPProduits : " + pPProduits.DateMAJ);
                    Console.WriteLine(" pPProduits : " + pPProduits.NombreItems);
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
            ViewData["NoCategorie"] = new SelectList(_context.PPCategories, "NoCategorie", "Description", pPProduits.NoCategorie);
            ViewData["NoVendeur"] = new SelectList(_context.PPVendeurs, "NoVendeur", "Nom", pPProduits.NoVendeur);
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
            try
            {
                Console.WriteLine("ID : " + id);
                var pPProduits = await _context.PPProduits.FindAsync(id);
                if (pPProduits != null)
                {
                    Console.WriteLine("Found produit");
                    System.IO.File.Delete("wwwroot/Logo/" + pPProduits.Photo);
                    Console.WriteLine("Deleted photo");
                    _context.PPProduits.Remove(pPProduits);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction("Error", "PPProduits");
            }
        }

        private bool PPProduitsExists(int id)
        {
            return _context.PPProduits.Any(e => e.NoProduit == id);

        }

        public IActionResult ErrorProduit()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
