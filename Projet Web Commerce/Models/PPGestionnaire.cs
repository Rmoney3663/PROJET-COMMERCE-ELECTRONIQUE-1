using Projet_Web_Commerce.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPGestionnaire
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NoGestionnaire { get; set; }      
        public DateTime DateCreation { get; set; }
        public string AdresseEmail { get; set; }
        public string MotDePasse { get; set; }
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public string IdUtilisateur { get; set; }

        [ForeignKey("IdUtilisateur")]
        [InverseProperty("PPGestionnaire")]
        public virtual Utilisateur? Utilisateur { get; set; }
    }
}
