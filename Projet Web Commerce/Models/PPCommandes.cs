using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPCommandes
    {
        public int NoCommande { get; set; }
        public int NoClient { get; set; }
        public int NoVendeur { get; set; }
        public DateTime DateCommande { get; set; }
        public decimal PoidsTotal { get; set; }
        [Column(TypeName = "char(1)")]
        public string Statut { get; set; }
        public string NoAutorisation { get; set; }
        [InverseProperty("PPCommandes")]
        public virtual ICollection<PPDetailsCommandes>? PPDetailsCommandes { get; set; }
        [InverseProperty("PPCommandes")]
        public virtual ICollection<PPMontantCommandes>? PPMontantCommandes { get; set; }
        [InverseProperty("PPCommandes")]
        public virtual ICollection<PPLivraison>? PPLivraison { get; set; }
        [ForeignKey("NoVendeur")]
        [InverseProperty("PPCommandes")]
        public virtual PPVendeurs? PPVendeurs { get; set; }
        [ForeignKey("NoClient")]
        [InverseProperty("PPCommandes")]
        public virtual PPClients? PPClients { get; set; }
    }
}
