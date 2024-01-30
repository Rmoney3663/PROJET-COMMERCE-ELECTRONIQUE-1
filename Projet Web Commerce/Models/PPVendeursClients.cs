using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPVendeursClients
    {
        public int NoVendeur { get; set; }
        
        public int NoClient { get; set; }

        public DateTime DateVisite { get; set; }
        [ForeignKey("NoClient")]
        [InverseProperty("PPVendeursClients")]
        public virtual PPClients? PPClients { get; set; }
        [ForeignKey("NoVendeur")]
        [InverseProperty("PPVendeursClients")]
        public virtual PPVendeurs? PPVendeurs { get; set; }
    }
}

