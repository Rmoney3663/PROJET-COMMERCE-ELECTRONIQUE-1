namespace Projet_Web_Commerce
{
    using MimeKit;
    using MailKit.Net.Smtp;
    using MailKit.Security;

    public class Methodes
    {
        public static async Task<bool> envoyerCourriel(string sujet, string message, string destinataire, string auteur = null)
        {
            try
            {
                var mimeMessage = new MimeMessage();

                string fromAddress = (auteur == null) ? "robotcupcake69@outlook.com" : auteur;
                mimeMessage.From.Add(new MailboxAddress("Les Petites Puces", fromAddress));

                string[] destinataires = destinataire.Split(',');

                foreach (var dest in destinataires)
                {
                    mimeMessage.To.Add(new MailboxAddress("", dest.Trim()));
                }
                mimeMessage.Subject = sujet;
                mimeMessage.Body = new TextPart("html") { Text = message };

                var client = new SmtpClient();

                client.Connect("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate("robotcupcake69@outlook.com", "tamere123");
                client.Send(mimeMessage);
                client.Disconnect(true);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        public static decimal pourcentageTaxes(bool taxe, string province)
        {
            if (taxe == true)
            {
                if (province == "QC")
                    return Convert.ToDecimal(14.975);
                else
                    return Convert.ToDecimal(5.00);
            }

            return Convert.ToDecimal(0.00);
        }
    }
}
