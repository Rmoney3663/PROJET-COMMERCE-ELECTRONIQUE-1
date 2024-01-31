// Licensed to the .NET Foundation under one or more agreements.
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
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;
using Projet_Web_Commerce.Models;

namespace Projet_Web_Commerce.Areas.Identity.Pages.Account
{
    public class RegisterModel2 : PageModel
    {
        private readonly SignInManager<Utilisateur> _signInManager;
        private readonly UserManager<Utilisateur> _userManager;
        private readonly IUserStore<Utilisateur> _userStore;
        private readonly IUserEmailStore<Utilisateur> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel2(
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
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Confirm Email")]
            [Compare("Email", ErrorMessage = "L'email et l'email de confirmation ne correspondent pas.")]
            public string ConfirmEmail { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "Le {0} doit comporter au moins {2} et au maximum {1} caractères.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "Le mot de passe et le mot de passe de confirmation ne correspondent pas.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "Poids max pour livraison (Kg)")]
            public int SelectedNumberPoids { get; set; }

            [Required]
            [Display(Name = "Prix pour livraison gratuite ($ CAD)")]
            public int SelectedNumberLivraison { get; set; }

            [Required]
            [Display(Name = "Client doit payer taxe ?")]
            public bool Tax { get; set; }

            [Required]
            [Display(Name = "Sélectionner la province")]
            public string SelectedProvince { get; set; }

            [Required]
            [Display(Name = "Nom d'affaires")]
            public string NomAffaires { get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (Input == null)
            {
                Input = new InputModel();
            }

            Input.SelectedNumberPoids = 1;

            Input.SelectedNumberLivraison = 0;
            Input.Tax = true;
            //Input.SelectedProvince= "QC";
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var SelectedNumberPoids = Input.SelectedNumberPoids;
                var SelectedNumberLivraison = Input.SelectedNumberLivraison;
                var email = Input.Email;
                var password = Input.Password;
                var date = DateTime.Now;
                var taxe = Input.Tax;
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Vendeur");

                    var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
                    optionsBuilder.UseSqlServer("Data Source=tcp:424sql.cgodin.qc.ca,5433;Initial Catalog=BDB68_424Q24;User ID=B68equipe424q24;Password=Password24;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Integrated Security=False");

                    var context = new AuthDbContext(optionsBuilder.Options);

                    int lowestNo = 10;

                    if (context.PPVendeurs.Any())
                    {
                        int minNo = 10;
                        int maxNo = 99;

                        for (int i = minNo; i <= maxNo; i++)
                        {
                            if (!context.PPVendeurs.Any(u => u.NoVendeur == i))
                            {
                                lowestNo = i;
                                break;
                            }
                        }
                    }


                    PPVendeurs newRecord = new PPVendeurs()
                    { IdUtilisateur = user.Id, NoVendeur = lowestNo, AdresseEmail = email, MotDePasse = password, Taxes = taxe, NomAffaires = Input.NomAffaires,
                      DateCreation = date, PoidsMaxLivraison = SelectedNumberPoids, LivraisonGratuite = SelectedNumberLivraison, Statut = 0 };

                    context.PPVendeurs.Add(newRecord);
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

                    await Methodes.envoyerCourriel(Input.Email, "Confirmer votre adresse courriel",
                        $"Veuillez confirmer votre compte en <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>cliquant ici</a>. Votre numéro de vendeur est {lowestNo}");

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
