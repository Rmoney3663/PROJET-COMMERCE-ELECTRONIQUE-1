using System.ComponentModel.DataAnnotations;

namespace Projet_Web_Commerce.Models
{
    public class ModelConfirmerCommande
    {

        public PPVendeurs vendeur { get; set; }
        public PPClients client { get; set; }
        public decimal sousTotal { get; set; }
        public decimal poidsTotal { get; set; }

        [Range(0000000000000000, 9999999999999999, ErrorMessage = "Le numéro de carte doit contenir 16 numéro.")]
        public long NoCarte { get; set; }
        public string dateExpiration { get; set; }

        [Range(0, 9999, ErrorMessage = "Numéro CVV/CVC doit se situer entre 0 et 9999.")]
        public int CVV { get; set; }


    }
}
