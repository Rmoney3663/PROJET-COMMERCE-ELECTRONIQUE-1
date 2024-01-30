using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPTypesPoids
    {
        [Key]
        public int CodePoids { get; set; }

        [Column(TypeName = "numeric(8,1)")]
        public decimal PoidsMin { get; set; }
        [Column(TypeName = "numeric(8,1)")]
        public decimal PoidsMax { get; set; }
        [InverseProperty("PPTypesPoids")]
        public virtual ICollection<PPPoidsLivraisons>? PPPoidsLivraisons { get; set; }
    }
}
