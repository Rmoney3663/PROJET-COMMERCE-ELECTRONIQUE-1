namespace Projet_Web_Commerce.Models
{
    public class ModelCatalogue
    {
        public List<PPVendeurs> VendeursList { get; set; }
        public List<PPCategories> CategoriesList { get; set; }

        public List<PPProduits> ProduitsList { get; set; }
        public List<PPProduits> NouveauxProduits { get; set; }
    }
}
