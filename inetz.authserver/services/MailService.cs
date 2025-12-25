using System.Net;
using System.Net.Mail;

namespace inetz.authserver.services
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;
        public MailService ( IConfiguration configuration )
        {
            _configuration = configuration;
           
        }
        public async Task SendEmailAsync ( SendEmailRequest sendEmailRequest )
        {
            try
            {
                MailMessage message = new MailMessage
                {
                    From = new MailAddress(_configuration ["EmailOptions:SenderEmail"] ?? string.Empty),
                    Subject = sendEmailRequest.Subject,
                    Body = sendEmailRequest.Body,

                };

                message.To.Add(sendEmailRequest.Recipient);

                using var smtpClient = new SmtpClient();
                smtpClient.Host = _configuration ["EmailOptions:Host"] ?? string.Empty;
                smtpClient.Port = Convert.ToInt32(_configuration ["EmailOptions:Port"] ?? "0");
                smtpClient.Credentials = new NetworkCredential
                {
                    UserName = _configuration ["EmailOptions:Login"],
                    Password = _configuration ["EmailOptions:Password"],
                };

                smtpClient.EnableSsl = false;

                await smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                //_logger.LogError("SendEmailAsync failed {0}", ex.Message);
            }



        }
    }
}
