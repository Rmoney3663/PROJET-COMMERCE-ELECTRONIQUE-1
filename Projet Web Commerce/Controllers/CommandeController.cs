using Microsoft.AspNetCore.Mvc;
using Projet_Web_Commerce.API;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;
using Projet_Web_Commerce.Models;

using System.Text;
using System.Text.Json;

namespace Projet_Web_Commerce.Controllers
{
    public class CommandeController : Controller
    {

        private readonly AuthDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<Utilisateur> _userManager;

        public CommandeController(AuthDbContext context, Microsoft.AspNetCore.Identity.UserManager<Utilisateur> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public ActionResult Commander(ModelConfirmerCommande model)
        {
            // Verif avant commande
            if (model.total < 5)
            {
                return RedirectToAction("Index", "Home");
            }

            var PostDataCommander = new PostDataCommander
            {
                NoVendeur = model.NoVendeur,
                NomVendeur = model.NomVendeur,
                NoCarteCredit = 1111123412341234,
                DateExpirationCarteCredit = model.dateExpiration,
                MontantPaiement = model.total,
                NomPageRetour = "google.ca",
                InfoSuppl = "Coucou"
            };

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://personnel.lmbrousseau.info/lesi-20XX/");

            var json = JsonSerializer.Serialize(PostDataCommander);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = httpClient.PostAsync("lesi-effectue-paiement.php", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var postResponse = JsonSerializer.Deserialize<PostResponseCommander>(responseContent);
                Console.WriteLine(postResponse.NoAutorisation);
            }
            else
            {
                Console.WriteLine("error : " + response.StatusCode);
            }

            return RedirectToAction("Index", "Home");
        }
        

        public ActionResult CommandeCompleter(PostResponseCommander response)
        {
            return View(response);
        }


        [HttpPost]
        public ActionResult ConfirmerCommande(ModelConfirmerCommande model)
        {

            var client = _context.PPClients.FirstOrDefault(c => c.NoClient == model.NoClient);
            var vendeur = _context.PPVendeurs.FirstOrDefault(v => v.NoVendeur == model.NoVendeur);


            ViewBag.Provinces = _context.Province.ToList();

            ViewBag.TypesLivraison = _context.PPTypesLivraison.ToList();



            var articlesEnPanier = _context.PPArticlesEnPanier.Where(p => p.NoClient == model.NoClient && p.NoVendeur == model.NoVendeur).ToList();

            // CALCUL POIDS
            var poidsTotal = articlesEnPanier
                .Join(_context.PPProduits,
                      article => article.NoProduit,
                      produit => produit.NoProduit,
                      (article, produit) => article.NbItems * produit.Poids)
                .Sum();

            // CALCUL SOUS TOTAL
            var sousTotal = articlesEnPanier
                .Join(_context.PPProduits,
                      article => article.NoProduit,
                      produit => produit.NoProduit,
                      (article, produit) => produit.PrixVente * article.NbItems ?? produit.PrixDemande * article.NbItems)
                .Sum();

            // CALCUL FRAIS LIVRAISON
            model.TypeLivraison = model.TypeLivraison == 0 ? 1 : model.TypeLivraison;
            var codePoids = _context.PPTypesPoids
                .Where(t => t.PoidsMin <= poidsTotal && t.PoidsMax > poidsTotal)
                .Select(t => t.CodePoids).FirstOrDefault();

            if (vendeur.LivraisonGratuite < sousTotal && model.TypeLivraison == 1)
            {
                model.FraisLivraison = 0;
            }
            else
            {
                model.FraisLivraison = _context.PPPoidsLivraisons
                .Where(t => t.CodeLivraison == model.TypeLivraison && t.CodePoids == codePoids)
                .Select(t => t.Tarif)
                .FirstOrDefault();
            }

            // CALCUL DES TAXES
            decimal taxes = 0;

            var tvq = _context.PPTaxeProvinciale
                    .Where(t => t.DateEffectiveTVQ > DateTime.Now)
                    .Select(t => t.TauxTVQ).FirstOrDefault();

            var tps = _context.PPTaxeFederale
                .Where(t => t.DateEffectiveTPS > DateTime.Now)
                .Select(t => t.TauxTPS).FirstOrDefault();

            if (vendeur.Taxes == true)
            {
                if (vendeur.Province == _context.Province.Where(p => p.ProvinceID == "QC").FirstOrDefault())
                {
                    taxes += (tvq / 100);
                }
                taxes += (tps / 100);

                sousTotal = sousTotal* taxes + sousTotal;
            }

            model.FraisLivraison = (model.FraisLivraison * taxes) + model.FraisLivraison;

            if (model.FraisLivraison.HasValue)
            {
                model.total = model.FraisLivraison.Value + sousTotal;
            }
            else
            {
                model.total = sousTotal;
            }

            model.client = client;
            model.vendeur = vendeur;
            model.sousTotal = sousTotal;
            model.poidsTotal = poidsTotal;


            return View(model);
        }
    }
}
