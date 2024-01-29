using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPVendeurs
    {
        public int NoVendeur { get; set; }

        public string NomAffaires { get; set; }
        public int PoidsMaxLivraison { get; set; }

        public bool Taxes { get; set; }
        public string Prenom { get; set; }
        public double Pourcentage { get; set; }
        public string Configuration { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime DateMAJ { get; set; }
        public int Statut { get; set; }
        [InverseProperty("PPVendeurs")]
        public virtual ICollection<PPVendeurAdresse>? PPVendeurAdresse { get; set; }
        [InverseProperty("PPVendeurs")]
        public virtual ICollection<PPVendeursInfo>? PPVendeursInfo { get; set; }
        [InverseProperty("PPVendeurs")]
        public virtual ICollection<TelephoneVendeurs>? TelephoneVendeurs { get; set; }
        [InverseProperty("PPVendeurs")]
        public virtual ICollection<PPVendeursClients>? PPVendeursClients { get; set; }
    }
}
