// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Projet_Web_Commerce.Areas.Identity.Data;

namespace Projet_Web_Commerce.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<Utilisateur> _userManager;
        private readonly IEmailSender _emailSender;

        public ResendEmailConfirmationModel(UserManager<Utilisateur> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Le champ {0} est requis")]
            [EmailAddress(ErrorMessage = "Le champ {0} n'est pas une adresse courriel valide")]
            public string Courriel { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Courriel);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Cet utilisateur n'existe pas.");
                return Page();
            }

            if (!user.EmailConfirmed)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { userId = userId, code = code },
                    protocol: Request.Scheme);
                await Methodes.envoyerCourriel(
                            "Confirmer votre courriel",
                            $"Veuillez confirmer votre compte en <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>cliquant ici</a>.",
                            Input.Courriel);

                ModelState.AddModelError(string.Empty, "Courriel de vérification envoyé. Veuillez vérifier votre courriel.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Ce compte a déjà été confirmé.");
            }
            
            return Page();
        }
    }
}
