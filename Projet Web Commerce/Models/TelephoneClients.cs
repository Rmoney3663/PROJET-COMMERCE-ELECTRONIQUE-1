using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class TelephoneClients
    {
        public int NoClient { get; set; }
        public string Tel { get; set; }
        [ForeignKey("NoClient")]
        [InverseProperty("TelephoneClients")]
        public virtual PPClients? PPClients { get; set; }
    }
}
