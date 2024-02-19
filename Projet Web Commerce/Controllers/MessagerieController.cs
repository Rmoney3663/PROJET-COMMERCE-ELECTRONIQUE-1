using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
        private readonly IHubContext<Notifications> _notificationsHubContext;

        public MessagerieController(AuthDbContext context, Microsoft.AspNetCore.Identity.UserManager<Utilisateur> userManager, IHubContext<Notifications> notificationsHubContext)
        {
            _context = context;
            _userManager = userManager;
            _notificationsHubContext = notificationsHubContext;
        }

        [HttpGet]
        // GET: MessagerieController
        public ActionResult BoiteDeReception()
        {
            var utilisateurCourantId = _userManager.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefault().Id;
            ViewBag.UtilisateurCourantId = utilisateurCourantId;

            var msgRecus = _context.PPDestinatairesMessage
                .Include(dest => dest.Message.Destinataires)
                .Where(dest => dest.Destinataire == utilisateurCourantId)
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
        public async Task<ActionResult> EnvoyerMessage(int? idMessage)
        {
            if (idMessage == null)
            {
                return View();
            }

            var msg = await _context.PPMessages
                .Include(m => m.Destinataires)
                .FirstOrDefaultAsync(m => m.NoMessage == idMessage);

            var utilisateurCourantId = _userManager.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefault().Id;
            
            if (utilisateurCourantId == msg.Auteur) // Si brouillon
            {
                List<string> lstCourrielsDests = new List<string>();
                foreach (var dest in msg.Destinataires)
                {
                    var courrielDest = _userManager.Users.Where(u => u.Id == dest.Destinataire).FirstOrDefault().Email;
                    lstCourrielsDests.Add(courrielDest);
                }
                ViewBag.Dests = lstCourrielsDests;
            }
            else // Si réponse à courriel
            {
                var courrielAuteur = _userManager.Users.Where(u => u.Id == msg.Auteur).FirstOrDefault().Email;
                List<string> lstCourrielDest = new List<string>();
                lstCourrielDest.Add(courrielAuteur);
                ViewBag.Dests = lstCourrielDest;
                msg.Sujet = "Re: " + msg.Sujet;
                msg.Message = "\n\n\n\n---------------------------------------------------------------------------------\n" + courrielAuteur + ", " + msg.DateEnvoi + "\n\n" + msg.Message;
            }

            return View(msg);
        }

        [HttpPost]
        public async Task<IActionResult> EnvoyerMessage(string sujet, string message, string selectedDestinataire
            , string auteur, int typeMessage, int? idMessage)
        {
            int noMsg = 0;
            var utilisateurCourantId = _userManager.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefault().Id;

            if (utilisateurCourantId != auteur) // Si réponse à courriel
            {
                idMessage = null;
            }
            
            if (idMessage != null)
            {
                var msg = await _context.PPMessages
                    .FirstOrDefaultAsync(m => m.NoMessage == idMessage);
                msg.Sujet = sujet == null ? "" : sujet;
                msg.Message = message == null ? "" : message;
                msg.TypeMessage = typeMessage;
                noMsg = Convert.ToInt32(idMessage);

                foreach (var dest in _context.PPDestinatairesMessage.ToList())
                {
                    if (dest.NoMessage == noMsg)
                        _context.PPDestinatairesMessage.Remove(dest);
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                PPMessages nouveauMessage = new PPMessages();
                {
                    nouveauMessage.Sujet = sujet == null ? "" : sujet;
                    nouveauMessage.Message = message == null ? "" : message;
                    nouveauMessage.Auteur = auteur;
                    nouveauMessage.AuteurUser = await _userManager.FindByEmailAsync(auteur);
                    nouveauMessage.TypeMessage = typeMessage;
                    nouveauMessage.PieceJointe = "";
                    nouveauMessage.Transmetteur = null;
                    nouveauMessage.DateEnvoi = DateTime.Now;
                }
                _context.PPMessages.Add(nouveauMessage);
                await _context.SaveChangesAsync();
                noMsg = nouveauMessage.NoMessage;
            }
            

            var listClients = _context.PPClients.ToList();
            var destinatairesList = new List<PPDestinatairesMessage>();

            if (selectedDestinataire != null)
            {
                string[] selectedDestinataireArray = selectedDestinataire.Split(',');

                foreach (string email in selectedDestinataireArray)
                {
                    var user = _context.Users.Where(v => v.Email == email).FirstOrDefault();

                    if (user != null)
                    {
                        PPDestinatairesMessage destinatairesMessage = new PPDestinatairesMessage
                        {
                            NoMessage = noMsg,
                            Destinataire = user.Email,
                            DestinataireUser = await _userManager.FindByEmailAsync(user.Email)
                        };

                        destinatairesList.Add(destinatairesMessage);
                    }
                }
                _context.PPDestinatairesMessage.AddRange(destinatairesList);
            }
            
            await _context.SaveChangesAsync();

            foreach (PPDestinatairesMessage email in destinatairesList)
            {
                await _notificationsHubContext.Clients.All.SendAsync("NotificationMessage", email.Destinataire);
            }
                if (typeMessage == 2)
                TempData["MsgStatut"] = "Votre brouillon a été sauvegardé!";
            else if (typeMessage == 0)
                TempData["MsgStatut"] = "Votre message a été envoyé!";
            return View();
        }

        [HttpGet]
        public ActionResult Envoyes()
        {
            var currentUserEmail = _userManager.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefault().Id;
            var msgEnvoyes = _context.PPMessages
                .Where(message => message.Auteur == currentUserEmail 
                && message.TypeMessage == 0)
                .Include(m => m.Destinataires)
                .ToList();

            return View(msgEnvoyes);
        }

        [HttpGet]
        // GET: EmailSenderController/Details/5
        public async Task<ActionResult> Details(int idMessage)
        {
            var user = await _userManager.GetUserAsync(User);
            var utilisateurCourantId = await _userManager.GetUserIdAsync(user);

            var messageCourant = await _context.PPMessages
                .Include(m => m.Destinataires)
                .FirstOrDefaultAsync(m => m.NoMessage == idMessage);

            // Vérifier si l'utilisateur courant est le destinataire du message
            var destinataire = messageCourant.Destinataires.FirstOrDefault(dest => dest.Destinataire == utilisateurCourantId);

            if (destinataire != null && !destinataire.MessageLu)
            {
                destinataire.MessageLu = true;
                await _context.SaveChangesAsync();
            }

            return View(messageCourant);
        }

        [HttpGet]
        // GET: EmailSenderController/Brouillons
        public ActionResult Brouillons()
        {
            var currentUserEmail = _userManager.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefault().Id;
            var brouillons = _context.PPMessages
                .Where(message => message.Auteur == currentUserEmail
                    && message.TypeMessage == 2)
                .Include(m => m.Destinataires)
                .ToList();
            return View(brouillons);
        }

        [HttpGet]
        public ActionResult Supprimes()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Supprimer()
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
