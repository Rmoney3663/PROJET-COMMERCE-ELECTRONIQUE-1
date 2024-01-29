using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPProduitsInfo
    {
        public int NoProduit { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }
        public decimal PrixDemande { get; set; }
        public bool Disponibilite { get; set; }
        public decimal  Poids { get; set; }
        [ForeignKey("NoProduit")]
        [InverseProperty("PPProduitsInfo")]
        public virtual PPProduits? PPProduits { get; set; }
    }
}
