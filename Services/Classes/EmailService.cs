using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Services.Classes
{
    public class EmailService
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public int Timeout { get; set; }

        public EmailService()
        {
            Host = "smtp-mail.outlook.com";
            Port = 587;
            EnableSsl = true;
            Timeout = 10000;
        }
        public EmailService(string host, int port, bool enableSsl, int timeout)
        {
            Host = host;
            Port = port;
            EnableSsl = enableSsl;
            Timeout = timeout;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using (SmtpClient client = new SmtpClient(Host, Port))
            {
                client.EnableSsl = EnableSsl;
                client.Timeout = Timeout;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;

                client.Credentials = new NetworkCredential("conversationoverflow@outlook.com", "#################");

                MailMessage msg = new MailMessage("conversationoverflow@outlook.com", email, subject, message);
                msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                await client.SendMailAsync(msg);
            }
        }
    }
}