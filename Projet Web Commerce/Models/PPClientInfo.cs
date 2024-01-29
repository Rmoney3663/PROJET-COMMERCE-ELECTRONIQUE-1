using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPClientInfo
    {
        public int NoClient { get; set; }

        public string AdresseEmail { get; set; }
        public string MotDePasse { get; set; }

        public string Nom { get; set; }
        public string Prenom { get; set; }

        [ForeignKey("NoClient")]
        [InverseProperty("PPClientInfo")]
        public virtual PPClients? PPClients { get; set; }
    }
}
