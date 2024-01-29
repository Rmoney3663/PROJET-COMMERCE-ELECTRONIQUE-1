using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class Province
    {
        [Column(TypeName = "char(2)")]
        public string ProvinceID { get; set; } 

        public string Nom { get; set; }

        [InverseProperty("Province")]
        public virtual ICollection<PPClientAdresse>? PPClientAdresse { get; set; }
        [InverseProperty("Province")]
        public virtual ICollection<PPVendeurAdresse>? PPVendeurAdresse { get; set; }
    }
}
