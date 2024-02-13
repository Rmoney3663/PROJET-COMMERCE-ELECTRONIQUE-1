using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Cmp;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;
using Projet_Web_Commerce.Models;
using System.Text;

namespace Projet_Web_Commerce.Controllers
{
    [Authorize(Roles = "Gestionnaire, Vendeur, Client")]
    public class MessagerieController : Controller
    {
        public readonly AuthDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<Utilisateur> _userManager;

        public MessagerieController(AuthDbContext context, Microsoft.AspNetCore.Identity.UserManager<Utilisateur> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: MessagerieController
        public ActionResult BoiteDeReception()
        {
            var currentUserEmail = _userManager.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefault().Id;
            var msgRecus = _context.PPDestinatairesMessage
                .Where(dest => dest.Destinataire == currentUserEmail)
                .Select(dest => dest.Message)
                .ToList();

            return View(msgRecus);
        }

        [HttpPost]
        public async Task<ActionResult> BoiteDeReception(string sujet, string message, string selectedDestinataire, string auteur)
        {
            
            return View();
        }

        [HttpGet]
        public ActionResult EnvoyerMessage()
        {

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> EnvoyerMessage(string sujet, string message, string selectedDestinataire, string auteur)
        {
            PPMessages nouveauMessage = new PPMessages();
            {
                nouveauMessage.Sujet = sujet;
                nouveauMessage.Message = message;
                nouveauMessage.Auteur = auteur;
                nouveauMessage.AuteurUser = await _userManager.FindByEmailAsync(auteur);
                nouveauMessage.TypeMessage = 0;
                nouveauMessage.PieceJointe = "";
                nouveauMessage.Transfemetteur = "";
            }
            _context.PPMessages.Add(nouveauMessage);
            await _context.SaveChangesAsync();

            var listClients = _context.PPClients.ToList();
            var destinatairesList = new List<PPDestinatairesMessage>();
            if (selectedDestinataire == "Tous" || selectedDestinataire == "william.anthony.burgess@gmail.com")
            {
                // Ajouter gestionnaire
                var user = _context.Users.Where(v => v.Email == "william.anthony.burgess@gmail.com").FirstOrDefault();
                if (user != null)
                {
                    PPDestinatairesMessage destinatairesMessage = new PPDestinatairesMessage();
                    {
                        destinatairesMessage.NoMessage = nouveauMessage.NoMessage;
                        destinatairesMessage.Destinataire = user.Email;
                        destinatairesMessage.DestinataireUser = await _userManager.FindByEmailAsync(user.Email);
                    }
                    destinatairesList.Add(destinatairesMessage);
                }

                if (selectedDestinataire == "Tous")
                {
                    foreach (PPClients client in listClients)
                    {
                        user = _context.Users.Where(v => v.Email == client.AdresseEmail).FirstOrDefault();

                        if (user != null)
                        {
                            PPDestinatairesMessage destinatairesMessage = new PPDestinatairesMessage();
                            {
                                destinatairesMessage.NoMessage = nouveauMessage.NoMessage;
                                destinatairesMessage.Destinataire = user.Email;
                                destinatairesMessage.DestinataireUser = await _userManager.FindByEmailAsync(user.Email);
                            }
                            destinatairesList.Add(destinatairesMessage);
                        }
                    }
                }
            }
            else
            {
                string[] selectedDestinataireArray = selectedDestinataire.Split(',');

                foreach (string email in selectedDestinataireArray)
                {
                    var user = _context.Users.Where(v => v.Email == email).FirstOrDefault();

                    if (user != null)
                    {
                        PPDestinatairesMessage destinatairesMessage = new PPDestinatairesMessage
                        {
                            NoMessage = nouveauMessage.NoMessage,
                            Destinataire = user.Email,
                            DestinataireUser = await _userManager.FindByEmailAsync(user.Email)
                        };

                        destinatairesList.Add(destinatairesMessage);
                    }
                }
            }

            _context.PPDestinatairesMessage.AddRange(destinatairesList);
            await _context.SaveChangesAsync();

            return View();
        }

        // GET: EmailSenderController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EmailSenderController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmailSenderController/Create
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

        // GET: EmailSenderController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EmailSenderController/Edit/5
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

        // GET: EmailSenderController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EmailSenderController/Delete/5
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
