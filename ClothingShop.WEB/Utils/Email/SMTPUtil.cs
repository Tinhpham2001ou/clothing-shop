using System.Net.Mail;
using System.Net;

namespace ClothingShop.WEB.Utils.Email
{
    public class SMTPUtil : IEmail
    {
        private string _username { get; set; }
        private string _password { get; set; }
        private int _port { get; set; }
        private string _host { get; set; }
        private bool _enableSsl { get; set; }
        private bool _useDefaultCredentials { get; set; }
        private bool _isBodyHtml { get; set; }

        public async Task ConfigMailAsync(IConfiguration Configuration)
        {
            _username = Configuration["Email:Username"];
            _password = Configuration["Email:Password"];
            _port = int.Parse(Configuration["Email:Port"]);
            _host = Configuration["Email:Host"];
            _enableSsl = bool.Parse(Configuration["Email:EnableSsl"]);
            _useDefaultCredentials = bool.Parse(Configuration["Email:UseDefaultCredentials"]);
            _isBodyHtml = bool.Parse(Configuration["Email:IsBodyHtml"]);
        }

        public async Task SendAsync(string toEmail, string subject, string content)
        {
            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            message.From = new MailAddress(_username, "5s Fahion");
            message.To.Add(new MailAddress(toEmail));
            message.Subject = subject;
            message.IsBodyHtml = _isBodyHtml;
            message.Body = content;
            smtp.Port = _port;
            smtp.Host = _host;
            smtp.EnableSsl = _enableSsl;
            smtp.UseDefaultCredentials = _useDefaultCredentials;
            smtp.Credentials = new NetworkCredential(_username, _password);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(message);
        }
    }
}
