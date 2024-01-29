using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPProduits
    {
        public int NoProduit { get; set; }
        public int NoVendeur { get; set; }
        public int NoCategorie { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime DateMAJ { get; set; }
        [InverseProperty("PPProduits")]
        public virtual ICollection<PPProduitsInfo>? PPProduitsInfo { get; set; }
        [InverseProperty("PPProduits")]
        public virtual ICollection<PPProduitsVente>? PPProduitsVente { get; set; }
        [InverseProperty("PPProduits")]
        public virtual ICollection<PPArticlesEnPanier>? PPArticlesEnPanier { get; set; }
        [InverseProperty("PPProduits")]
        public virtual ICollection<PPDetailsCommandes>? PPDetailsCommandes { get; set; }
        [ForeignKey("NoVendeur")]
        [InverseProperty("PPProduits")]
        public virtual PPVendeurs? PPVendeurs { get; set; }
        [ForeignKey("NoCategorie")]
        [InverseProperty("PPProduits")]
        public virtual PPCategories? PPCategories { get; set; }
    }
}
