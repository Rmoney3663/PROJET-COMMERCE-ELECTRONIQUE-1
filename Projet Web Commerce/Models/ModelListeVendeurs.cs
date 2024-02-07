

namespace Projet_Web_Commerce.Models
{
    public class ModelListeVendeurs
    {
        //public Dictionary<List<int>, PPVendeurs> VendeursList { get; set; }
        public List<PPVendeurs> VendeursList { get; set; }

        public List<PPProduits> ProduitsList { get; set; }

        public List<ModelMoisAnnees> MoisAnneesDistinctsList { get; set; }
    }
}
