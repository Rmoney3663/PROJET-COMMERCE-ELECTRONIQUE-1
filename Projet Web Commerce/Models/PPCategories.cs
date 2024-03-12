using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPCategories
    {
        [Key]
        public int NoCategorie { get; set; }

        [Required(ErrorMessage = "Le champ Description est requis.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Le champ Détails est requis.")]
        public string Details { get; set; }

        [InverseProperty("PPCategories")]
        public virtual ICollection<PPProduits>? PPProduits { get; set; }
    }
}
