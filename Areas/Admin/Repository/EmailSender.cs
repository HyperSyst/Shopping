using System.Net;
using System.Net.Mail;

namespace Shopping.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, //bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("conghuycr@gmail.com", "gvphlbhzuigoxypl")
            };

            return client.SendMailAsync(
                new MailMessage(from: "conghuycr@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}
