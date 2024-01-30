using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class Province
    {
        [Column(TypeName = "char(2)")]
        public string ProvinceID { get; set; } 

        public string Nom { get; set; }

        [InverseProperty("Province")]
        public virtual ICollection<PPClients>? PPClients { get; set; }
        [InverseProperty("Province")]
        public virtual ICollection<PPVendeurs>? PPVendeurs { get; set; }
    }
}
