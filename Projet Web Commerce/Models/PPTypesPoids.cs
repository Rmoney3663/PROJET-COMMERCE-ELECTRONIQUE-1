using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPTypesPoids
    {
        public int CodePoids { get; set; }
        public decimal PoidsMin { get; set; }
        public decimal PoidsMax { get; set; }
        [InverseProperty("PPTypesPoids")]
        public virtual ICollection<PPPoidsLivraisons>? PPPoidsLivraisons { get; set; }
    }
}
