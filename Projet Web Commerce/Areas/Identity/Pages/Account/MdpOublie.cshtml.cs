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

namespace Projet_Web_Commerce.Areas.Identity.Pages.Account
{
    public class MdpOublieModel : PageModel
    {
        private readonly UserManager<Utilisateur> _userManager;
        private readonly IEmailSender _emailSender;

        public MdpOublieModel(UserManager<Utilisateur> userManager, IEmailSender emailSender)
        {
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

                //var mdpDecode = 

                await Methodes.envoyerCourriel(Input.Email, "Oubli du mot de passe",
                        $"Voici le mot de passe pour l'utilisateur {user.UserName}: {"chien"}");
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
