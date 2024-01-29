using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class TelephoneVendeurs
    {
        public int NoVendeur { get; set; }
        public string Tel { get; set; }
        [ForeignKey("NoVendeur")]
        [InverseProperty("TelephoneVendeurs")]
        public virtual PPVendeurs? PPVendeurs { get; set; }
    }
}
