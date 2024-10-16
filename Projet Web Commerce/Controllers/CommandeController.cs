﻿using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Projet_Web_Commerce.API;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;
using Projet_Web_Commerce.Models;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using iTextSharp.text.pdf.draw;

namespace Projet_Web_Commerce.Controllers
{
    [Authorize(Roles = "Client")]
    public class CommandeController : Controller
    {

        private readonly AuthDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<Utilisateur> _userManager;
        private readonly IHubContext<Notifications> _notificationsHubContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CommandeController(IWebHostEnvironment webHostEnvironment, AuthDbContext context, Microsoft.AspNetCore.Identity.UserManager<Utilisateur> userManager, IHubContext<Notifications> notificationsHubContext)
        {
            _context = context;
            _userManager = userManager;
            _notificationsHubContext = notificationsHubContext;
            _webHostEnvironment = webHostEnvironment;
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
        

        public ActionResult CommandeCompleter(string NoAutorisation, string DateAutorisation, string FraisMarchand, string InfoSuppl, string NoCommande)
        {
            ViewBag.NoAutorisation = NoAutorisation;
            ViewBag.DateAutorisation = DateAutorisation;
            ViewBag.FraisMarchand = FraisMarchand;
            ViewBag.InfoSuppl = InfoSuppl;
            ViewBag.NoCommande = NoCommande;
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
    .Join(_context.PPProduits.Where(produit => produit.Disponibilite), // Filter only available products
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
                var item = _context.PPProduits.FirstOrDefault(p => p.NoProduit == article.NoProduit);
                if (item.Disponibilite == true)
                {
                    nbTotal += article.NbItems;

                    if (item.PrixVente != null && item.DateVente > DateTime.Now)
                    {
                        totalPrice += item.PrixVente * article.NbItems;
                    }
                    else
                    {
                        totalPrice += item.PrixDemande * article.NbItems;
                    }
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

            var test = _context.PPTaxeFederale;

            var tvq = _context.PPTaxeProvinciale
                            .Where(t => DateTime.Now > t.DateEffectiveTVQ)
                            .OrderByDescending(t => t.DateEffectiveTVQ) // specify the key to order by
                            .Select(t => t.TauxTVQ)
                            .FirstOrDefault();

            var tps = _context.PPTaxeFederale
                            .Where(t => DateTime.Now > t.DateEffectiveTPS)
                            .OrderByDescending(t => t.DateEffectiveTPS) // specify the key to order by
                            .Select(t => t.TauxTPS)
                            .FirstOrDefault();



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

                var messageErreur = "";
                switch (NoAutorisation)
                {
                    
                    case 0:
                        messageErreur = "Transaction annulée par l'utilisateur \n";
                        break;
                    case 1:
                        messageErreur = "Transaction refusée: Date expiration dépassée \n";
                        break;
                    case 2:
                        messageErreur = "Transaction refusée: Limite de crédit atteinte\n";
                        break;
                    case 3:
                        messageErreur = "Contactez notre bureau (514-626-2666) \n";
                        break;
                    case 1234:
                        messageErreur = "ERREUR\n";
                        break;
                    case 9999:
                        messageErreur = "ERREUR DE VALIDATION \n";
                        break;
                    default:

                        using (var transaction = _context.Database.BeginTransaction())
                        {
                            try
                            {
                                var client = _context.PPClients.FirstOrDefault(c => c.NoClient == model.NoClient);
                                if (postalClient.Length == 6)
                                {
                                    postalClient = postalClient.Insert(3, " ");
                                }
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
                                    NoClient = model.NoClient,
                                    NoVendeur = model.NoVendeur,
                                    DateCommande = DateTime.Now,
                                    PoidsTotal = poidsTotal.Value,
                                    Statut = "P",
                                    MontantTotAvantTaxes = Math.Round(sousTotal.Value, 2),
                                    TPS = Math.Round(sousTotal.Value * (tps / 100), 2),
                                    TVQ = Math.Round(sousTotal.Value * (tvq / 100), 2),
                                    CoutLivraison = roundedFraisLivraison,
                                    TypeLivraison = model.TypeLivraison,
                                    NoAutorisation = NoAutorisation.ToString()
                                };

                                _context.PPCommandes.Add(ppCommande);
                                _context.SaveChanges();

                                foreach (PPArticlesEnPanier article in listPaniers)
                                {
                                    var produit = _context.PPProduits.FirstOrDefault(p => p.NoProduit == article.NoProduit);

                                    if (produit == null || produit.Disponibilite == false)
                                    {
                                        // Handle case where product is not found
                                        continue;
                                    }

                                    decimal prix = produit.PrixVente ?? produit.PrixDemande;

                                    var ppDetailsCommande = new PPDetailsCommandes
                                    {
                                        NoCommande = ppCommande.NoCommande,
                                        NoProduit = article.NoProduit,
                                        Quantité = article.NbItems,
                                        PrixVente = prix
                                    };

                                    _context.Add(ppDetailsCommande);
                                }

                                bool dispo = true;

                                foreach (PPArticlesEnPanier article in listPaniers)
                                {
                                    var produit = _context.PPProduits.FirstOrDefault(p => p.NoProduit == article.NoProduit);

                                    if (produit == null || produit.Disponibilite == false)
                                    {
                                        // Handle case where product is not found
                                        continue;
                                    }

                                    int newQuantity = produit.NombreItems.Value - article.NbItems;

                                    if (newQuantity < 0)
                                    {
                                        messageErreur += $"Le produit {produit.Nom} excède le nombre d'items en stock.\n";
                                        dispo = false; // Set dispo flag to false if any item is out of stock
                                    }

                                    produit.NombreItems = newQuantity;
                                }

                                if (dispo)
                                {
                                    var paniers = _context.PPArticlesEnPanier.Where(p => p.NoVendeur == model.NoVendeur && p.NoClient == model.NoClient).ToList();

                                    foreach(PPArticlesEnPanier panier in paniers)
                                    {
                                        _context.Remove(panier);
                                    }

                                    decimal fraisLesi;
                                    if (decimal.TryParse(FraisMarchand,
                                         NumberStyles.Number,
                                         CultureInfo.InvariantCulture,
                                         out fraisLesi))
                                    {

                                        var RedevancePourcentage = _context.PPVendeurs.Where(v => v.NoVendeur == model.NoVendeur).FirstOrDefault().Pourcentage;
                                        decimal? Redevance = (sousTotal.Value/100)* RedevancePourcentage;


                                        var ppHistoriqueCommande = new PPHistoriquePaiements
                                        {
                                            MontantVenteAvantLivraison = Math.Round(sousTotal.Value, 2),
                                            NoVendeur = model.NoVendeur,
                                            NoClient = model.NoClient,
                                            NoCommande = ppCommande.NoCommande,
                                            DateVente = ppCommande.DateCommande,
                                            NoAutorisation = NoAutorisation.ToString(),
                                            FraisLesi = fraisLesi,
                                            Redevance = Math.Round(Redevance.Value, 2),
                                            FraisLivraison = roundedFraisLivraison,
                                            FraisTPS = ppCommande.TPS,
                                            FraisTVQ = ppCommande.TVQ
                                        };
                                        _context.Add(ppHistoriqueCommande);
                                    }
                                    else
                                    {
                                        throw new Exception();
                                    }

                                    var TypeLivre = _context.PPTypesLivraison.Where(v => v.CodeLivraison == ppCommande.TypeLivraison).FirstOrDefault().Description;
                                    string prixtot = null;
                                    string prixtaxe = null;
                                    if (vendeur.Taxes == true)
                                    {
                                        prixtaxe = ((ppCommande.MontantTotAvantTaxes / 100) * vendeur.PourcentageTaxe)?.ToString("F2");
                                        prixtot = ((ppCommande.MontantTotAvantTaxes / 100) * vendeur.PourcentageTaxe + ppCommande.CoutLivraison + ppCommande.MontantTotAvantTaxes)?.ToString("F2");
                                    }
                                    else
                                    {
                                        prixtaxe = "0.00";
                                        prixtot = (ppCommande.MontantTotAvantTaxes + ppCommande.CoutLivraison ).ToString("F2");
                                    }
                                    

                                    if (ppCommande.NoCommande != null)
                                    {
                                        using (MemoryStream memoryStream = new MemoryStream())
                                        {
                                            Document document = new Document();
                                            PdfWriter.GetInstance(document, memoryStream);
                                            document.Open();

                                            Paragraph title = new Paragraph("Reçu", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, BaseColor.BLACK));
                                            title.Alignment = Element.ALIGN_CENTER;
                                            document.Add(title);

                                            Paragraph orderInfo = new Paragraph();
                                            orderInfo.SpacingBefore = 20f; 
                                            orderInfo.Alignment = Element.ALIGN_LEFT;
                                            orderInfo.Font = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                                            orderInfo.Add($"Date de la commande: {ppCommande.DateCommande}\n");
                                            orderInfo.Add($"Nom du vendeur: {vendeur.Prenom} {vendeur.Nom}\n");
                                            orderInfo.Add($"Numéro de client: {client.NoClient}\n");
                                            orderInfo.Add($"Numéro d'autorisation: {ppCommande.NoAutorisation}\n");

                                            LineSeparator line = new LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_CENTER, -1);
                                            orderInfo.Add(new Chunk(line));

                                            Chunk totalChunk = new Chunk($"\n\nTotal: ".PadRight(25) + $"{prixtot} $\n", FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                                            orderInfo.Add(totalChunk);
                                            orderInfo.Add($"Montant: ".PadRight(30) + $"{ppCommande.MontantTotAvantTaxes} $\n");
                                            orderInfo.Add($"Taxe: ".PadRight(32) + $"{prixtaxe} $\n");
                                            orderInfo.Add($"Coût de livraison: ".PadRight(26) + $"{ppCommande.CoutLivraison} $\n");
                                            orderInfo.Add($"Type de livraison: ".PadRight(26) + $"{TypeLivre}\n");
                                            orderInfo.Add($"Poids total: ".PadRight(30) + $"{ppCommande.PoidsTotal} kg\n");

                                            orderInfo.Add(new Chunk(line));

                                            orderInfo.Add($"\n\nDestination: {client.Rue}, {client.Ville}, {client.NoProvince}, {client.CodePostal}\n");

                                            document.Add(orderInfo);

                                            document.Close();


                                            byte[] pdfBytes = memoryStream.ToArray();

                                            string fileName = $"{ppCommande.NoCommande}.pdf";                                            
                                            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "data", "pdf", fileName);                                           
                                            System.IO.File.WriteAllBytes(filePath, pdfBytes);
                                            var Auteur = _context.PPGestionnaire.FirstOrDefault().IdUtilisateur;
                                            var ppMessage = new PPMessages
                                            {
                                               Sujet = "Confirmation de commande",
                                               Message = "Voici le pdf qui contient l'information de la commande.",
                                               Auteur = Auteur,
                                               TypeMessage = 0,
                                               PieceJointe = null,
                                               Transmetteur = null,
                                                DateEnvoi = DateTime.Now
                                            };

                                            _context.PPMessages.Add(ppMessage);
                                            await _context.SaveChangesAsync();
                                            string fileName2 = $"{ppMessage.NoMessage}.pdf";
                                            string filePath2 = Path.Combine(_webHostEnvironment.WebRootPath, "data", "piecesJointes", fileName2);
                                            System.IO.File.WriteAllBytes(filePath2, pdfBytes);
                                            ppMessage.PieceJointe = fileName2;
                                            await _context.SaveChangesAsync();

                                            var vendeurmessage = _context.PPVendeurs.Where(v => v.NoVendeur == ppCommande.NoVendeur).FirstOrDefault().AdresseEmail;
                                            var userV = _context.Users.Where(v => v.Email == vendeurmessage).FirstOrDefault();
                                            PPDestinatairesMessage destinatairesMessage1 = new PPDestinatairesMessage
                                            {
                                                NoMessage = ppMessage.NoMessage,
                                                Destinataire = userV.Email,
                                                DestinataireUser = await _userManager.FindByEmailAsync(userV.Email)
                                            };

                                            var clientmessage = _context.PPClients.Where(v => v.NoClient == ppCommande.NoClient).FirstOrDefault().AdresseEmail;
                                            var userC = _context.Users.Where(v => v.Email == clientmessage).FirstOrDefault();
                                            PPDestinatairesMessage destinatairesMessage2 = new PPDestinatairesMessage
                                            {
                                                NoMessage = ppMessage.NoMessage,
                                                Destinataire = userC.Email,
                                                DestinataireUser = await _userManager.FindByEmailAsync(userC.Email)
                                            };

                                            _context.PPDestinatairesMessage.Add(destinatairesMessage1);
                                            _context.PPDestinatairesMessage.Add(destinatairesMessage2);
                                            await _context.SaveChangesAsync();

                                            await _notificationsHubContext.Clients.User(destinatairesMessage1.Destinataire).SendAsync("NotificationMessage", destinatairesMessage1.Destinataire);
                                            await _notificationsHubContext.Clients.User(destinatairesMessage2.Destinataire).SendAsync("NotificationMessage", destinatairesMessage2.Destinataire);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception();
                                    }

                                   


                                    _context.SaveChanges();
                                    transaction.Commit(); // Commit the transaction if all operations succeed
                                    TempData.Clear();
                                    return RedirectToAction("CommandeCompleter", new { NoAutorisation = NoAutorisation, DateAutorisation = DateAutorisation, FraisMarchand = FraisMarchand, InfoSuppl = InfoSuppl, NoCommande = ppCommande.NoCommande });
                                }
                                else
                                {

                                    throw new Exception();
                                }
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback(); // Rollback the transaction if an exception occurs
                                TempData.Clear();

                                messageErreur = messageErreur == "" ? "Une erreur s'est produite !" : messageErreur;
                                TempData["ErrorMessage"] = messageErreur;
                            }
                        }

                        break;

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
            model.poidsTotal = poidsTotal.Value;
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

                if (string.IsNullOrEmpty(model.TelClient))
                {
                    errors.Add("Le numéro de téléphone est requis.");
                    ModelState.AddModelError("TelClient", "Le numéro de téléphone est requis.");
                }
                else if (!Regex.IsMatch(model.TelClient, "^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]\\d{3}[\\s.-]\\d{4}$"))
                {
                    errors.Add("Le numéro de téléphone est requis.");
                    ModelState.AddModelError("TelClient", "Le numéro de téléphone doit être dans le format 999-999-9999.");
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
                if (string.IsNullOrEmpty(model.PostalClient))
                {
                    errors.Add("Le code postal doit être dans le format A1A1A1 ou A1A 1A1.");
                    ModelState.AddModelError("PostalClient", "Le code postal est requis.");
                }
                else if (!Regex.IsMatch(model.PostalClient, "^([A-Za-z]\\d[A-Za-z] \\d[A-Za-z]\\d)|([A-Za-z]\\d[A-Za-z]\\d[A-Za-z]\\d)$"))
                {
                    errors.Add("Le code postal doit être dans le format A1A1A1 ou A1A 1A1.");
                    ModelState.AddModelError("PostalClient", "Le code postal doit être dans le format A1A1A1 ou A1A 1A1.");
                }

                if (string.IsNullOrEmpty(model.CVV) || !Regex.IsMatch(model.CVV, @"^\d{3,4}$"))
                {
                    errors.Add("Le code postal doit être dans le format A1A1A1.");
                    ModelState.AddModelError("CVV", "Le champ CVV doit contenir 3 ou 4 chiffres.");
                }
                DateTime currentDateTime = DateTime.Now;
                if (string.IsNullOrEmpty(model.dateExpiration) || !Regex.IsMatch(model.dateExpiration, @"^(0[1-9]|1[0-2])-\d{4}$"))
                {
                    errors.Add("Le code postal doit être dans le format A1A1A1.");
                    ModelState.AddModelError("dateExpiration", "La date d'expiration doit être dans le format MM-AAAA.");
                }


                //else if (currentDateTime.Year.ToString() != model.dateExpiration.Substring(model.dateExpiration.Length - 4))
                //{
                //    errors.Add("Le code postal doit être dans le format A1A1A1.");
                //    ModelState.AddModelError("dateExpiration", "La date d'expiration doit être avant l'an 2025");
                //}
                //else if ((int.Parse(model.dateExpiration.Substring(0, 2)) <= currentDateTime.Month))
                //{
                //    errors.Add("Le code postal doit être dans le format A1A1A1.");
                //    ModelState.AddModelError("dateExpiration", "La date d'expiration doit être après la date d'aujourd'hui");
                //}

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

                //string formPageUrl = "http://424w.informatique.cgodin.qc.ca/lesi-20XX/lesi-effectue-paiement.php";
                //string formPageUrl = "https://424w.cgodin.qc.ca/lesi-20XX/lesi-effectue-paiement.php";
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

                try
                {
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
                    else
                    {
                        TempData["ErrorMessage"] = "Une erreur inattendue s'est produite.";
                    }
                }
                catch (Exception e)
                {
                    TempData["ErrorMessage"] = "Une erreur inattendue s'est produite.";
                }



                return View(model);
            }

            return View(model);

        }
    }
}
