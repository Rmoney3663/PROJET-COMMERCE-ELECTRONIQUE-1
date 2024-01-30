using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPTaxeFederale
    {
        [Key]
        public int NoTPS { get; set; }
        public DateTime DateEffectiveTPS { get; set; }
        [Column(TypeName = "numeric(4,2)")]
        public decimal TauxTPS { get; set; }

    }
}
