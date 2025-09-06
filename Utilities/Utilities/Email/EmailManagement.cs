using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Email;

#region Interface
public interface IEmailManagement
{
    Task SendEmailAsync(EmailModel email);
}
#endregion

#region Class
public class EmailManagement : IEmailManagement
{
    public async Task SendEmailAsync(EmailModel email)
    {
        try
        {
            MailMessage mail = new MailMessage()
            {
                From = new MailAddress("support@mail.com", "نام نمایشی"),
                To = { email.To },
                Subject = email.Subject,
                Body = email.Body,
                IsBodyHtml = true,
            };
            /// <summary>
            /// Smtp Ports
            /// </summary>
            // Not-Encrypted 25,
            // Secure Tls 587
            // Secure SSL 465
            SmtpClient smtpServer = new SmtpClient("host", 587) // Host => forExample webmail.codeyad.com
            {
                Credentials = new System.Net.NetworkCredential("userName", "Password"), // UserName == Email
                EnableSsl = true
            };
            smtpServer.Send(mail);
            await Task.CompletedTask;
        }
        catch (Exception err)
        {

        }
    }
}
#endregion


#region Model
public class EmailModel
{
    public EmailModel(string to, string subject, string body)
    {
        To = to;
        Subject = subject;
        Body = body;
    }
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}
#endregion