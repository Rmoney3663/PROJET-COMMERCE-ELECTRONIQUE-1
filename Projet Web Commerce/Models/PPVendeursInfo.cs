using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPVendeursInfo
    {
        public int NoVendeur { get; set; }
        public string AdresseEmail { get; set; }
        public string MotDePasse { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        [ForeignKey("NoVendeur")]
        [InverseProperty("PPVendeursInfo")]
        public virtual PPVendeurs? PPVendeurs { get; set; }
    }
}
