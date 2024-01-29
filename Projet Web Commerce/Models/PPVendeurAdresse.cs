using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPVendeurAdresse
    {
        public int NoVendeur { get; set; }
        public string Rue { get; set; }

        public string Ville { get; set; }

        [Column(TypeName = "char(2)")]
        public string NoProvince { get; set; }

        public string CodePostal { get; set; }
        public string Pays { get; set; }
        [ForeignKey("Province")]
        [InverseProperty("PPVendeurAdresse")]
        public virtual Province? Province { get; set; }
        [ForeignKey("NoVendeur")]
        [InverseProperty("PPVendeurAdresse")]
        public virtual PPVendeurs? PPVendeurs { get; set; }
    }
}
