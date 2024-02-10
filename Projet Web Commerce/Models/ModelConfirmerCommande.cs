namespace Projet_Web_Commerce.Models
{
    public class ModelConfirmerCommande
    {

        public PPVendeurs vendeur { get; set; }
        public PPClients client { get; set; }
        public decimal sousTotal { get; set; }
        public decimal poidsTotal { get; set; }


    }
}
