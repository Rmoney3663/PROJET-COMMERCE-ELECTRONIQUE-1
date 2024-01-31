namespace Projet_Web_Commerce
{
    using MimeKit;
    using MailKit.Net.Smtp;
    using MailKit.Security;

    public class Methodes
    {
        public static async Task<bool> envoyerCourriel(string email, string sujet, string message)
        {
            try
            {
                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress("Les Petites Puces", "robotcupcake69@outlook.com"));
                mimeMessage.To.Add(new MailboxAddress("", email));
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
    }
}
