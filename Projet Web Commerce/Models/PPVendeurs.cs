using Projet_Web_Commerce.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPVendeurs
    {
        [Key]
        public int NoVendeur { get; set; }

        public string NomAffaires { get; set; }

        public int PoidsMaxLivraison { get; set; }

        public bool Taxes { get; set; }

        [Column(TypeName = "decimal(4,2)")]
        public decimal Pourcentage { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal LivraisonGratuite { get; set; }

        public string Configuration { get; set; }

        public DateTime DateCreation { get; set; }

        public DateTime DateMAJ { get; set; }

        public int Statut { get; set; }

        public string AdresseEmail { get; set; }
        public string MotDePasse { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Rue { get; set; }

        public string Ville { get; set; }

        [Column(TypeName = "char(2)")]
        public string NoProvince { get; set; }

        public string CodePostal { get; set; }
        public string Pays { get; set; }
        public string IdUtilisateur { get; set; }

        [InverseProperty("PPVendeurs")]
        public virtual ICollection<TelephoneVendeurs>? TelephoneVendeurs { get; set; }
        [InverseProperty("PPVendeurs")]
        public virtual ICollection<PPVendeursClients>? PPVendeursClients { get; set; }
        [InverseProperty("PPVendeurs")]
        public virtual ICollection<PPArticlesEnPanier>? PPArticlesEnPanier { get; set; }

        [InverseProperty("PPVendeurs")]
        public virtual ICollection<PPCommandes>? PPCommandes { get; set; }

        [InverseProperty("PPVendeurs")]
        public virtual ICollection<PPProduits>? PPProduits { get; set; }

        [ForeignKey("NoProvince")]
        [InverseProperty("PPVendeurs")]
        public virtual Province? Province { get; set; }

        [ForeignKey("IdUtilisateur")]
        [InverseProperty("PPVendeurs")]
        public virtual Utilisateur? Utilisateur { get; set; }
    }
}
