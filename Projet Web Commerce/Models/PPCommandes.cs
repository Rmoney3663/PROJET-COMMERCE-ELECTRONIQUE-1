using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPCommandes
    {
        [Key]
        public int NoCommande { get; set; }
        public int NoClient { get; set; }
        public int NoVendeur { get; set; }
        public DateTime DateCommande { get; set; }

        [Column(TypeName = "numeric(8,1)")]
        public decimal PoidsTotal { get; set; }
        [Column(TypeName = "char(1)")]
        public string Statut { get; set; }
        public string NoAutorisation { get; set; }

        [Column(TypeName = "smallmoney")]
        public decimal MontantTotAvantTaxes { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal TPS { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal TVQ { get; set; }

        [Column(TypeName = "smallmoney")]
        public decimal CoutLivraison { get; set; }
        
        public int TypeLivraison { get; set; }
        [InverseProperty("PPCommandes")]
        public virtual ICollection<PPDetailsCommandes>? PPDetailsCommandes { get; set; }
        [ForeignKey("TypeLivraison")]
        [InverseProperty("PPCommandes")]
        public virtual PPTypesLivraison? PPTypesLivraison { get; set; }
        [ForeignKey("NoVendeur")]
        [InverseProperty("PPCommandes")]
        public virtual PPVendeurs? PPVendeurs { get; set; }
        [ForeignKey("NoClient")]
        [InverseProperty("PPCommandes")]
        public virtual PPClients? PPClients { get; set; }
    }
}
