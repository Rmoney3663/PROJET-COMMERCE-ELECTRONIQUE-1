using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;
using Projet_Web_Commerce.Models;
using System.Globalization;

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
            var listPaniers = from unPanier in _context.PPArticlesEnPanier
                              where unPanier.PPClients.AdresseEmail == User.Identity.Name
                              group unPanier.NoPanier by unPanier.NoVendeur into grouper
                              select new { vendeur = grouper.Key, articles = grouper.ToList() };

            Console.WriteLine("QAAAAAAAAAAAAAAAAAAAAAAAA");
            Console.WriteLine(listPaniers.ToList<object>().ToString());

            ViewData["listPaniers"] = listPaniers.ToList<object>();


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

            Console.WriteLine(id);
            // Action logic
            var vendeur = _context.PPVendeurs.Where(v => v.NomAffaires == id).FirstOrDefault();



            var produitsVendeur = _context.PPProduits
                .Where(p => p.NoVendeur == vendeur.NoVendeur)
                .OrderBy(v => v.DateCreation)
                .Take(15)
                .ToList();

            var CategoriesList = _context.PPCategories.ToList();
            var VendeursList = _context.PPVendeurs.ToList();


            ModelCatalogue modelCatalogue = new ModelCatalogue()
            {
                CategoriesList = CategoriesList,
                VendeursList = VendeursList,
                ProduitsList = produitsVendeur
            };

            return View(modelCatalogue);
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
