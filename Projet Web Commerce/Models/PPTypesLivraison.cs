using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPTypesLivraison
    {
        [Key]
        public int CodeLivraison { get; set; }
        public string Description { get; set; }
        [InverseProperty("PPTypesLivraison")]
        public virtual ICollection<PPCommandes>? PPCommandes { get; set; }
        [InverseProperty("PPTypesLivraison")]
        public virtual ICollection<PPPoidsLivraisons>? PPPoidsLivraisons { get; set; }
    }
}
