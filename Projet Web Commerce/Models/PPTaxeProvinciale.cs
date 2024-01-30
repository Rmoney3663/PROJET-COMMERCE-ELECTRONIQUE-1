using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPTaxeProvinciale
    {
        [Key]
        public int NoTVQ { get; set; }
        public DateTime DateEffectiveTVQ { get; set; }

        [Column(TypeName = "numeric(5,3)")]
        public decimal TauxTVQ { get; set; }
    }
}
