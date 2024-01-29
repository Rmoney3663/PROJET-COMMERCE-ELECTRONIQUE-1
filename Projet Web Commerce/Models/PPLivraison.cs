using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPLivraison
    {
        public int NoCommande { get; set; }
        public decimal CoutLivraison { get; set; }
        public int TypeLivraison { get; set; }


        [ForeignKey("NoCommande")]
        [InverseProperty("PPLivraison")]
        public virtual PPCommandes? PPCommandes { get; set; }
        [ForeignKey("TypeLivraison")]
        [InverseProperty("PPLivraison")]
        public virtual PPTypesLivraison? PPTypesLivraison { get; set; }
    }
}
