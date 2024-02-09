using Microsoft.AspNetCore.Mvc.Rendering;
using Projet_Web_Commerce.Controllers;

namespace Projet_Web_Commerce.Models
{
    public class ModelCatalogueVendeur
    {
        public string nomAffaire { get; set; }
        public int noClient { get; set; }
        public List<PPCategories> CategoriesList { get; set; }
        public PaginatedList<PPProduits> ProduitsList { get; set; }
        public List<PPProduits> NouveauxProduits { get; set; }

        // Search fields //
        public string? searchString { get; set; }

        // Add properties for sort order
        public List<SelectListItem> sortOrderOptions { get; set; }
        public string? sortOrder { get; set; }

        // Add properties for parPage
        public List<SelectListItem> parPageOptions { get; set; }
        public string? parPage { get; set; }

        // Add properties for searchCat
        public List<SelectListItem> categorieOptions { get; set; }
        public string? searchCat { get; set; }

        public int? pageNumber { get; set; }
        
        public DateTime? dateApres { get; set; }
        public DateTime? dateAvant { get; set; }

        public bool? menuVis {  get; set; }
    }
}
