using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPCategories
    {
        public int NoCategorie { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        [InverseProperty("PPCategories")]
        public virtual ICollection<PPProduits>? PPProduits { get; set; }
    }
}
