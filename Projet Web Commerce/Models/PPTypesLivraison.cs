using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPTypesLivraison
    {
        public int CodeLivraison { get; set; }
        public string Description { get; set; }
        [InverseProperty("PPTypesLivraison")]
        public virtual ICollection<PPLivraison>? PPLivraison { get; set; }
        [InverseProperty("PPTypesLivraison")]
        public virtual ICollection<PPPoidsLivraisons>? PPPoidsLivraisons { get; set; }
    }
}
