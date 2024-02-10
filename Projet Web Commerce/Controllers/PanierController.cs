using DocumentFormat.OpenXml.InkML;
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

            ViewData["Panier"] = listPaniers.ToList<PPArticlesEnPanier>();
            ViewData["VendeurId"] = id;

            ModelCatalogue modelCatalogue = new ModelCatalogue()
            {
                CategoriesList = CategoriesList,
                VendeursList = VendeursList,
                ProduitsList = ProduitsList
            };

            return View(modelCatalogue);
        }

        [HttpGet]
        public ActionResult ConfirmerCommande(int NoClient, int NoVendeur, decimal poidsTotal, decimal sousTotal)
        {

            var client = _context.PPClients.FirstOrDefault(c => c.NoClient == NoClient);
            var vendeur = _context.PPVendeurs.FirstOrDefault(v => v.NoVendeur == NoVendeur);

            ViewBag.Provinces = _context.Province.ToList();

            ModelConfirmerCommande model = new ModelConfirmerCommande()
            {
                client = client,
                vendeur = vendeur,
                poidsTotal = poidsTotal,
                sousTotal = sousTotal
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(int id, bool delete, int vendeur, int nb)
        {
            var article = _context.PPArticlesEnPanier.Where(v => v.NoPanier == id).FirstOrDefault();

            
            if (article != null)
            {
                if (delete)
                {
                    _context.PPArticlesEnPanier.Remove(article);
                }
                else
                {
                    var produit = _context.PPProduits.Where(v => v.NoProduit == article.NoProduit).FirstOrDefault();
                    if (nb <= produit.NombreItems)
                    {
                        article.NbItems = nb;
                        
                        if (article.NbItems <= 0)
                        {
                            article.NbItems = 1;
                        }
                    }
                    else
                    {
                        article.NbItems = produit.NombreItems;
                    }
                    _context.PPArticlesEnPanier.Update(article);
                }
            }
            else
            {
                return RedirectToAction("Index", "Paniers");
            }

            _context.SaveChanges();
            
            var CategoriesList = _context.PPCategories.ToList();
            var VendeursList = _context.PPVendeurs.ToList();
            var ProduitsList = _context.PPProduits.ToList();
            

            var listPaniers = from unPanier in _context.PPArticlesEnPanier
                              where unPanier.PPClients.AdresseEmail == User.Identity.Name
                              where unPanier.NoVendeur == vendeur
                              select unPanier;

            if (listPaniers.ToList<PPArticlesEnPanier>().Count <=0)
            {
                return RedirectToAction("Index", "Paniers");
            }

            ViewData["Panier"] = listPaniers.ToList<PPArticlesEnPanier>();
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
