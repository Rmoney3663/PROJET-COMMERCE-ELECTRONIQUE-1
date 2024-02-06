namespace Projet_Web_Commerce.Models
{
    public class ModelCatalogueVendeur
    {
        public string nomAffaire { get; set; }
        public int noClient { get; set; }
        public List<PPVendeurs> VendeursList { get; set; }
        public List<PPCategories> CategoriesList { get; set; }
        public List<PPProduits> ProduitsList { get; set; }
        public List<PPProduits> NouveauxProduits { get; set; }
    }
}
