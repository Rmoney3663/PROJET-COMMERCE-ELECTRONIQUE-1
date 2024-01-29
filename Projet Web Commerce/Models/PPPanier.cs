using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPPanier
    {
        public int NoClient { get; set; }
        public int NoPanier { get; set; }

        [InverseProperty("PPPanier")]
        public virtual ICollection<PPArticlesEnPanier>? PPArticlesEnPanier { get; set; }
        [ForeignKey("NoClient")]
        [InverseProperty("PPPanier")]
        public virtual PPClients? PPClients { get; set; }
    }
}
