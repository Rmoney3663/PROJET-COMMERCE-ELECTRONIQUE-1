using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class TelephoneClients
    {
        [Key]
        public int Id { get; set; }
        public int NoClient { get; set; }
        public string Tel { get; set; }
        [ForeignKey("NoClient")]
        [InverseProperty("TelephoneClients")]
        public virtual PPClients? PPClients { get; set; }
    }
}
