using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPClientAdresse
    {
        public int NoClient { get; set; }

        public string Rue { get; set; }

        public string Ville { get; set; }

        [Column(TypeName = "char(2)")]
        public string NoProvince { get; set; }

        public string CodePostal { get; set; }
        public string Pays { get; set; }
        [ForeignKey("Province")]
        [InverseProperty("PPClientAdresse")]
        public virtual Province? Province { get; set; }
        [ForeignKey("NoClient")]
        [InverseProperty("PPClientAdresse")]
        public virtual PPClients? PPClients { get; set; }
    }
}
