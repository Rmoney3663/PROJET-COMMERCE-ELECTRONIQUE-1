using System.ComponentModel.DataAnnotations;

namespace Projet_Web_Commerce.Models
{
    public class ModelConfirmerCommande
    {

        public PPVendeurs vendeur { get; set; }
        public PPClients client { get; set; }

        public string? NomClient {  get; set; }
        public string? PrenomClient { get; set; }
        public string? AdresseClient { get; set; }
        public string? TelClient { get; set; }
        public string? RueClient { get; set; }
        public string? VilleClient { get; set; }
        public string? PostalClient { get; set; }
        public string? ProvinceCLient { get; set; }
        public string? PaysClient { get; set; }

        public int NoClient { get; set; }

        public int NoVendeur { get; set; }

        public string NomVendeur { get; set; }
        public decimal sousTotal { get; set; }
        public decimal poidsTotal { get; set; }

        public decimal total { get; set; }

        public string? NoCarte { get; set; }

        public string? dateExpiration { get; set; }

        public string? CVV { get; set; }

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
                if (!DateTime.TryParseExact(dateString, "MM-yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime expirationDate))
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
