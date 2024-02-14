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
    public class ProfilClientModel : PageModel
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<Utilisateur> _userManager;
        private readonly SignInManager<Utilisateur> _signInManager;

        public ProfilClientModel(
            AuthDbContext context,
            UserManager<Utilisateur> userManager,
            SignInManager<Utilisateur> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Courriel { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            //[Required(ErrorMessage = "Le nom est requis.")]
            [Display(Name = "Nom")]
            public string Nom { get; set; }

            //[Required(ErrorMessage = "Le prénom est requis.")]
            [Display(Name = "Prénom")]
            public string Prenom { get; set; }

            //[Required(ErrorMessage = "La rue est requise.")]
            [Display(Name = "Rue")]
            public string Rue { get; set; }

            //[Required(ErrorMessage = "La ville est requise.")]
            [Display(Name = "Ville")]
            public string Ville { get; set; }

            [Display(Name = "Province")]
            public string Province { get; set; }

            //[Required(ErrorMessage = "Le code postal est requis.")]
            [RegularExpression(@"^([A-Za-z]\d[A-Za-z] \d[A-Za-z]\d)|([A-Za-z]\d[A-Za-z]\d[A-Za-z]\d)$", ErrorMessage = "Le code postal doit respecter le format A1A 1A1 ou A1A1A1.")]
            [Display(Name = "Code postal")]
            public string CodePostal { get; set; }

            [Display(Name = "Pays")]
            public string Pays { get; set; }

            //[Required(ErrorMessage = "Le numéro de téléphone est requis")]
            [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Le numéro de téléphone doit respêcter le format 999-999-9999.")]
            [Display(Name = "Numéro de téléphone")]
            public string Telephone { get; set; }

            [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Le cellulaire doit respecter le format 999-999-9999.")]
            [Display(Name = "Cellulaire (optionnel)")]
            public string Cellulaire { get; set; }
        }

        private async Task LoadAsync(Utilisateur user)
        {
            var email = await _userManager.GetEmailAsync(user);
            var nom = _context.PPClients.FirstOrDefault(x => x.AdresseEmail == email).Nom;
            var prenom = _context.PPClients.FirstOrDefault(x => x.AdresseEmail == email).Prenom;
            var rue = _context.PPClients.FirstOrDefault(x => x.AdresseEmail == email).Rue;
            var ville = _context.PPClients.FirstOrDefault(x => x.AdresseEmail == email).Ville;
            var province = _context.PPClients.FirstOrDefault(x => x.AdresseEmail == email).NoProvince;
            var codePostal = _context.PPClients.FirstOrDefault(x => x.AdresseEmail == email).CodePostal;
            var pays = "Canada"; //_context.PPClients.FirstOrDefault(x => x.AdresseEmail == email).Pays;
            var telephone = _context.PPClients.FirstOrDefault(x => x.AdresseEmail == email).Tel1;
            var cellulaire = _context.PPClients.FirstOrDefault(x => x.AdresseEmail == email).Tel2;

            Courriel = email;

            Input = new InputModel
            {
                Nom = nom,
                Prenom = prenom,
                Rue = rue,
                Ville = ville,
                Province = province,
                CodePostal = codePostal,
                Pays = pays,
                Telephone = telephone,
                Cellulaire = cellulaire
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
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

            //ModelState.Remove("");
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
            var telephone = Input.Telephone;
            var cellulaire = (Input.Cellulaire == "" ? null : Input.Cellulaire);

            if (codePostal.Length == 6)
            {
                codePostal = codePostal.Insert(3, " ");
            }
            
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
            clientCourant.Tel1 = telephone;
            clientCourant.Tel2 = cellulaire;
            await _context.SaveChangesAsync();

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Votre profil a été mis à jour";
            return RedirectToPage();
        }
    }
}
