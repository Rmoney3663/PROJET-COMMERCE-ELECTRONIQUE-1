using Org.BouncyCastle.Asn1.Mozilla;
using Projet_Web_Commerce.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projet_Web_Commerce.Models
{
    public class PPMessages
    {
        [Key]
        public int NoMessage { get; set; }
        public string Sujet { get; set; }
        public string Message { get; set; }
        public string Auteur { get; set; }
        public int TypeMessage { get; set; }
        public string PieceJointe { get; set; }
        public string Transfemetteur { get; set; }

        // Navigation property for recipients
        public ICollection<PPDestinatairesMessage> Destinataires { get; set; }

        [ForeignKey("Auteur")]
        public virtual Utilisateur AuteurUser { get; set; }
    }
}
