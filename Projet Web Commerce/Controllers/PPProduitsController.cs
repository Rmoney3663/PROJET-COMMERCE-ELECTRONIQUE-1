using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Packaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework.Profiler;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly SignInManager<Utilisateur> SignInManager;

        public PPProduitsController(IWebHostEnvironment webHostEnvironment, AuthDbContext context, UserManager<Utilisateur> userManager, SignInManager<Utilisateur> signInManager)
        {
            _context = context;
            _userManager = userManager;
            SignInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: PPProduits
        [Authorize(Roles = "Vendeur")]
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

        [HttpGet]
        public async Task<IActionResult> GestionCommandes()
        {
            var user = await _userManager.GetUserAsync(User);

            var noVendeur = _context.PPVendeurs.Where(c => c.AdresseEmail == user.Email).FirstOrDefault().NoVendeur;

            return View(await _context.PPCommandes.Where(c=>c.NoVendeur == noVendeur).OrderByDescending(o=>o.Statut).ThenByDescending(d => d.DateCommande).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> GestionCommandes(int noCommande)
        {
            var user = await _userManager.GetUserAsync(User);

            var noVendeur = _context.PPVendeurs.Where(c => c.AdresseEmail == user.Email).FirstOrDefault().NoVendeur;

            PPCommandes commande = _context.PPCommandes.Where(c => c.NoCommande == noCommande).FirstOrDefault();
            commande.Statut = "L";

            _context.SaveChanges();
            var commandes = await _context.PPCommandes.Where(c => c.NoVendeur == noVendeur).OrderByDescending(o => o.Statut).ThenByDescending(d => d.DateCommande).ToListAsync();

            return View(commandes);
        }

        public IActionResult Download(int id)
        {
            string fileName = $"{id}.pdf";
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "data", "pdf", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                ViewBag.ErrorMessage = $"Le fichier demandé '{fileName}' n'a pas été trouvé.";

                return View("FileNotFound");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/pdf", fileName);
        }

        [HttpPost]
        public async Task<IActionResult> Details(int? id, int noClient,string first,string ajout, bool eval, int rating, string msg, string source, ModelCatalogueVendeur model)
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

            DateTime dateCreate = DateTime.Now;
            DateTime dateMaj = DateTime.Now;
            PPEvaluations newEvaluation = new PPEvaluations();

            if (source == "tous")
            {
                ViewData["source"] = source;
                ViewData["searchString"] = model.searchString;
                ViewData["parPage"] = model.parPage;
                ViewData["dateApres"] = model.dateApres;
                ViewData["dateAvant"] = model.dateAvant;
                ViewData["searchCat"] = model.searchCat;
                ViewData["pageNumber"] = model.pageNumber;
                ViewData["sortOrder"] = model.sortOrder;
            }
            else if (source == "vendeur")
            {
                ViewData["source"] = source;
                ViewData["nomAffaire"] = model.nomAffaire;
                ViewData["searchString"] = model.searchString;
                ViewData["parPage"] = model.parPage;
                ViewData["dateApres"] = model.dateApres;
                ViewData["dateAvant"] = model.dateAvant;
                ViewData["searchCat"] = model.searchCat;
                ViewData["pageNumber"] = model.pageNumber;
                ViewData["sortOrder"] = model.sortOrder;
                ViewData["premiereConnexion"] = model.premiereConnexion;
            }

            if (first == null)
            {
                if (ajout == null)
                {
                    if (source == "tous")
                    {

                        return RedirectToAction("CatalogueTous", "MainMenu", new
                        {
                            searchString = model.searchString,
                            parPage = model.parPage,
                            dateApres = model.dateApres,
                            dateAvant = model.dateAvant,
                            searchCat = model.searchCat,
                            pageNumber = model.pageNumber,
                            sortOrder = model.sortOrder
                        });
                    }
                    else if (source == "vendeur")
                    {

                        return RedirectToAction("CatalogueVendeur", "MainMenu", new
                        {
                            nomAffaire = model.nomAffaire,
                            searchString = model.searchString,
                            parPage = model.parPage,
                            dateApres = model.dateApres,
                            dateAvant = model.dateAvant,
                            searchCat = model.searchCat,
                            pageNumber = model.pageNumber,
                            sortOrder = model.sortOrder,
                            premiereConnexion = model.premiereConnexion
                        });
                    }
                }

                if (ajout != null)
                {
                    if (eval)
                    {
                        var evalActu = _context.PPEvaluations.Where(e => e.NoClient == noClient && e.NoProduit == id).FirstOrDefault();
                        evalActu.Commentaire = msg != null ? msg.Trim() : msg;
                        evalActu.Cote = rating;
                        evalActu.DateMAJ = dateMaj;
                        _context.SaveChanges();
                    }
                    else
                    {
                        newEvaluation.NoProduit = (int)id;
                        newEvaluation.NoClient = noClient;
                        newEvaluation.Commentaire = msg != null ? msg.Trim() : msg;
                        newEvaluation.Cote = rating;
                        newEvaluation.DateCreation = dateCreate;
                        newEvaluation.DateMAJ = dateMaj;

                        _context.Add(newEvaluation);
                        _context.SaveChanges();
                    }

                }
                else { 
                }
            }

            ViewData["idProduit"] = id;

            return View(pPProduits);
        }

        // GET: PPProduits/Create
        [Authorize(Roles = "Vendeur")]
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
        [Authorize(Roles = "Vendeur")]
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
            if (pPProduits.PrixVente != null && pPProduits.DateVente == null)
            {
                ModelState.AddModelError("DateVente", "La date de vente est obligatoire, si vous mettez un prix de vente.");
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
                string tempFilePath = Path.Combine("wwwroot/data/images", tempFilename + extension);

                using (Stream fileStream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                _context.Add(pPProduits);
                await _context.SaveChangesAsync();
                var finalFilename = $"{int.Parse(combined)}{extension}";
                var finalFilePath = Path.Combine("wwwroot/data/images", finalFilename);
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
        [Authorize(Roles = "Vendeur")]
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
        [Authorize(Roles = "Vendeur")]
        public async Task<IActionResult> Edit([Bind("NoProduit,NoVendeur,NoCategorie,DateCreation,DateMAJ,Nom,Description,Photo,PrixDemande,Disponibilite,Poids,DateVente,PrixVente,NombreItems")] PPProduits pPProduits,
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

            if (pPProduits.PrixVente > pPProduits.PrixDemande)
            {
                ModelState.AddModelError("PrixVente", "Le prix de vente ne peut pas être supérieur au prix demandé.");
            }
            if (pPProduits.PrixVente != null && pPProduits.DateVente == null)
            {
                ModelState.AddModelError("DateVente", "La date de vente est obligatoire, si vous mettez un prix de vente.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    pPProduits.DateMAJ = DateTime.Now;

                    if (!pPProduits.Disponibilite)
                    {
                        var productInCart = _context.PPArticlesEnPanier.Any(p => p.NoProduit == pPProduits.NoProduit);

                        if (productInCart)
                        {
                            
                        }
                    }

                    if (file != null && file.Length > 0)
                    {
                        System.IO.File.Delete("wwwroot/data/images/" + pPProduits.Photo);

                        // Save new photo
                        string extension = Path.GetExtension(file.FileName);
                        pPProduits.Photo = pPProduits.NoProduit.ToString() + extension;
                        string tempFilePath = Path.Combine("wwwroot/data/images", pPProduits.NoProduit + extension);
                        using (Stream fileStream = new FileStream(tempFilePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                    }

                    _context.Update(pPProduits);
                    await _context.SaveChangesAsync();

                    Console.WriteLine(" pPProduits : " + pPProduits.DateMAJ);
                    Console.WriteLine(" pPProduits : " + pPProduits.NombreItems);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PPProduitsExists(pPProduits.NoProduit))
                    {
                        return RedirectToAction("ErrorNoFound", "PPProduits");
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


        [HttpGet]
        [Authorize(Roles = "Vendeur")]
        public async Task<IActionResult> IsProductInCart(int noProduit)
        {
            var productInCart = await _context.PPArticlesEnPanier.AnyAsync(p => p.NoProduit == noProduit);
            return Json(new { productInCart });
        }

        // GET: PPProduits/Delete/5
        [Authorize(Roles = "Vendeur")]
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
        [Authorize(Roles = "Vendeur")]
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
                        System.IO.File.Delete("wwwroot/data/images/" + pPProduits.Photo);
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
        [Authorize(Roles = "Vendeur")]
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
                        System.IO.File.Delete("wwwroot/data/images/" + pPProduits.Photo);
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

        [Authorize(Roles = "Vendeur")]
        public async Task<IActionResult> ListePanier()
        {

            var authDbContext = _context.PPProduits.Include(p => p.PPCategories).Include(p => p.PPVendeurs);
            return View(await authDbContext.ToListAsync());
        }

        [Authorize(Roles = "Vendeur")]
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
        [Authorize(Roles = "Vendeur")]
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
        [Authorize(Roles = "Vendeur")]
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

        //[Route("/PPProduitsController/ValiderAsync")]
        ////[Authorize(Roles = "Gestionnaire")]
        //public async Task<IActionResult> ValiderAsync(int noVendeur, string sujet, string message)
        //{
        //    var vendeurAEnvoyerCourriel = _context.PPVendeurs.FirstOrDefault(v => v.NoVendeur == noVendeur);

        //    var result = new
        //    {
        //        Success = false,
        //        Message = $"Erreur%"
        //    };

        //    if (vendeurAEnvoyerCourriel != null)
        //    {
        //        var user = await _userManager.FindByEmailAsync(vendeurAUpdate.AdresseEmail);
        //        if (user != null)
        //        {
        //            if (infosSupp != null)
        //                message += "\n\n" + infosSupp;
        //            await Methodes.envoyerCourriel(sujet, message, vendeurAUpdate.AdresseEmail);
        //            if (vendeurAccepte)
        //            {
        //                vendeurAUpdate.Statut = 1;
        //                vendeurAUpdate.Pourcentage = pourcentageRedevence;
        //                _context.Update(vendeurAUpdate);
        //                await _context.SaveChangesAsync();
        //            }
        //            else
        //            {
        //                _context.PPVendeurs.Remove(vendeurAUpdate);
        //                await _userManager.DeleteAsync(user);
        //            }
        //            _context.SaveChanges();

        //            var vendeursStatutZero = _context.PPVendeurs
        //                    .Where(v => v.Statut == 0)
        //                    .OrderBy(v => v.DateCreation)
        //                    .ToList();

        //            View(vendeursStatutZero);

        //            result = new
        //            {
        //                Success = true,
        //                Message = $"Courriel envoyé%."
        //            };
        //        }
        //    }

        //    return Json(result);
        //}
    }
}
