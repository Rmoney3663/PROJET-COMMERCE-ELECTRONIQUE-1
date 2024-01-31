using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPHistoriquePaiements
    {
        [Key]
        public int NoHistorique { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal MontantVenteAvantLivraison { get; set; }
        public int NoVendeur { get; set; }
        public int NoClient { get; set; }
        public int NoCommande { get; set; }
        public DateTime DateVente { get; set; }
        public string NoAutorisation { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal FraisLesi { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal Redevance { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal FraisLivraison { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal FraisTPS { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal FraisTVQ { get; set; }
    }
}
