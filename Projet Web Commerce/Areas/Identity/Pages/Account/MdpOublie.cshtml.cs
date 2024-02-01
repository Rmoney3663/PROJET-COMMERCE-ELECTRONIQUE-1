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

        public MdpOublieModel(AuthDbContext context, UserManager<Utilisateur> userManager)
        {
            _context = context;
            _userManager = userManager;
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

                var password = "";

                bool estDansRole = await _userManager.IsInRoleAsync(user, "Vendeur");
                if (estDansRole)
                {
                    var lstVendeurs = _context.PPVendeurs.ToList();
                    var foundVendeur = lstVendeurs.FirstOrDefault(v => v.AdresseEmail == Input.Email);

                    password = foundVendeur?.MotDePasse.ToString() ?? "";
                }
                else
                {
                    estDansRole = await _userManager.IsInRoleAsync(user, "Client");

                    var lstClients = _context.PPClients.ToList();
                    var foundClient = lstClients.FirstOrDefault(client => client.AdresseEmail == Input.Email);

                    password = foundClient?.MotDePasse.ToString() ?? "";
                }

                if (password != "")
                {
                    await Methodes.envoyerCourriel(Input.Email, "Oubli du mot de passe",
                        $"Voici le mot de passe pour l'utilisateur {user.UserName}: {password}");
                    ModelState.AddModelError("",
                        "Un courriel contenant votre mot de passe vous a été envoyé.");
                }
                //else
                //{
                //    ModelState.AddModelError(string.Empty,
                //        "");
                //}
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
