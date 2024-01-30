using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPDetailsCommandes
    {
        [Key]
        public int NoDetailsCommande { get; set; }
        public int NoCommande { get; set; }
        public int NoProduit { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal PrixVente { get; set; }
        public int Quantité { get; set; }
       
        
        [ForeignKey("NoCommande")]
        [InverseProperty("PPDetailsCommandes")]
        public virtual PPCommandes? PPCommandes { get; set; }
        [ForeignKey("NoProduit")]
        [InverseProperty("PPDetailsCommandes")]
        public virtual PPProduits? PPProduits { get; set; }
    }
}
