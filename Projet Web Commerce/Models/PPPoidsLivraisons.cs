using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPPoidsLivraisons
    {
        public int CodeLivraison { get; set; }
        public int CodePoids { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal Tarif { get; set; }


        [ForeignKey("CodeLivraison")]
        [InverseProperty("PPPoidsLivraisons")]
        public virtual PPTypesLivraison? PPTypesLivraison { get; set; }
        [ForeignKey("CodePoids")]
        [InverseProperty("PPPoidsLivraisons")]
        public virtual PPTypesPoids? PPTypesPoids { get; set; }
    }
}
