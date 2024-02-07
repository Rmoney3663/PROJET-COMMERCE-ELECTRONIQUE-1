using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;
using Projet_Web_Commerce.Models;

namespace Projet_Web_Commerce.Controllers
{
    public class PanierController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<Utilisateur> _userManager;

        public PanierController(AuthDbContext context, Microsoft.AspNetCore.Identity.UserManager<Utilisateur> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: PanierController
        [HttpGet]
        public ActionResult Index(int id)
        {
            var CategoriesList = _context.PPCategories.ToList();
            var VendeursList = _context.PPVendeurs.ToList();
            var ProduitsList = _context.PPProduits.ToList();
            var listPaniers = from unPanier in _context.PPArticlesEnPanier
                              where unPanier.PPClients.AdresseEmail == User.Identity.Name
                              where unPanier.NoVendeur == id
                              select unPanier;

            ViewData["Panier"] = listPaniers.ToList<object>();
            ViewData["VendeurId"] = id;

            ModelCatalogue modelCatalogue = new ModelCatalogue()
            {
                CategoriesList = CategoriesList,
                VendeursList = VendeursList,
                ProduitsList = ProduitsList
            };

            return View(modelCatalogue);
        }

        [HttpPost]
        public ActionResult Index(int id, bool delete, int vendeur)
        {
            var article = _context.PPArticlesEnPanier.Where(v => v.NoPanier == id).FirstOrDefault();

            
            if (delete)
            {
                _context.PPArticlesEnPanier.Remove(article);
            }

            _context.SaveChanges();
            
            var CategoriesList = _context.PPCategories.ToList();
            var VendeursList = _context.PPVendeurs.ToList();
            var ProduitsList = _context.PPProduits.ToList();
            

            var listPaniers = from unPanier in _context.PPArticlesEnPanier
                              where unPanier.PPClients.AdresseEmail == User.Identity.Name
                              where unPanier.NoVendeur == vendeur
                              select unPanier;

            ViewData["Panier"] = listPaniers.ToList<object>();
            ViewData["VendeurId"] = vendeur;

            ModelCatalogue modelCatalogue = new ModelCatalogue()
            {
                CategoriesList = CategoriesList,
                VendeursList = VendeursList,
                ProduitsList = ProduitsList
            };

            return View(modelCatalogue);
        }
    }
}
