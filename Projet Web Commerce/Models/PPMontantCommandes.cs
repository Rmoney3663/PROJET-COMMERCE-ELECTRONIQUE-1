using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPMontantCommandes
    {
        public int NoCommande { get; set; }
        public decimal MontantTotAvantTaxes { get; set; }
        public decimal TPS { get; set; }
        public decimal TVQ { get; set; }


        [ForeignKey("NoCommande")]
        [InverseProperty("PPMontantCommandes")]
        public virtual PPCommandes? PPCommandes { get; set; }
    }
}
