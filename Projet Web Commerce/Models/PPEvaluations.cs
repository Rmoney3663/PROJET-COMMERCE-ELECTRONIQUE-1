using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPEvaluations
    {
        [Key]
        public int Id { get; set; }
        public int NoClient { get; set; }
        public int NoProduit { get; set; }       

        [Column(TypeName = "numeric(8,2)")]
        public decimal Cote { get; set; }

        public string Commentaire { get; set; }
        public DateTime DateMAJ { get; set; }
        public DateTime DateCreation { get; set; }

        [ForeignKey("NoProduit")]
        [InverseProperty("PPEvaluations")]
        public virtual PPProduits? PPProduits { get; set; }
        [ForeignKey("NoClient")]
        [InverseProperty("PPEvaluations")]
        public virtual PPClients? PPClients { get; set; }
    }
}
