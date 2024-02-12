using System.ComponentModel.DataAnnotations;

namespace Projet_Web_Commerce.Models
{
    public class ModelConfirmerCommande
    {

        public PPVendeurs vendeur { get; set; }
        public PPClients client { get; set; }
        public int NoClient { get; set; }

        public int NoVendeur { get; set; }

        public string NomVendeur { get; set; }
        public decimal sousTotal { get; set; }
        public decimal poidsTotal { get; set; }

        public decimal total { get; set; }

        [Range(0000000000000000, 9999999999999999, ErrorMessage = "Le numéro de carte doit contenir 16 numéro.")]
        public string NoCarte { get; set; }

        [ExpirationDateGreaterThanCurrent(ErrorMessage = "La date doit être dans le format MM/AAAA et doit être après la date d'aujourd'hui.")]
        public string dateExpiration { get; set; }

        [Range(0, 9999, ErrorMessage = "Numéro CVV/CVC doit se situer entre 0 et 9999.")]
        public int CVV { get; set; }

        public int TypeLivraison { get; set; }

        public decimal? FraisLivraison { get; set; }


    }

    public class ExpirationDateGreaterThanCurrentAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (value is string dateString)
            {
                if (!DateTime.TryParseExact(dateString, "MM/yy", null, System.Globalization.DateTimeStyles.None, out DateTime expirationDate))
                {
                    return new ValidationResult("Invalid date format. Date format must be MM/AA.");
                }

                if (expirationDate <= DateTime.Now)
                {
                    return new ValidationResult("Expiration date must be greater than the current date.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
