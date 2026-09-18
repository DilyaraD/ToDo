using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ToDo.Services
{
    public class EmailService
    {
        public async Task SendCodeAsync(string toEmail, string code)
        {
            await Task.Run(() =>
            {
                MailAddress from = new MailAddress(EmailSecrets.SenderEmail, "ToDo App");
                MailAddress to = new MailAddress(toEmail);
                MailMessage m = new MailMessage(from, to);
                m.Subject = "Password recovery";
                m.Body = "<h1>Code: " + code + "</h1>";
                m.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);
                smtp.Credentials = new NetworkCredential(
                    EmailSecrets.SenderEmail,
                    EmailSecrets.SenderPassword);
                smtp.EnableSsl = true;
                smtp.Send(m);
            });
        }
    }
}