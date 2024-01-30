using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class TelephoneVendeurs
    {
        [Key]
        public int NoVendeur { get; set; }
        public string Tel { get; set; }
        [ForeignKey("NoVendeur")]
        [InverseProperty("TelephoneVendeurs")]
        public virtual PPVendeurs? PPVendeurs { get; set; }
    }
}
