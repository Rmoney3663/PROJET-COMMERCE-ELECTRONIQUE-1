using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Projet_Web_Commerce.Migrations;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using Projet_Web_Commerce.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using Projet_Web_Commerce.Models;
using Projet_Web_Commerce.Data;

namespace Projet_Web_Commerce.Areas.Identity.Pages.Account
{
    public class MdpOublieModel : PageModel
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<Utilisateur> _userManager;
        private readonly IEmailSender _emailSender;

        public MdpOublieModel(AuthDbContext context, UserManager<Utilisateur> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                    ModelState.AddModelError(string.Empty, "Cette adresse courriel n'existe pas dans la base de données.");

                var role = "help";
                var email = "";
                if (User.IsInRole("Gestionnaire"))
                    role = "Gestionnaire";
                else if (User.IsInRole("Vendeur"))
                {
                    role = "Vendeur";
                    var lstVendeurs = _context.PPVendeurs.ToList();
                    email = (from vendeur in lstVendeurs
                                  where vendeur.AdresseEmail == Input.Email
                                  select vendeur).FirstOrDefault().ToString();
                }
                else
                {
                    role = "Client";
                    var lstClients = _context.PPClients.ToList();
                    email = (from client in lstClients
                             where client.AdresseEmail == Input.Email
                             select client).FirstOrDefault().ToString();
                }

                await Methodes.envoyerCourriel(Input.Email, "Oubli du mot de passe",
                        $"Voici le mot de passe pour l'utilisateur {user.UserName}: {email}");
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
