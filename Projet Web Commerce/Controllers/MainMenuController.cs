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

            return View(CategoriesList);
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
        public ActionResult Index(int NoVendeur)
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
                }
            }

            var vendeursStatutZero = _context.PPVendeurs
            .Where(v => v.Statut == 0)
            .OrderBy(v => v.DateCreation)  // Assuming DateCreation is the property you want to order by
            .ToList();

            var CategoriesList = _context.PPCategories.ToList();

            var model = new ModelMainMenu
            {
                VendeursList = vendeursStatutZero,
                CategoriesList = CategoriesList
            };

            return View(model);
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
