namespace Projet_Web_Commerce.Models
{
    public class PPHistoriquePaiements
    {
        public int NoHistorique { get; set; }
        public decimal MontantVenteAvantLivraison { get; set; }
        public int NoVendeur { get; set; }
        public int NoClient { get; set; }
        public int NoCommande { get; set; }
        public DateTime DateVente { get; set; }
        public string NoAutorisation { get; set; }
        public decimal FraisLesi { get; set; }
        public decimal Redevance { get; set; }
        public decimal FraisLivraison { get; set; }
        public decimal FraisTPS { get; set; }
        public decimal FraisTVQ { get; set; }
    }
}
