using System.Net;
using System.Net.Mail;

namespace Services.Classes
{
    public static class Sender
    {
        private static string email = "########################";
        private static string password = "#########################";
        private static string host = "smtp-mail.outlook.com";
        private static int port = 587;
        private static bool enableSsl = true;
        private static int timeout = 10000;

        public static void Send(string tomail, string subject, string body)
        {
            SmtpClient client = new SmtpClient(host, port);
            client.EnableSsl = enableSsl;
            client.Timeout = timeout;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            client.Credentials = new NetworkCredential(email, password);

            MailMessage msg = new MailMessage(email, tomail, subject, body);
            msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(msg);
        }
    }
}
