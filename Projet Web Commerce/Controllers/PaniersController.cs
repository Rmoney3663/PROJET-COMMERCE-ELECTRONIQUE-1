using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;
using Projet_Web_Commerce.Models;

namespace Projet_Web_Commerce.Controllers
{
    [Authorize(Roles = "Client")]
    public class PaniersController : Controller
    {

        private readonly AuthDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<Utilisateur> _userManager;

        public PaniersController(AuthDbContext context, Microsoft.AspNetCore.Identity.UserManager<Utilisateur> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: PanierController
        public ActionResult Index()
        {
            var CategoriesList = _context.PPCategories.ToList();
            var VendeursList = _context.PPVendeurs.ToList();
            var ProduitsList = _context.PPProduits.ToList();
            var listPaniers = from unPanier in _context.PPArticlesEnPanier
                              where unPanier.PPClients.AdresseEmail == User.Identity.Name
                              group unPanier.NoPanier by unPanier.NoVendeur into grouper
                              select new { vendeur = grouper.Key, articles = grouper.ToList() };

            ViewData["listPaniers"] = listPaniers.ToList<object>();

            var model = new ModelCatalogue
            {
                CategoriesList = CategoriesList,
                VendeursList = VendeursList,
                ProduitsList = ProduitsList
            };

            return View(model);
        }

        // GET: PanierController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PanierController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PanierController/Create
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

        // GET: PanierController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PanierController/Edit/5
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

        // GET: PanierController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PanierController/Delete/5
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
