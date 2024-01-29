using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPClients
    {
        public int NoClient { get; set; }

        public DateTime DateCreation { get; set; }
        public DateTime DateMAJ { get; set; }

        public int NbConnexions { get; set; }
        public DateTime DateDerniereConnexion { get; set; }
        public int Statut { get; set; }
        [InverseProperty("PPClients")]
        public virtual ICollection<PPClientInfo>? PPClientInfo { get; set; }
        [InverseProperty("PPClients")]
        public virtual ICollection<TelephoneClients>? TelephoneClients { get; set; }
        [InverseProperty("PPClients")]
        public virtual ICollection<PPClientAdresse>? PPClientAdresse { get; set; }
        [InverseProperty("PPClients")]
        public virtual ICollection<PPVendeursClients>? PPVendeursClients { get; set; }
        [InverseProperty("PPClients")]
        public virtual ICollection<PPPanier>? PPPanier { get; set; }
        [InverseProperty("PPClients")]
        public virtual ICollection<PPCommandes>? PPCommandes { get; set; }
    }
}
