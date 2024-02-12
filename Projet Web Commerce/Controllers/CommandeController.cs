using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Projet_Web_Commerce.API;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;
using Projet_Web_Commerce.Models;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

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
        public ActionResult ConfirmerCommande(ModelConfirmerCommande model, bool? payer)
        {

            if (model.client == null)
            {
                var client = _context.PPClients.FirstOrDefault(c => c.NoClient == model.NoClient);
                model.client = client;
                model.PostalClient = client.CodePostal;
                model.VilleClient = client.Ville;
                model.RueClient = client.Rue;
                model.NomClient = client.Nom;
                model.PrenomClient = client.Prenom;
                model.AdresseClient = client.AdresseEmail;
                model.TelClient = client.Tel1;

                model.ProvinceCLient = client.NoProvince;
            }

            

            var vendeur = _context.PPVendeurs.FirstOrDefault(v => v.NoVendeur == model.NoVendeur);
            model.vendeur = vendeur;

            
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

            var listPaniers = from unPanier in _context.PPArticlesEnPanier
                              where unPanier.PPClients.AdresseEmail == User.Identity.Name
                              where unPanier.NoVendeur == vendeur.NoVendeur
                              select unPanier;

            decimal? totalPrice = 0;
            
            var nbTotal = 0;

            foreach (PPArticlesEnPanier article in listPaniers)
            {
                nbTotal += article.NbItems;
                var item = _context.PPProduits.FirstOrDefault(p => p.NoProduit == article.NoProduit);
                if (item.PrixVente != null && item.DateVente > DateTime.Now)
                {
                    totalPrice += item.PrixVente * article.NbItems;
                }
                else
                {
                    totalPrice += item.PrixDemande * article.NbItems;
                }
            }
            var sousTotal = totalPrice;

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
                model.total = model.FraisLivraison.Value + sousTotal.Value;
            }
            else
            {
                model.total = sousTotal.Value;
            }





            model.sousTotal = sousTotal.Value;
            model.poidsTotal = poidsTotal;
            model.client.CodePostal = model.client.CodePostal;


            // If there are no errors, proceed with your logic
            // Your logic here

            List<string> errors = new List<string>();

            // Validate client details
            if (payer != null )
            {

                // Validate Nom
                if (string.IsNullOrEmpty(model.NomClient))
                {
                    errors.Add("Le nom est requis.");
                    ModelState.AddModelError("NomClient", "Le nom est requis.");
                }

                // Validate Prenom
                if (string.IsNullOrEmpty(model.PrenomClient))
                {
                    errors.Add("Le prénom est requis.");
                    ModelState.AddModelError("PrenomClient", "Le prénom est requis.");
                }

                // Validate Email
                if (string.IsNullOrEmpty(model.AdresseClient))
                {
                    errors.Add("L'adresse email est requise.");
                    ModelState.AddModelError("AdresseClient", "L'adresse courriel est requise.");
                }

                // Validate Rue
                if (string.IsNullOrEmpty(model.RueClient))
                {
                    errors.Add("La rue est requise.");
                    ModelState.AddModelError("RueClient", "La rue est requise.");
                }

                // Validate Tel1
                if (string.IsNullOrEmpty(model.TelClient) || !Regex.IsMatch(model.TelClient, "^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]\\d{3}[\\s.-]\\d{4}$"))
                {
                    errors.Add("Le numéro de téléphone est requis.");
                    ModelState.AddModelError("TelClient", "Le téléphone est requis.");
                }

                // Validate NoProvince
                if (string.IsNullOrEmpty(model.ProvinceCLient))
                {
                    errors.Add("La province est requise.");
                    ModelState.AddModelError("ProvinceClient", "La province est requise.");
                }

                // Validate Ville
                if (string.IsNullOrEmpty(model.VilleClient))
                {
                    errors.Add("La ville est requise.");
                    ModelState.AddModelError("VilleClient", "La ville est requise.");
                }

                // Validate CodePostal format
                if (string.IsNullOrEmpty(model.PostalClient) || !Regex.IsMatch(model.PostalClient, "^[A-Za-z]\\d[A-Za-z] \\d[A-Za-z]\\d$"))
                {
                    errors.Add("Le code postal doit être dans le format A1A1A1.");
                    ModelState.AddModelError("PostalClient", "Le code postal doit être dans le format A1A1A1.");
                }

                if (string.IsNullOrEmpty(model.CVV) || !Regex.IsMatch(model.CVV, @"^\d{3,4}$"))
                {
                    errors.Add("Le code postal doit être dans le format A1A1A1.");
                    ModelState.AddModelError("CVV", "Le champ CVV doit contenir 3 ou 4 chiffres.");
                }

                if (string.IsNullOrEmpty(model.dateExpiration) || !Regex.IsMatch(model.dateExpiration, @"^\d{2}-\d{4}$"))
                {
                    errors.Add("Le code postal doit être dans le format A1A1A1.");
                    ModelState.AddModelError("dateExpiration", "La date d'expiration doit être dans le format MM-AAAA.");
                }

                if (string.IsNullOrEmpty(model.NoCarte) || !Regex.IsMatch(model.NoCarte, @"^\d{16}$"))
                {
                    errors.Add("Le code postal doit être dans le format A1A1A1.");
                    ModelState.AddModelError("NoCarte", "Le numéro de carte doit être 16 chiffres.");
                }


            }


            if (errors.Any())
            {
                // If there are errors, return the view with the model and the list of errors
                ViewBag.Errors = errors;
                return View(model);
            }


            if (payer != null)
            {
                if (model.total < 5)
                {
                    return RedirectToAction("Index", "Home");
                }

                //var PostDataCommander = new PostDataCommander
                //{
                //    NoVendeur = model.NoVendeur,
                //    NomVendeur = model.NomVendeur,
                //    NoCarteCredit = 1111123412341234,
                //    DateExpirationCarteCredit = model.dateExpiration,
                //    MontantPaiement = model.total,
                //    NomPageRetour = "google.ca",
                //    InfoSuppl = "Coucou"
                //};

                //var httpClient = new HttpClient();
                //httpClient.BaseAddress = new Uri("http://424w.cgodin.qc.ca/lesi-20XX/");

                //var json = JsonSerializer.Serialize(PostDataCommander);
                //var content = new StringContent(json, Encoding.UTF8, "application/json");

                //var response = httpClient.PostAsync("lesi-effectue-paiement.php", content).Result;

                //if (response.IsSuccessStatusCode)
                //{
                //    var responseContent = response.Content.ReadAsStringAsync().Result;
                //    var postResponse = JsonSerializer.Deserialize<PostResponseCommander>(responseContent);
                //    Console.WriteLine(postResponse.NoAutorisation);
                //}
                //else
                //{
                //    Console.WriteLine("error : " + response.StatusCode);
                //}

                WebRequest req = WebRequest.Create("http://424w.cgodin.qc.ca/lesi-20XX/lesi-effectue-paiement.php");
                string postData = $"?NoVendeur={model.NoVendeur}&NomVendeur={model.NomVendeur}&NoCarteCredit={model.NoCarte}&DateExpirationCarteCredit={model.dateExpiration}&MontantPaiement={model.total}&NomPageRetour={"google.ca"}&InfoSuppl={"test"}";

                byte[] send = Encoding.Default.GetBytes(postData);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = send.Length;

                Stream sout = req.GetRequestStream();
                sout.Write(send, 0, send.Length);
                sout.Flush();
                sout.Close();

                WebResponse res = req.GetResponse();
                StreamReader sr = new StreamReader(res.GetResponseStream());
                string returnvalue = sr.ReadToEnd();

                return View(model);
            }

            return View(model);

        }
    }
}
