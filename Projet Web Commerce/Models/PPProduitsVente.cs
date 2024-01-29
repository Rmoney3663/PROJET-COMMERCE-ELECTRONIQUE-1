using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPProduitsVente
    {
        public int NoProduit { get; set; }
        public DateTime DateVente { get; set; }
        public decimal PrixVente { get; set; }
        [ForeignKey("NoProduit")]
        [InverseProperty("PPProduitsVente")]
        public virtual PPProduits? PPProduits { get; set; }
    }
}
