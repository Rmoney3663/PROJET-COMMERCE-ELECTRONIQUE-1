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

        // GET: EmailSenderController
        public ActionResult BoiteDeReception()
        {
            return View();
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
            var listClients = _context.PPClients.ToList();
            if (selectedDestinataire == "Tous")
            {
                selectedDestinataire = string.Empty;
                StringBuilder selectedDestinataireBuilder = new StringBuilder();
                foreach (PPClients client in listClients)
                {
                    var user = _context.Users.Where(v => v.Email == client.AdresseEmail).FirstOrDefault();

                    if (user != null)
                    {
                        selectedDestinataireBuilder.Append(user.Email);

                        if (listClients.IndexOf(client) < listClients.Count - 1)
                        {
                            selectedDestinataireBuilder.Append(",");
                        }
                    }
                }
                selectedDestinataire = selectedDestinataireBuilder.ToString();
            }

            //PPMessages nouveauMessage = new PPMessages();
            //{
            //    nouveauMessage.Sujet = sujet;
            //    nouveauMessage.Message = message;
            //    nouveauMessage.Destinataire = selectedDestinataire;
            //    nouveauMessage.Auteur = auteur;
            //}
            //_context.PPMessages.Add(nouveauMessage);
            //_context.SaveChanges();

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
