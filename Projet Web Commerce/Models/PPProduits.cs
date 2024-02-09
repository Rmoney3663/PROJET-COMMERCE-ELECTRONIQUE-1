using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPProduits
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NoProduit { get; set; }
        public int NoVendeur { get; set; }
        public int NoCategorie { get; set; }
        public DateTime DateCreation { get; set; }
        public int NombreItems { get; set; }
        public DateTime DateMAJ { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal PrixDemande { get; set; }
        public bool Disponibilite { get; set; }
        [Column(TypeName = "numeric(8,1)")]
        public decimal Poids { get; set; }
        public DateTime? DateVente { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal? PrixVente { get; set; }

        [InverseProperty("PPProduits")]
        public virtual ICollection<PPArticlesEnPanier>? PPArticlesEnPanier { get; set; }
        [InverseProperty("PPProduits")]
        public virtual ICollection<PPDetailsCommandes>? PPDetailsCommandes { get; set; }
        [InverseProperty("PPProduits")]
        public virtual ICollection<PPEvaluations>? PPEvaluations { get; set; }

        [ForeignKey("NoVendeur")]
        [InverseProperty("PPProduits")]
        public virtual PPVendeurs? PPVendeurs { get; set; }
        [ForeignKey("NoCategorie")]
        [InverseProperty("PPProduits")]
        public virtual PPCategories? PPCategories { get; set; }

    }
}
