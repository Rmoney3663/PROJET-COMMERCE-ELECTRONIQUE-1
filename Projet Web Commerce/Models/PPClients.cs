using Projet_Web_Commerce.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPClients
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NoClient { get; set; }

        public DateTime DateCreation { get; set; }

        public DateTime? DateMAJ { get; set; }

        public int? NbConnexions { get; set; }

        public DateTime? DateDerniereConnexion { get; set; }

        // 0: Peut pas log in, doit confirmer son courriel
        // 1: Peut log in (normal/actif)
        // 2: Inactif
        public int? Statut { get; set; }

        public string AdresseEmail { get; set; }
        public string MotDePasse { get; set; }

        public string? Nom { get; set; }
        public string? Prenom { get; set; }

        public string? Rue { get; set; }

        public string? Ville { get; set; }

        [Column(TypeName = "char(2)")]
        public string? NoProvince { get; set; }

        public string? CodePostal { get; set; }
        public string? Pays { get; set; }

        public string? Tel1 { get; set; }
        public string? Tel2 { get; set; }
        public string IdUtilisateur { get; set; }

        [InverseProperty("PPClients")]
        public virtual ICollection<PPVendeursClients>? PPVendeursClients { get; set; }
        [InverseProperty("PPClients")]
        public virtual ICollection<PPArticlesEnPanier>? PPArticlesEnPanier { get; set; }
        [InverseProperty("PPClients")]
        public virtual ICollection<PPCommandes>? PPCommandes { get; set; }
        [InverseProperty("PPClients")]
        public virtual ICollection<PPEvaluations>? PPEvaluations { get; set; }

        [ForeignKey("NoProvince")]
        [InverseProperty("PPClients")]
        public virtual Province? Province { get; set; }

        [ForeignKey("IdUtilisateur")]
        [InverseProperty("PPClients")]
        public virtual Utilisateur? Utilisateur { get; set; }
    }
}