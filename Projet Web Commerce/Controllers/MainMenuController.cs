using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;
using Projet_Web_Commerce.Models;
using System.Globalization;
using System.Security.Claims;

namespace Projet_Web_Commerce.Controllers
{
    public class MainMenuController : Controller
    {

        private readonly AuthDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<Utilisateur> _userManager;

        public MainMenuController(AuthDbContext context, Microsoft.AspNetCore.Identity.UserManager<Utilisateur> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: MainMenuController

        public ActionResult Catalogue()
        {
            var CategoriesList = _context.PPCategories.ToList();
            var VendeursList = _context.PPVendeurs.ToList();
            var ProduitsList = _context.PPProduits.ToList();


            ModelCatalogue modelCatalogue = new ModelCatalogue()
            {
                CategoriesList = CategoriesList,
                VendeursList = VendeursList,
                ProduitsList = ProduitsList
            };

            return View(modelCatalogue);
        }

        [HttpGet]
        public ActionResult GestionVendeurs()
        {
            if (User.IsInRole("Gestionnaire"))
            {
                var vendeursStatutZero = _context.PPVendeurs
                .Where(v => v.Statut == 0)
                .OrderBy(v => v.DateCreation)  // Assuming DateCreation is the property you want to order by
                .ToList();

                return View(vendeursStatutZero);
            }

            return Redirect("AccessDenied");

        }

        public IActionResult CatalogueVendeur(string id)
        {
            // Action logic
            var vendeur = _context.PPVendeurs.Where(v => v.NomAffaires == id).FirstOrDefault();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var client = _context.PPClients.FirstOrDefault(c => c.IdUtilisateur == userId);

            var nouveauxProduits = _context.PPProduits
                .Where(p => p.NoVendeur == vendeur.NoVendeur)
                .OrderBy(v => v.DateCreation)
                .Take(15)
                .ToList();

            var CategoriesList = _context.PPCategories.ToList();
            var VendeursList = _context.PPVendeurs.ToList();

            var ProduitsVendeur = _context.PPProduits
                .Where(p => p.NoVendeur == vendeur.NoVendeur && p.NombreItems > 0)
                .ToList();

            ModelCatalogueVendeur modelCatalogueVendeur;

            if (client != null)
            {
                modelCatalogueVendeur = new ModelCatalogueVendeur()
                {
                    nomAffaire = id,
                    CategoriesList = CategoriesList,
                    VendeursList = VendeursList,
                    ProduitsList = ProduitsVendeur,
                    NouveauxProduits = nouveauxProduits,
                    noClient = client.NoClient
                };
            }
            else
            {
                modelCatalogueVendeur = new ModelCatalogueVendeur()
                {
                    nomAffaire = id,
                    CategoriesList = CategoriesList,
                    VendeursList = VendeursList,
                    ProduitsList = ProduitsVendeur,
                    NouveauxProduits = nouveauxProduits
                };
            }

            return View(modelCatalogueVendeur);
        }



        [HttpPost]
        public ActionResult GestionVendeurs(int NoVendeur)
        {
            if (User.IsInRole("Gestionnaire"))
            {
                var vendeurToUpdate = _context.PPVendeurs.FirstOrDefault(v => v.NoVendeur == NoVendeur);

                if (vendeurToUpdate != null)
                {
                    // Update the properties of the vendeur
                    vendeurToUpdate.Statut = 1;

                    // Save changes to the database
                    _context.SaveChanges();

                    var vendeursStatutZero = _context.PPVendeurs
                    .Where(v => v.Statut == 0)
                    .OrderBy(v => v.DateCreation)  // Assuming DateCreation is the property you want to order by
                    .ToList();

                    return View(vendeursStatutZero);
                }
            }

            return Redirect("AccessDenied");

        }

        [HttpPost]
        public ActionResult AjoutPanier(int quantite, int NoProduit, int NoClient, int NoVendeur)
        {
            var vendeur = _context.PPVendeurs.Where(v => v.NoVendeur == NoVendeur).FirstOrDefault();
            var produit = _context.PPProduits.Where(v => v.NoProduit == NoProduit).FirstOrDefault();

            if (produit != null && vendeur != null)
            {
                var ajoutPanier = new PPArticlesEnPanier
                {
                    NoClient = NoClient, // Provide the NoClient value
                    NoVendeur = NoVendeur, // Provide the NoVendeur value
                    NoProduit = NoProduit, // Provide the NoProduit value
                    DateCreation = DateTime.Now, // Set the creation date
                    NbItems = quantite // Set the number of items
                };


                // Ajout au panier
                _context.PPArticlesEnPanier.Add(ajoutPanier);

                // Save changes to the database
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"Le produit {produit.Nom} à été ajout au panier.  ";

                return RedirectToAction("CatalogueVendeur", new { id = vendeur.NomAffaires });
            }


            return RedirectToAction("Catalogue");
           
        }

        // GET: MainMenuController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MainMenuController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MainMenuController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MainMenuController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MainMenuController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MainMenuController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MainMenuController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
