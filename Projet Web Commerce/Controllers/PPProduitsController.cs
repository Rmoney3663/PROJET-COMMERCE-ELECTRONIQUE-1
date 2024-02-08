using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Vendeur, Client")]
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
                return RedirectToAction("ErrorNoFound", "PPProduits");
            }

            var pPProduits = await _context.PPProduits
                .Include(p => p.PPCategories)
                .Include(p => p.PPVendeurs)
                .FirstOrDefaultAsync(m => m.NoProduit == id);
            if (pPProduits == null)
            {
                return RedirectToAction("ErrorNoFound", "PPProduits");
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
        public async Task<IActionResult> Create([Bind("NoVendeur,NoCategorie,Nom,Description,PrixDemande,Disponibilite,Poids,PrixVente, NombreItems, DateVente")] PPProduits pPProduits,
            IFormFile? file)
        {
            var user = await _userManager.GetUserAsync(User);
            var vendeur = _context.PPVendeurs.FirstOrDefault(v => v.IdUtilisateur == user.Id);
            pPProduits.NoVendeur = vendeur.NoVendeur;
            pPProduits.DateCreation = DateTime.Now;
            pPProduits.DateMAJ = DateTime.Now;


            Console.WriteLine("DATE : " + pPProduits.DateVente);
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
                var highestId = 0;
                foreach (var produit in _context.PPProduits.Where(v => v.NoVendeur == vendeur.NoVendeur))
                {
                    var idString = produit.NoProduit.ToString();
                    var substring = idString.Length > 2 ? idString.Substring(2) : idString;
                    var idWithoutFirstTwoDigits = int.Parse(substring);
                    if (idWithoutFirstTwoDigits > highestId)
                    {
                        highestId = idWithoutFirstTwoDigits;
                    }
                }

                var id = highestId + 1;

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
                return RedirectToAction("ErrorNoFound", "PPProduits");
            }

            var pPProduits = await _context.PPProduits.FindAsync(id);
            if (pPProduits == null)
            {
                return RedirectToAction("ErrorNoFound", "PPProduits");
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
            Console.WriteLine("DATE : " + pPProduits.DateVente);
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
                return RedirectToAction("ErrorNoFound", "PPProduits");
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

                    var inPanier = await _context.PPArticlesEnPanier.AnyAsync(a => a.NoProduit == id);

                    if (inPanier)
                    {
                        ViewBag.WarningMessage = "Ce produit est dans un panier. Êtes-vous sûr de vouloir le supprimer ?";
                        ViewBag.ProductId = id;
                        return View("ConfirmDelete", pPProduits);
                    }

                    var detailsCommandes = await _context.PPDetailsCommandes.FirstOrDefaultAsync(dc => dc.NoProduit == id);

                    if (detailsCommandes != null)
                    {
                        pPProduits.Disponibilite = false;
                        pPProduits.NombreItems = 0;
                        pPProduits.DateMAJ = DateTime.Now;
                        var detailsCommandesList = await _context.PPDetailsCommandes
                            .Where(dc => dc.NoProduit == id)
                            .ToListAsync();

                        foreach (var dc in detailsCommandesList)
                        {
                            dc.Quantité = 0;
                            _context.PPDetailsCommandes.Update(dc);
                        }
                        _context.PPDetailsCommandes.Update(detailsCommandes);
                        _context.Update(pPProduits);
                    }
                    else
                    {
                        var articlesToDelete = _context.PPArticlesEnPanier.Where(a => a.NoProduit == id);
                        _context.PPArticlesEnPanier.RemoveRange(articlesToDelete);
                        System.IO.File.Delete("wwwroot/Logo/" + pPProduits.Photo);
                        Console.WriteLine("Deleted photo");
                        _context.PPProduits.Remove(pPProduits);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction("Error", "PPProduits");
            }
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed2(int id)
        {
            try
            {
                Console.WriteLine("ID : " + id);
                var pPProduits = await _context.PPProduits.FindAsync(id);
                if (pPProduits != null)
                {
                    Console.WriteLine("Found produit");
                    var detailsCommandes = await _context.PPDetailsCommandes.FirstOrDefaultAsync(dc => dc.NoProduit == id);
                    
                    var articlesToDelete = _context.PPArticlesEnPanier.Where(a => a.NoProduit == id);
                    _context.PPArticlesEnPanier.RemoveRange(articlesToDelete);

                    if (detailsCommandes != null)
                    {
                        pPProduits.Disponibilite = false;
                        pPProduits.NombreItems = 0;
                        pPProduits.DateMAJ = DateTime.Now;
                        var detailsCommandesList = await _context.PPDetailsCommandes
                            .Where(dc => dc.NoProduit == id)
                            .ToListAsync();

                        foreach (var dc in detailsCommandesList)
                        {
                            dc.Quantité = 0;
                            _context.PPDetailsCommandes.Update(dc);
                        }
                        _context.PPDetailsCommandes.Update(detailsCommandes);
                        _context.Update(pPProduits);
                    }
                    else
                    {                        
                        System.IO.File.Delete("wwwroot/Logo/" + pPProduits.Photo);
                        Console.WriteLine("Deleted photo");
                        _context.PPProduits.Remove(pPProduits);
                    }
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

        public IActionResult ErrorNoFound()
        {
            return View();
        }

        public async Task<IActionResult> ListePanier()
        {

            var authDbContext = _context.PPProduits.Include(p => p.PPCategories).Include(p => p.PPVendeurs);
            return View(await authDbContext.ToListAsync());
        }

        public async Task<IActionResult> DetailsPanier(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ErrorNoFound", "PPProduits");
            }

            var pPClients = await _context.PPClients
                .FirstOrDefaultAsync(m => m.NoClient == id);
            if (pPClients == null)
            {
                return RedirectToAction("ErrorNoFound", "PPProduits");
            }

            return View(pPClients);
        }

        public async Task<IActionResult> SupprimerPanier(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ErrorNoFound", "PPProduits");
            }

            var pPClients = await _context.PPClients
                .FirstOrDefaultAsync(m => m.NoClient == id);
            if (pPClients == null)
            {
                return RedirectToAction("ErrorNoFound", "PPProduits");
            }

            return View(pPClients);
        }

        [HttpPost, ActionName("SupprimerPanier")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupprimerPanier(int noVendeur, int noClient)
        {
            try
            {
                Console.WriteLine("NOCLIENT : " + noVendeur + "   NOCLIENT : " + noClient);
                var pPClient = await _context.PPClients.FindAsync(noClient);
                var pPVendeur = await _context.PPVendeurs.FindAsync(noVendeur);
                if (pPClient != null && pPVendeur != null)
                {
                    Console.WriteLine("Found produit");

                    var articlesToDelete = _context.PPArticlesEnPanier.Where(a => a.NoVendeur == noVendeur && a.NoClient == noClient);
                    _context.PPArticlesEnPanier.RemoveRange(articlesToDelete);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction("Error", "PPProduits");
            }
        }
    }
}
