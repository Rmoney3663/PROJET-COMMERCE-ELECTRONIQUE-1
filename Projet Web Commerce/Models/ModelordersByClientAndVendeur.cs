namespace Projet_Web_Commerce.Models
{
    public class ModelordersByClientAndVendeur
    {
        public int NoClient { get; set; }
        public string NomPrenomClient { get; set; }
        public decimal TotalCommandeAT { get; set; }
        public decimal CoutLivraison { get; set; }        
        public DateTime DateDerniereCommande { get; set; }
        public int NoVendeur { get; set; }
        public string NomPrenomVendeur { get; set; }
        public string ProvinceVendeur { get; set; }
        public decimal? PourcentageTaxeVendeur { get; set; } 
        public bool? TaxesVendeur { get; set; }
        

    }
}
