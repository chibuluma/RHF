namespace RHF.API.Services;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using MimeKit;

public class MailService : IMailService
{
    private readonly MailSettings _mailSettings;
    public MailService(IOptions<MailSettings> options)
    {
        _mailSettings = options.Value;
    }

    public bool SendMail(MailData mailData)
    {
        try
        {
            using (MimeMessage emailMessage = new MimeMessage())
            {
                MailboxAddress emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
                emailMessage.From.Add(emailFrom);
                MailboxAddress emailTo = new MailboxAddress(mailData.EmailToName, mailData.EmailToId);
                emailMessage.To.Add(emailTo);

                emailMessage.Cc.Add(new MailboxAddress("Cc Receiver", _mailSettings.SenderCCEmail));
                // emailMessage.Bcc.Add(new MailboxAddress("Bcc Receiver", "bcc@example.com"));

                emailMessage.Subject = mailData.EmailSubject;

                BodyBuilder emailBodyBuilder = new BodyBuilder();
                emailBodyBuilder.HtmlBody = mailData.EmailBody;

                emailMessage.Body = emailBodyBuilder.ToMessageBody();
                //this is the SmtpClient from the Mailkit.Net.Smtp namespace, not the System.Net.Mail one
                using (MailKit.Net.Smtp.SmtpClient mailClient = new MailKit.Net.Smtp.SmtpClient())
                {
                    mailClient.Connect(_mailSettings.Server, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                    mailClient.Authenticate(_mailSettings.UserName, _mailSettings.Password);
                    mailClient.Send(emailMessage);
                    mailClient.Disconnect(true);
                }

                return true;
            }

        }
        catch (Exception ex)
        {
            throw;
        }
    }
}