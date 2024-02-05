// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;

namespace Projet_Web_Commerce.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<Utilisateur> _userManager;
        private readonly SignInManager<Utilisateur> _signInManager;

        public IndexModel(
            AuthDbContext context,
            UserManager<Utilisateur> userManager,
            SignInManager<Utilisateur> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Courriel { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

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
            [Required(ErrorMessage = "Le nom est requis.")]
            [Display(Name = "Nom")]
            public string Nom { get; set; }

            [Required(ErrorMessage = "Le prénom est requis.")]
            [Display(Name = "Prénom")]
            public string Prenom { get; set; }

            [Required(ErrorMessage = "La rue est requise.")]
            [Display(Name = "Rue")]
            public string Rue { get; set; }

            [Required(ErrorMessage = "La ville est requise.")]
            [Display(Name = "Ville")]
            public string Ville { get; set; }

            [Display(Name = "Province")]
            public string Province { get; set; }

            [Required(ErrorMessage = "Le code postal est requis.")]
            [RegularExpression(@"^([A-Za-z]\d[A-Za-z] \d[A-Za-z]\d)|([A-Za-z]\d[A-Za-z]\d[A-Za-z]\d)$", ErrorMessage = "Le code postal doit respecter le format A1A 1A1 ou A1A1A1.")]
            [Display(Name = "Code postal")]
            public string CodePostal { get; set; }

            [Display(Name = "Pays")]
            public string Pays { get; set; }
        }

        private async Task LoadAsync(Utilisateur user)
        {
            var email = await _userManager.GetEmailAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Courriel = email;

            //Input = new InputModel
            //{
            //    PhoneNumber = phoneNumber
            //};
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            bool estDansRole = await _userManager.IsInRoleAsync(user, "Vendeur");
            if (estDansRole)
            {

            }

            await LoadAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var dateMAJ = DateTime.Now;
            var nom = Input.Nom;
            var prenom = Input.Prenom;
            var rue = Input.Rue;
            var ville = Input.Ville;
            var province = Input.Province;
            var codePostal = Input.CodePostal.ToUpper();
            var pays = "Canada";

            if (codePostal.Length == 6)
            {
                codePostal = codePostal.Insert(3, " ");
            }

            bool estDansRole = await _userManager.IsInRoleAsync(user, "Vendeur");
            if (estDansRole) // Si user est vendeur
            {
                var lstVendeurs = _context.PPVendeurs.ToList();
                var vendeurCourant = lstVendeurs.FirstOrDefault(v => v.AdresseEmail == user.Email);
                //vendeurCourant.
                await _context.SaveChangesAsync();
            }
            else // User est client
            {
                var lstClients = _context.PPClients.ToList();
                var clientCourant = lstClients.FirstOrDefault(c => c.AdresseEmail == user.Email);
                clientCourant.DateMAJ = dateMAJ;
                clientCourant.Nom = nom;
                clientCourant.Prenom = prenom;
                clientCourant.Rue = rue;
                clientCourant.Ville = ville;
                clientCourant.NoProvince = province;
                clientCourant.CodePostal = codePostal;
                clientCourant.Pays = pays;
                await _context.SaveChangesAsync();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Votre profil a été mis à jour";
            return RedirectToPage();
        }
    }
}
