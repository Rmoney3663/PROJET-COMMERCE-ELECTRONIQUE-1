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
        private readonly AuthDbContext _context;
        private readonly SignInManager<Utilisateur> _signInManager;
        private readonly UserManager<Utilisateur> _userManager;
        private readonly IUserStore<Utilisateur> _userStore;
        private readonly IUserEmailStore<Utilisateur> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel2(
            AuthDbContext context,
            UserManager<Utilisateur> userManager,
            IUserStore<Utilisateur> userStore,
            SignInManager<Utilisateur> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            
            [Required(ErrorMessage = "Le courriel est requis.")]
            [EmailAddress(ErrorMessage = "Le champ 'Courriel' n'est pas une adresse courriel valide.")]
            [Display(Name = "Courriel")]
            public string Email { get; set; }

            //[Required(ErrorMessage = "Retaper le courriel.")]
            [EmailAddress(ErrorMessage = "Le champ 'Confirmer courriel' n'est pas une adresse courriel valide.")]
            [Display(Name = "Confirmer courriel")]
            [Compare("Email", ErrorMessage = "Le courriel et le courriel de confirmation ne correspondent pas.")]
            public string ConfirmEmail { get; set; }

            
            [Required(ErrorMessage = "Le mot de passe est requis.")]
            [StringLength(100, ErrorMessage = "Le {0} doit comporter au moins {2} et au maximum {1} caractères.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mot de passe")]
            public string Password { get; set; }

            //[Required(ErrorMessage = "Retaper le mot de passe.")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirmer mot de passe")]
            [Compare("Password", ErrorMessage = "Le mot de passe et le mot de passe de confirmation ne correspondent pas.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Le poids est requis.")]
            [Display(Name = "Poids max pour livraison (Kg)")]
            public int SelectedNumberPoids { get; set; }

            [Required(ErrorMessage = "Le prix est requis.")]
            [Display(Name = "Prix pour livraison gratuite ($ CAD)")]
            public int SelectedNumberLivraison { get; set; }

            [Required(ErrorMessage = "Le taxe est requis.")]
            [Display(Name = "Client doit payer taxe?")]
            public bool Tax { get; set; }

            [Required(ErrorMessage = "La province est requise.")]
            [Display(Name = "Sélectionner la province")]
            public string SelectedProvince { get; set; }

            [Required(ErrorMessage = "Le nom de la ville est requis.")]
            [Display(Name = "Nom de la ville")]
            public string Ville { get; set; }

            [Required(ErrorMessage = "Le nom de la rue est requis.")]
            [Display(Name = "Nom de la rue")]
            public string Rue { get; set; }

            [Required(ErrorMessage = "Le nom d'affaires est requis.")]
            [Display(Name = "Nom d'affaires")]
            public string NomAffaires { get; set; }

            [Required(ErrorMessage = "Le code postal est requis.")]
            [RegularExpression(@"^([A-Za-z]\d[A-Za-z] \d[A-Za-z]\d)|([A-Za-z]\d[A-Za-z]\d[A-Za-z]\d)$", ErrorMessage = "Le code postal doit être au format A1A 1A1 ou A1A1A1.")]
            [Display(Name = "Code Postal")]
            public string CodePostal { get; set; }

            [Required(ErrorMessage = "Le nom est requis.")]
            [Display(Name = "Nom")]
            public string Nom { get; set; }

            [Required(ErrorMessage = "Le prénom est requis.")]
            [Display(Name = "Prénom")]
            public string Prenom { get; set; }

            [Required(ErrorMessage = "Le numéro de téléphone est requis")]
            [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Le numéro de téléphone doit respecter le format 999-999-9999.")]
            [Display(Name = "Numéro de téléphone")]
            public string PhoneNumber1 { get; set; }

            [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Le cellulaire doit respecter le format 999-999-9999.")]
            [Display(Name = "Cellulaire (optionnel)")]
            public string PhoneNumber2 { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (Input == null)
            {
                Input = new InputModel();
            }

            Input.SelectedNumberPoids = 0;

            Input.SelectedNumberLivraison = 0;
            Input.Tax = true;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            var compagnieExistante = _context.PPVendeurs.FirstOrDefault(c => c.NomAffaires == Input.NomAffaires);

            if (compagnieExistante != null)
            {
                ModelState.AddModelError(string.Empty, "Ce nom d'affaires existe déjà");
            }

            if (ModelState.IsValid)
            {
                var SelectedNumberPoids = Input.SelectedNumberPoids;
                var SelectedNumberLivraison = Input.SelectedNumberLivraison;
                var email = Input.Email;
                var password = Input.Password;
                var date = DateTime.Now;
                var taxe = Input.Tax;
                var province = Input.SelectedProvince;
                var ville = Input.Ville;
                var rue = Input.Rue;
                var codePostal = Input.CodePostal;
                var nom = Input.Nom;
                var prenom = Input.Prenom;
                var phone1 = Input.PhoneNumber1;
                var phone2 = Input.PhoneNumber2;
                var pays = "Canada";

                if (codePostal.Length == 6)
                {
                    codePostal = codePostal.Insert(3, " ");
                }

                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Vendeur");

                    var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
                    optionsBuilder.UseSqlServer("Data Source=tcp:424sql.cgodin.qc.ca,5433;Initial Catalog=BDB68_424Q24;User ID=B68equipe424q24;Password=Password24;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Integrated Security=False");

                    int lowestNo = 10;

                    if (_context.PPVendeurs.Any())
                    {
                        int minNo = 10;
                        int maxNo = 99;

                        for (int i = minNo; i <= maxNo; i++)
                        {
                            if (!_context.PPVendeurs.Any(u => u.NoVendeur == i))
                            {
                                lowestNo = i;
                                break;
                            }
                        }
                    }

                    var pourcentage = 0.00;
                    if(taxe == true)
                    {
                        if(province == "QC")
                        {
                            pourcentage = 14.975;
                        }
                        else
                        {
                            pourcentage = 5.00;
                        }
                    }

                    PPVendeurs newRecord = new PPVendeurs()
                    { 
                      IdUtilisateur = user.Id, NoVendeur = lowestNo, AdresseEmail = email, MotDePasse = password, Taxes = taxe, NomAffaires = Input.NomAffaires, NoProvince = province,
                      DateCreation = date, PoidsMaxLivraison = SelectedNumberPoids, LivraisonGratuite = SelectedNumberLivraison, Statut = 0, Pourcentage = (decimal?)pourcentage,
                      Ville = ville, Pays = "Canada", Rue = rue, CodePostal = codePostal, Prenom = prenom, Nom = nom, Tel1 = phone1, Tel2 = phone2
                    };
                    _context.PPVendeurs.Add(newRecord);
                    _context.SaveChanges();


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
