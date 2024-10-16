﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;
using Projet_Web_Commerce.Models;

namespace Projet_Web_Commerce.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Utilisateur> _signInManager;
        private readonly UserManager<Utilisateur> _userManager;
        private readonly IUserStore<Utilisateur> _userStore;
        private readonly IUserEmailStore<Utilisateur> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<Utilisateur> userManager,
            IUserStore<Utilisateur> userStore,
            SignInManager<Utilisateur> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
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
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

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
            [Required(ErrorMessage = "Le courriel est requis.")]
            [EmailAddress(ErrorMessage = "Le champ {0} n'est pas une adresse courriel valide")]
            [Display(Name = "Courriel")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Veuillez entrer le courriel à nouveau")]
            [EmailAddress(ErrorMessage = "Le champ {0} n'est pas une adresse courriel valide")]
            [Display(Name = "Confirmer le courriel")]
            [Compare("Email", ErrorMessage = "Les courriels ne correspondent pas.")]
            public string ConfirmEmail { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Le mot de passe est requis.")]
            [StringLength(100, ErrorMessage = "Le mot de passe doit comporter au moins {2} et au maximum {1} caractères.", MinimumLength = 6)]
            [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
            [Display(Name = "Mot de passe")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
            [Display(Name = "Confirmer le mot de passe")]
            [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (_signInManager.IsSignedIn(User))
            {
                return Redirect("/MainMenu");
            }
            else
                return Page();
           
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Client");

                    var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
                    optionsBuilder.UseSqlServer("Data Source=tcp:424sql.cgodin.qc.ca,5433;Initial Catalog=BDB68_424Q24;User ID=B68equipe424q24;Password=Password24;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Integrated Security=False");

                    var context = new AuthDbContext(optionsBuilder.Options);

                    int lowestNo = 10000;

                    if (context.PPClients.Any())
                    {
                        int minNo = 10000;
                        int maxNo = 99999;

                        for (int i = minNo; i <= maxNo; i++)
                        {
                            if (!context.PPClients.Any(u => u.NoClient == i))
                            {
                                lowestNo = i;
                                break;
                            }
                        }
                    }
                    
                    PPClients newRecord = new PPClients()
                    { IdUtilisateur = user.Id, NoClient = lowestNo, MotDePasse= Input.Password, DateCreation=DateTime.Now, Statut=0, AdresseEmail=Input.Email, NbConnexions=0, Pays="Canada" };

                    context.PPClients.Add(newRecord);
                    context.SaveChanges();

                    _logger.LogInformation("L'utilisateur a créé un nouveau compte avec un mot de passe.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await Methodes.envoyerCourriel(
                        "Confirmer votre courriel", 
                        $"Veuillez confirmer votre compte en <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>cliquant ici</a>. Votre numéro de client est {lowestNo}",
                        Input.Email);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    if (error.Code == "PasswordRequiresNonAlphanumeric")
                        ModelState.AddModelError(string.Empty, "Les mots de passe doivent comporter au moins un caractère non alphanumérique.");
                    else if (error.Code == "PasswordRequiresDigit")
                        ModelState.AddModelError(string.Empty, "Les mots de passe doivent comporter au moins un chiffre ('0'-'9').");
                    else if (error.Code == "PasswordRequiresLower")
                        ModelState.AddModelError(string.Empty, "Les mots de passe doivent comporter au moins une minuscule ('a'-'z').");
                    else if (error.Code == "PasswordRequiresUpper")
                        ModelState.AddModelError(string.Empty, "Les mots de passe doivent comporter au moins une majuscule ('A'-'Z').");
                    else
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private Utilisateur CreateUser()
        {
            try
            {
                return Activator.CreateInstance<Utilisateur>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(Utilisateur)}'. " +
                    $"Ensure that '{nameof(Utilisateur)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<Utilisateur> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("L'interface utilisateur par défaut nécessite un magasin d'utilisateurs avec support par courrier électronique.");
            }
            return (IUserEmailStore<Utilisateur>)_userStore;
        }
    }
}
