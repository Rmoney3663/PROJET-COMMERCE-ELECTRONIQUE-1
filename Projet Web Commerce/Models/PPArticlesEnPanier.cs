using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPArticlesEnPanier
    {
        [Column(TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NoPanier { get; set; }

        public int NoClient { get; set; }

        public int NoVendeur { get; set; }

        public int NoProduit { get; set; }

        public DateTime DateCreation { get; set; }

        public int NbItems { get; set; }


        [ForeignKey("NoVendeur")]
        [InverseProperty("PPArticlesEnPanier")]
        public virtual PPVendeurs? PPVendeurs { get; set; }

        [ForeignKey("NoClient")]
        [InverseProperty("PPArticlesEnPanier")]
        public virtual PPClients? PPClients { get; set; }

        [ForeignKey("NoProduit")]
        [InverseProperty("PPArticlesEnPanier")]
        public virtual PPProduits? PPProduits { get; set; }
    }
}
