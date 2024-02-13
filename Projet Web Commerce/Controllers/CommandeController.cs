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
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        

        public ActionResult CommandeCompleter(string NoAutorisation, string DateAutorisation, string FraisMarchand, string InfoSuppl)
        {
            ViewBag.NoAutorisation = NoAutorisation;
            ViewBag.DateAutorisation = DateAutorisation;
            ViewBag.FraisMarchand = FraisMarchand;
            ViewBag.InfoSuppl = InfoSuppl;
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> ConfirmerCommandeAsync(ModelConfirmerCommande model, bool? payer, int? NoAutorisation, string? DateAutorisation, string? FraisMarchand, string? InfoSuppl)
        {

            if (TempData.ContainsKey("NoVendeur"))
            {
                // Retrieve the value from TempData and convert it to a string
                string novendeur = TempData.Peek("NoVendeur").ToString();
                model.NoVendeur = Convert.ToInt32(novendeur);
            }

            if (TempData.ContainsKey("NoClient"))
            {
                // Retrieve the value from TempData and convert it to a string
                string noclient = TempData.Peek("NoClient").ToString();
                model.NoClient = Convert.ToInt32(noclient);
            }

            string nomClient = TempData.ContainsKey("NomClient") ? TempData.Peek("NomClient") as string : null;
            string prenomClient = TempData.ContainsKey("PrenomClient") ? TempData.Peek("PrenomClient") as string : null;
            string adresseClient = TempData.ContainsKey("AdresseClient") ? TempData.Peek("AdresseClient") as string : null;
            string telClient = TempData.ContainsKey("TelClient") ? TempData.Peek("TelClient") as string : null;
            string rueClient = TempData.ContainsKey("RueClient") ? TempData.Peek("RueClient") as string : null;
            string villeClient = TempData.ContainsKey("VilleClient") ? TempData.Peek("VilleClient") as string : null;
            string postalClient = TempData.ContainsKey("PostalClient") ? TempData.Peek("PostalClient") as string : null;
            string provClient = TempData.ContainsKey("provClient") ? TempData.Peek("provClient") as string : null;
            string fraisLivraison = TempData.ContainsKey("fraisLivraison") ? TempData.Peek("fraisLivraison") as string : null;
            


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


            if (NoAutorisation != null)
            {


                switch (NoAutorisation)
                {

                    case 0:
                        TempData["ErrorMessage"] = "Transaction annulée par l'utilisateur ";
                        break;
                    case 1:
                        TempData["ErrorMessage"] = "Transaction refusée: Date expiration dépassée ";
                        break;
                    case 2:
                        TempData["ErrorMessage"] = "Transaction refusée: Limite de crédit atteinte";
                        break;
                    case 3:
                        TempData["ErrorMessage"] = "Contactez notre bureau (514-626-2666) ";
                        break;
                    case 1234:
                        TempData["ErrorMessage"] = "ERREUR";
                        break;
                    case 9999:
                        TempData["ErrorMessage"] = "ERREUR DE VALIDATION ";
                        break;
                    default:
                        bool dispo = true;

                        foreach (PPArticlesEnPanier article in listPaniers)
                        {
                            var produit = _context.PPProduits.Where(p => p.NoProduit == article.NoProduit).FirstOrDefault();
                            if (article.NbItems > produit.NombreItems)
                            {
                                dispo = false;
                            }   
                        }
                        if (!dispo) {
                            TempData["ErrorMessage"] = "Items pas en stock ";
                            break; }


                            var client = _context.PPClients.FirstOrDefault(c => c.NoClient == model.NoClient);
                        if (client != null)
                        {
                            client.Nom = nomClient;
                            client.Prenom = prenomClient;
                            client.AdresseEmail = adresseClient;
                            client.Tel1 = telClient;
                            client.Rue = rueClient;
                            client.Ville = villeClient;
                            client.CodePostal = postalClient;
                            client.NoProvince = provClient;

                            _context.SaveChanges();
                        }
                        decimal fraisLivraisonDecimal = Convert.ToDecimal(fraisLivraison);
                        decimal roundedFraisLivraison = Math.Round(fraisLivraisonDecimal, 2);

                        var ppCommande = new PPCommandes
                        {
                            NoClient = model.NoClient, // Assuming model.NoClient contains the NoClient value
                            NoVendeur = model.NoVendeur, // Assuming model.NoVendeur contains the NoVendeur value
                            DateCommande = DateTime.Now, // Set the current date and time for DateCommande
                            PoidsTotal = poidsTotal, // Assuming model.PoidsTotal contains the PoidsTotal value
                            Statut = "s", // Set the initial status, e.g., "N" for New
                            MontantTotAvantTaxes = Math.Round(sousTotal.Value,2), // Assuming model.total contains the total amount before taxes
                            TPS = Math.Round(sousTotal.Value * (tps / 100),2), // Assuming TPS is 5% of the total amount
                            TVQ = Math.Round(sousTotal.Value * (tvq / 100),2), // Assuming TVQ is 9.975% of the total amount
                            CoutLivraison = roundedFraisLivraison, // Assuming model.FraisLivraison contains the delivery cost, handle null case
                            TypeLivraison = model.TypeLivraison,
                            NoAutorisation = NoAutorisation.ToString()
                        };

                        //// Add the newly created PPCommandes object to the context and save changes
                        _context.PPCommandes.Add(ppCommande);
                        _context.SaveChanges();

                        foreach (PPArticlesEnPanier article in listPaniers)
                        {
                            var produit = _context.PPProduits.Where(p => p.NoProduit == article.NoProduit).FirstOrDefault();

                            decimal prix = 0;
                            if (produit.PrixVente != null && produit.DateVente > DateTime.Now)
                            {
                                prix = produit.PrixVente.Value;
                            }
                            else
                                prix = produit.PrixDemande;

                            var ppDetailsCommande = new PPDetailsCommandes
                            {
                                NoCommande = ppCommande.NoCommande,
                                NoProduit = article.NoProduit,
                                Quantité = article.NbItems,
                                PrixVente = prix
                            };

                            _context.Add(ppDetailsCommande);
                        }

                        _context.SaveChanges();

                        TempData.Clear();

                        return RedirectToAction("CommandeCompleter", new { NoAutorisation = NoAutorisation, DateAutorisation = DateAutorisation, FraisMarchand = FraisMarchand, InfoSuppl = InfoSuppl });
                }

            }



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

                string formPageUrl = "http://424w.informatique.cgodin.qc.ca/lesi-20XX/lesi-effectue-paiement.php";

                // Create an instance of HttpClient

                var handler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };

                using var httpClient = new HttpClient(handler);

                string prixtotString = model.total.ToString("0.00", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                // Prepare form data
                Dictionary<string, string> formDataDictionary = new Dictionary<string, string>
                {
                    { "NoVendeur", $"{model.NoVendeur}" },
                    { "NomVendeur", $"{model.NomVendeur}" },
                    { "NoCarteCredit", $"{model.NoCarte}"},
                    { "DateExpirationCarteCredit", $"{model.dateExpiration}" },
                    { "MontantPaiement",prixtotString },
                    { "NoSecuriteCarteCredit", $"{model.CVV}" },
                    { "NomPageRetour", $"https://localhost:44376/Commande/ConfirmerCommande"},
                    { "InfoSuppl", "Coucou" }
                    // Add other form fields as needed
                };

                var formData = new MultipartFormDataContent();
                foreach (var kvp in formDataDictionary)
                {
                    formData.Add(new StringContent(kvp.Value), kvp.Key);
                }

                // Send the POST request to submit the form
                HttpResponseMessage response = await httpClient.PostAsync(formPageUrl, formData);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the HTML content of the response
                    string htmlContent = await response.Content.ReadAsStringAsync();

                    string scriptToInsert = @"<style>body { display: none; }</style>
                    <script>
                        // Find the form element by tag name (assuming it's the first form in the document)
                        var form = document.getElementById('frmTransmission');
                        form.submit();

                    </script>";



                    // Insert the script directly into the HTML content
                    htmlContent += scriptToInsert;

                    TempData["NoVendeur"] = model.NoVendeur;
                    TempData["NoClient"] = model.NoClient;

                    TempData["NomClient"] = model.NomClient;
                    TempData["PrenomClient"] = model.PrenomClient;
                    TempData["AdresseClient"] = model.AdresseClient;
                    TempData["TelClient"] = model.TelClient;
                    TempData["RueClient"] = model.RueClient;
                    TempData["VilleClient"] = model.VilleClient;
                    TempData["PostalClient"] = model.PostalClient;
                    TempData["provClient"] = model.ProvinceCLient;

                    string tempFrais = model.FraisLivraison.ToString();
                    TempData["fraisLivraison"] = tempFrais;

                    TempData.Keep();

                    return Content(htmlContent, "text/html");
                }



                    return View(model);
            }

            return View(model);

        }
    }
}
