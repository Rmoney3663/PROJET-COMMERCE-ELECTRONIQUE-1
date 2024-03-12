#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;
using System.ComponentModel.DataAnnotations;

namespace Projet_Web_Commerce.Areas.Identity.Pages.Account.Manage
{
    public class ProfilVendeurModel : PageModel
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<Utilisateur> _userManager;
        private readonly SignInManager<Utilisateur> _signInManager;

        public ProfilVendeurModel(
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
            public string? Pays { get; set; }

            [Required(ErrorMessage = "Le numéro de téléphone est requis")]
            [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Le numéro de téléphone doit respêcter le format 999-999-9999.")]
            [Display(Name = "Numéro de téléphone")]
            public string Telephone { get; set; }

            [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Le cellulaire doit respecter le format 999-999-9999.")]
            [Display(Name = "Cellulaire (optionnel)")]
            public string Cellulaire { get; set; }


            [Required(ErrorMessage = "Le nom d'affaires est requis")]
            [Display(Name = "Nom d'affaires")]
            public string NomAffaires { get; set; }

            [Required(ErrorMessage = "Le poids maximum de la livraison est requis")]
            [Display(Name = "Poids max livraison")]
            public int PoidsMaxLivraison { get; set; }

            [Required(ErrorMessage = "Le montant minimal pour la livraison gratuite est requis")]
            [Display(Name = "Coût livraison gratuite")]
            public decimal LivraisonGratuite { get; set; }

            [Required(ErrorMessage = "La taxe est requise")]
            [Display(Name = "Taxes")]
            public bool Taxes { get; set; }

            [Display(Name = "Pourcentage")]
            public decimal Pourcentage { get; set; }
           
        }

        private async Task LoadAsync(Utilisateur user)
        {
            var email = await _userManager.GetEmailAsync(user);
            var nom = _context.PPVendeurs.FirstOrDefault(x => x.AdresseEmail == email).Nom;
            var prenom = _context.PPVendeurs.FirstOrDefault(x => x.AdresseEmail == email).Prenom;
            var rue = _context.PPVendeurs.FirstOrDefault(x => x.AdresseEmail == email).Rue;
            var ville = _context.PPVendeurs.FirstOrDefault(x => x.AdresseEmail == email).Ville;
            var province = _context.PPVendeurs.FirstOrDefault(x => x.AdresseEmail == email).NoProvince;
            var codePostal = _context.PPVendeurs.FirstOrDefault(x => x.AdresseEmail == email).CodePostal;
            var pays = _context.PPVendeurs.FirstOrDefault(x => x.AdresseEmail == email).Pays;
            var telephone = _context.PPVendeurs.FirstOrDefault(x => x.AdresseEmail == email).Tel1;
            var cellulaire = _context.PPVendeurs.FirstOrDefault(x => x.AdresseEmail == email).Tel2;

            var nomAffaires = _context.PPVendeurs.FirstOrDefault(x => x.AdresseEmail == email).NomAffaires;
            var poidsMaxLivraison = (int) _context.PPVendeurs.FirstOrDefault(x => x.AdresseEmail == email).PoidsMaxLivraison;
            var livraisonGratuite = (decimal) _context.PPVendeurs.FirstOrDefault(x => x.AdresseEmail == email).LivraisonGratuite;
            var taxes = (bool) _context.PPVendeurs.FirstOrDefault(x => x.AdresseEmail == email).Taxes;
            var pourcentage = (decimal) _context.PPVendeurs.FirstOrDefault(x => x.AdresseEmail == email).Pourcentage;

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
                Cellulaire = cellulaire,

                NomAffaires = nomAffaires,
                PoidsMaxLivraison = poidsMaxLivraison,
                LivraisonGratuite = livraisonGratuite,
                Taxes = taxes,
                Pourcentage = pourcentage               
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

            var compagnieExistante = _context.PPVendeurs.FirstOrDefault(c => c.NomAffaires == Input.NomAffaires);

            if (compagnieExistante != null)
            {
                ModelState.AddModelError(string.Empty, "Ce nom d'affaires existe déjà");
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
            var telephone = Input.Telephone;
            var cellulaire = (Input.Cellulaire == "" ? null : Input.Cellulaire);

            var nomAffaires = Input.NomAffaires;
            var poidsMaxLivraison = Input.PoidsMaxLivraison;
            var livraisonGratuite = Input.LivraisonGratuite;
            var taxes = Input.Taxes;

            if (codePostal.Length == 6)
            {
                codePostal = codePostal.Insert(3, " ");
            }

            var lstVendeurs = _context.PPVendeurs.ToList();
            var vendeurCourant = lstVendeurs.FirstOrDefault(v => v.AdresseEmail == user.Email);

            vendeurCourant.DateMAJ = dateMAJ;
            vendeurCourant.Nom = nom;
            vendeurCourant.Prenom = prenom;
            vendeurCourant.Rue = rue;
            vendeurCourant.Ville = ville;
            vendeurCourant.NoProvince = province;
            vendeurCourant.CodePostal = codePostal;
            vendeurCourant.Pays = pays;
            vendeurCourant.Tel1 = telephone;
            vendeurCourant.Tel2 = cellulaire;

            vendeurCourant.NomAffaires = nomAffaires;
            vendeurCourant.PoidsMaxLivraison = poidsMaxLivraison;
            vendeurCourant.LivraisonGratuite = livraisonGratuite;
            vendeurCourant.Taxes = taxes;           
            vendeurCourant.PourcentageTaxe = Methodes.pourcentageTaxes(taxes, province);

            await _context.SaveChangesAsync();

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Votre profil a été mis à jour";
            return RedirectToPage();
        }
    }
}
