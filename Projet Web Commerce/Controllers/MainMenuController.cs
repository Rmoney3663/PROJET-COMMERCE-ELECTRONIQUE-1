using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Spreadsheet;
﻿using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;
using Projet_Web_Commerce.Models;
using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        
        [AllowAnonymous]
        //[Authorize(Roles = "Client,Gestionnaire")]
        public ActionResult Catalogue()
        {
            if (User.IsInRole("Vendeur"))
            {
                return RedirectToAction("Index", "PPProduits", new { area = "" });
            }
            var CategoriesList = _context.PPCategories.ToList();
            var VendeursList = _context.PPVendeurs.ToList();
            var ProduitsList = _context.PPProduits.ToList();
            var listPaniers = from unPanier in _context.PPArticlesEnPanier
                              where unPanier.PPClients.AdresseEmail == User.Identity.Name
                              group unPanier.NoPanier by unPanier.NoVendeur into grouper
                              select new { vendeur = grouper.Key, articles = grouper.ToList() };

            ViewData["listPaniers"] = listPaniers.ToList<object>();


            ModelCatalogue modelCatalogue = new ModelCatalogue()
            {
                CategoriesList = CategoriesList,
                VendeursList = VendeursList,
                ProduitsList = ProduitsList
            };

            return View(modelCatalogue);
        }

        //[HttpGet]
        //public async Task<IActionResult> CatalogueVendeurAsync(string id)
        //{

        //    var model = new ModelCatalogueVendeur();

        //    model.sortOrder = 

        //    return View(model);
        //}

        [HttpPost]
        public async Task<IActionResult> CatalogueVendeurAsync(string id, ModelCatalogueVendeur model)
        {

            // Action logic
            var vendeur = _context.PPVendeurs.Where(v => v.NomAffaires == id).FirstOrDefault();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var client = _context.PPClients.FirstOrDefault(c => c.IdUtilisateur == userId);

            model.CategoriesList = _context.PPCategories.ToList();
            model.NouveauxProduits = _context.PPProduits
           .OrderBy(v => v.DateCreation)
           .Take(15)
           .ToList();

            //if (model.parPage != "Tous")
            //{
            //    ViewBag.parPage = model.parPage ?? "15";
            //}
            //else
            //{
            //    ViewBag.parPage = "Tous";
            //}

            //ViewBag.id = id;

            model.sortOrderOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "dateAsc", Text = "Date de parution ↑" },
                new SelectListItem { Value = "dateDesc", Text = "Date de parution ↓" },
                new SelectListItem { Value = "numProdAsc", Text = "Numéro de produit ↑" },
                new SelectListItem { Value = "numProdDesc", Text = "Numéro de produit ↓" },
                new SelectListItem { Value = "catProdAsc", Text = "Catégorie particulière de produit ↑" },
                new SelectListItem { Value = "catProdDesc", Text = "Catégorie particulière de produit ↓" },
                new SelectListItem { Value = "descProdAsc", Text = "Description du produit ↑" },
                new SelectListItem { Value = "descProdDesc", Text = "Description du produit ↓" }
            };

            model.parPageOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "6", Text = "6" },
                new SelectListItem { Value = "9", Text = "9" },
                new SelectListItem { Value = "12", Text = "12" },
                new SelectListItem { Value = "15", Text = "15" },
                new SelectListItem { Value = "21", Text = "21" },
                new SelectListItem { Value = "30", Text = "30" },
                new SelectListItem { Value = "60", Text = "60" },
                new SelectListItem { Value = "Tous", Text = "Tous" }
            };

            model.categorieOptions = _context.PPProduits
            .Where(p => p.NoVendeur == vendeur.NoVendeur)
            .Join(_context.PPCategories,
                produit => produit.NoCategorie,
                categorie => categorie.NoCategorie,
                (produit, categorie) => categorie.Description)
            .Distinct()
            .Select(category => new SelectListItem
            {
                Value = category,
                Text = category
            })
            .ToList();

            IQueryable<PPProduits> ProduitsVendeur;
            // Recherche string description et nom + verif dispo et vendeur selectionner
            if (!String.IsNullOrEmpty(model.searchString))
            {
                int intValue;
                ProduitsVendeur = from p in _context.PPProduits
                    .Where(p => p.NoVendeur == vendeur.NoVendeur && p.Disponibilite == true && (p.Nom.Contains(model.searchString) || p.Description.Contains(model.searchString) || (int.TryParse(model.searchString, out intValue) && p.NoProduit == intValue)))
                    select p;
            }
            else
            {
                ProduitsVendeur = from p in _context.PPProduits
                    .Where(p => p.NoVendeur == vendeur.NoVendeur && p.Disponibilite == true)
                                  select p;
            }

            // Recherche par date
            if (model.dateApres != null)
            {
                ProduitsVendeur = ProduitsVendeur.Where(p => p.DateCreation > model.dateApres);
            }

            if (model.dateAvant != null)
            {
                ProduitsVendeur = ProduitsVendeur.Where(p => p.DateCreation < model.dateAvant);
            }

            // Filtrer par categorie
            if (!String.IsNullOrEmpty(model.searchCat))
            {
                var categorySelect = _context.PPCategories
                .Where(c => c.Description == model.searchCat)
                .Select(c => c.NoCategorie)
                .FirstOrDefault();

                if (categorySelect != null)
                {
                    ProduitsVendeur = ProduitsVendeur
                    .Where(p => p.NoVendeur == vendeur.NoVendeur && p.NoCategorie == categorySelect);
                }
            }

            switch (model.sortOrder)
            {
                case "dateAsc":
                    ProduitsVendeur = ProduitsVendeur.OrderBy(p => p.DateCreation);
                    break;
                case "dateDesc":
                    ProduitsVendeur = ProduitsVendeur.OrderByDescending(p => p.DateCreation);
                    break;
                case "numProdAsc":
                    ProduitsVendeur = ProduitsVendeur.OrderBy(p => p.NoProduit);
                    break;
                case "numProdDesc":
                    ProduitsVendeur = ProduitsVendeur.OrderByDescending(p => p.NoProduit);
                    break;
                case "catProdAsc":
                    ProduitsVendeur = ProduitsVendeur.OrderBy(p => p.NoCategorie);
                    break;
                case "catProdDesc":
                    ProduitsVendeur = ProduitsVendeur.OrderByDescending(p => p.NoCategorie);
                    break;
                case "descProdAsc":
                    ProduitsVendeur = ProduitsVendeur.OrderBy(p => p.Description);
                    break;
                case "descProdDesc":
                    ProduitsVendeur = ProduitsVendeur.OrderByDescending(p => p.Description);
                    break;
            }

            int pageSize;

            if (model.parPage == null)
            {
                model.parPage = "15";
            }

            if (model.parPage == "Tous")
            {
                pageSize = 999;
            }
            else
            {
                pageSize = Convert.ToInt32(model.parPage);
            }

            var produitsVendeurQueryable = ProduitsVendeur.AsQueryable();

            var produitsVendeurPaginated = await PaginatedList<PPProduits>.CreateAsync(
                produitsVendeurQueryable,
                model.pageNumber ?? 1,
                pageSize);



            return View(model);


        }

        [HttpPost]
        public ActionResult AjoutPanier(int quantite, int NoProduit, int NoClient, int NoVendeur, ModelCatalogueVendeur model)
        {
            var vendeur = _context.PPVendeurs.Where(v => v.NoVendeur == NoVendeur).FirstOrDefault();
            var produit = _context.PPProduits.Where(v => v.NoProduit == NoProduit).FirstOrDefault();

            var articlesEnPanier = _context.PPArticlesEnPanier
            .Where(a => a.NoProduit == NoProduit && a.NoClient == NoClient)
            .ToList();

            // Calculate the total quantity of items in the cart for the given product
            int quantiteDansPanier = articlesEnPanier.Sum(a => a.NbItems);

            Console.WriteLine(model.searchString);
            if (produit.NombreItems >= quantite + quantiteDansPanier) // Verif nb items panier plus petit que stock
            {
                if (produit != null && vendeur != null)
                {
                    if (!articlesEnPanier.Any(a => a.NoProduit == produit.NoProduit)) // Verif si panier meme produit existe
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
                    }
                    else
                    {
                        var panierModif = articlesEnPanier.Where(a => a.NoProduit == produit.NoProduit).FirstOrDefault();
                        panierModif.NbItems = panierModif.NbItems + quantite;
                    }

                    _context.SaveChanges();

                    TempData["SuccessMessage"] = $"Le produit {produit.Nom} à été ajouté au panier.  ";

                    return RedirectToAction("CatalogueVendeur", new
                    {
                        id = vendeur.NomAffaires,
                        searchString = model.searchString
                    });

                }

            }
            else
            {
                TempData["ErrorMessage"] = $"Le produit {produit.Nom} n'a pas été ajout au panier. Assurez-vous que le panier n'excède pas le nombre d'items en stock. ";
                return RedirectToAction("CatalogueVendeur", new { id = vendeur.NomAffaires });
            }


            return RedirectToAction("Catalogue");

        }

        //[Route("/MainMenuController/ValiderAsync")]
        //[Authorize(Roles = "Gestionnaire")]
        //public async Task<IActionResult> ValiderAsync(int id, string sujet, string message, bool vendeurAccepte, int pourcentage)
        //{
        //    var vendeurAUpdate = _context.PPVendeurs.FirstOrDefault(v => v.NoVendeur == id);

        //    var result = new
        //    {
        //        Success = false,
        //        Message = $"Erreur%"
        //    };

        //    if (vendeurAUpdate != null)
        //    {
        //        var user = await _userManager.FindByEmailAsync(vendeurAUpdate.AdresseEmail);
        //        if (user != null)
        //        {
        //            await Methodes.envoyerCourriel(vendeurAUpdate.AdresseEmail, sujet, message);
        //            if (vendeurAccepte)
        //            {
        //                vendeurAUpdate.Statut = 1;
        //                vendeurAUpdate.Pourcentage = Convert.ToDecimal(pourcentage, CultureInfo.InvariantCulture);
        //            }
        //            else
        //            {
        //                _context.PPVendeurs.Remove(vendeurAUpdate);
        //                _userManager.DeleteAsync(user);
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

        [HttpPost]
        public ActionResult GestionVendeurs(int NoVendeur, string Pourcentage, bool vendeurAccepte)
        {
            if (User.IsInRole("Gestionnaire"))
            {
                var vendeurAUpdate = _context.PPVendeurs.FirstOrDefault(v => v.NoVendeur == NoVendeur);

                if (vendeurAUpdate != null)
                {
                    if (vendeurAccepte)
                    {
                        vendeurAUpdate.Statut = 1;
                        vendeurAUpdate.Pourcentage = Convert.ToDecimal(Pourcentage, CultureInfo.InvariantCulture);
                    }
                    else
                        _context.PPVendeurs.Remove(vendeurAUpdate);

                    _context.SaveChanges();

                    var vendeursStatutZero = _context.PPVendeurs
                            .Where(v => v.Statut == 0)
                            .OrderBy(v => v.DateCreation)
                            .ToList();

                    return View(vendeursStatutZero);
                }
            }
            return Redirect("AccessDenied");
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
