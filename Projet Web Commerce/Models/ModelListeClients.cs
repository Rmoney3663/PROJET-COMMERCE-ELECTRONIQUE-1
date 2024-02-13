namespace Projet_Web_Commerce.Models
{
    public class ModelListeClients
    {
        public List<PPClients> ClientsList { get; set; }
        public List<PPVendeurs> VendeursList { get; set; }

        public List<PPProduits> ProduitsList { get; set; }

        public List<ModelMoisAnnees> MoisAnneesDistinctsList { get; set; }

        public List<PPCommandes> CommandesList { get; set; }

        public List<PPVendeursClients> VendeursClientsList { get; set; }
    }
}
