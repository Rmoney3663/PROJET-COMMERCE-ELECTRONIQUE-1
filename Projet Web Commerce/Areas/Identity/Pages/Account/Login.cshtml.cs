﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Projet_Web_Commerce.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Projet_Web_Commerce.Data;

namespace Projet_Web_Commerce.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<Utilisateur> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<Utilisateur> _userManager;
        private readonly AuthDbContext _context;

        public LoginModel(SignInManager<Utilisateur> signInManager, ILogger<LoginModel> logger, UserManager<Utilisateur> userManager, AuthDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
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
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

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
            [Required(ErrorMessage = "Le champ {0} est requis.")]
            [EmailAddress(ErrorMessage = "Le champ {0} n'est pas une adresse courriel valide")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Le mot de passe est requis.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Se rappeler de moi?")]
            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (_signInManager.IsSignedIn(User))
            {
                return Redirect("/MainMenu");
            }

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var foundVendeur = _context.PPVendeurs.FirstOrDefault(v => v.AdresseEmail == Input.Email);
                if (foundVendeur != null && foundVendeur.Statut != 1 && foundVendeur.Statut != 2)
                {
                    ModelState.AddModelError(string.Empty, "Votre compte vendeur a besoin d'être validé par un gestionnaire");
                    return Page();
                }
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(Input.Email);                   
                    bool estVendeur = await _userManager.IsInRoleAsync(user, "Vendeur");
                    bool estClient = await _userManager.IsInRoleAsync(user, "Client");
                    bool estGestionnaire = await _userManager.IsInRoleAsync(user, "Gestionnaire");
                    var lstVendeurs = _context.PPVendeurs.ToList();

                    if (foundVendeur != null)
                    {
                        if (estVendeur)
                        {
                            if (foundVendeur.Statut == 2)
                            {
                                foundVendeur.Statut = 1;
                                foundVendeur.DateMAJ = DateTime.Now;
                            }
                            try
                            {
                                await _context.SaveChangesAsync();
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError(string.Empty, "Une erreur s'est produite lors de la mise à jour des informations du vendeur.");
                                _logger.LogError(ex, "An error occurred while updating vendeur information.");
                                return Page();
                            }

                            return RedirectToAction("Index", "PPProduits", new { area = "" });
                        }
                    }
                    _logger.LogInformation("Utilisateur connecté.");

                    if (estClient)
                    {
                        var foundClient = _context.PPClients.FirstOrDefault(c => c.AdresseEmail == Input.Email);
                        if (foundClient != null)
                        {
                            foundClient.DateDerniereConnexion = DateTime.Now;
                            foundClient.NbConnexions++;
                            if (foundClient.Statut == 2)
                            {
                                foundClient.Statut = 1;
                                foundClient.DateMAJ = DateTime.Now;
                            }

                            try
                            {
                                await _context.SaveChangesAsync(); 
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError(string.Empty, "Une erreur s'est produite lors de la mise à jour des informations du client.");
                                _logger.LogError(ex, "An error occurred while updating client information.");
                                return Page();
                            }
                        }
                    }

                    if (estGestionnaire)
                    {
                        return RedirectToAction("ListeVendeurs", "PPGestionnaires", new { area = "" });
                    }
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Compte utilisateur bloqué.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Tentative de connexion non valide.");
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
