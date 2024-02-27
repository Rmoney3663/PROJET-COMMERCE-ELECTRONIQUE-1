using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;
using Projet_Web_Commerce.Models;

namespace Projet_Web_Commerce.Controllers
{
    [Authorize(Roles = "Client")]
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

            if (listPaniers.ToList<PPArticlesEnPanier>().Count <= 0)
            {
                return RedirectToAction("Index", "Paniers");
            }

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
        public ActionResult ConfirmerCommande(int NoClient, int NoVendeur)
        {

            var client = _context.PPClients.FirstOrDefault(c => c.NoClient == NoClient);
            var vendeur = _context.PPVendeurs.FirstOrDefault(v => v.NoVendeur == NoVendeur);

            ViewBag.Provinces = _context.Province.ToList();

            var articlesEnPanier = _context.PPArticlesEnPanier.Where(p => p.NoClient == NoClient && p.NoVendeur == NoVendeur).ToList();

            var poidsTotal = articlesEnPanier
                .Join(_context.PPProduits,
                      article => article.NoProduit,
                      produit => produit.NoProduit,
                      (article, produit) => article.NbItems * produit.Poids)
                .Sum();

            var sousTotal = articlesEnPanier
                .Join(_context.PPProduits,
                      article => article.NoProduit,
                      produit => produit.NoProduit,
                      (article, produit) => produit.PrixVente * article.NbItems ?? produit.PrixDemande * article.NbItems)
                .Sum();

            decimal taxes = 0;

            if (vendeur.Taxes == true)
            {
                if (vendeur.Province == _context.Province.Where(p => p.ProvinceID == "QC").FirstOrDefault())
                {
                    var tvq = _context.PPTaxeProvinciale.Select(t => t.TauxTVQ).FirstOrDefault();
                    taxes += sousTotal * (tvq / 100);
                }

                var tps = _context.PPTaxeFederale.Select(t => t.TauxTPS).FirstOrDefault();
                taxes += sousTotal * (tps / 100);

                sousTotal += taxes;
            }

            ModelConfirmerCommande model = new ModelConfirmerCommande()
            {
                client = client,
                vendeur = vendeur,
                poidsTotal = poidsTotal.Value,
                sousTotal = sousTotal
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(int id, int delete, int vendeur, int nb)
        {
            Console.WriteLine("HELP");
            if (delete == 3)
            {
                foreach(var article in _context.PPArticlesEnPanier.Where(a=>a.NoVendeur == vendeur && a.NoClient == id).ToList())
                {
                    _context.PPArticlesEnPanier.Remove(article);
                }
            }
            else
            {
                var article = _context.PPArticlesEnPanier.Where(v => v.NoPanier == id).FirstOrDefault();


                if (article != null)
                {
                    if (delete == 1)
                    {
                        _context.PPArticlesEnPanier.Remove(article);
                    }
                    else if (delete == 2)
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
                            article.NbItems = produit.NombreItems.Value;
                        }
                        _context.PPArticlesEnPanier.Update(article);
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Paniers");
                }
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
