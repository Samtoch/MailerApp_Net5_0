using MailerApp_Net5_0.Interfaces;
using MailerApp_Net5_0.Models;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MailerApp_Net5_0.Services
{
    public class MailService : IMailService
    {
        private readonly string SMTP, SMTPUser, Sender, Password, UseDefaultCredentials;
        private static Logger log = LogManager.GetCurrentClassLogger();
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;
        private readonly bool SSL;
        private readonly int Port;
        public MailService(IConfiguration configuration, IHostingEnvironment environment)
        {
            _environment = environment;
            _configuration = configuration;
            SMTP = configuration.GetSection("SMTP").Value;
            Port = Convert.ToInt32(configuration.GetSection("Port").Value);
            SSL = Convert.ToBoolean(configuration.GetSection("SSL").Value);
            SMTPUser = configuration.GetSection("SMTPUser").Value;
            Sender = configuration.GetSection("Sender").Value;
            Password = configuration.GetSection("Password").Value;
            UseDefaultCredentials = configuration.GetSection("UseDefaultCredentials").Value;
        }

        public Task<string> SendMail(Email email)
        {
            log.Info("SendMail Request by: " + email.Recipient);
            string wwwPath = _environment.WebRootPath;
            //string contentPath = _environment.ContentRootPath;
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(Sender));
            message.To.Add(MailboxAddress.Parse(email.Recipient));
            message.Subject = email.Subject;

            var builder = new BodyBuilder();
            //builder.HtmlBody = email.Body;
            //builder.TextBody = @"Hey Girl,

            //What are you up to this weekend? Monica is throwing one of her parties on
            //Saturday and I was hoping you could make it.

            //Will you be my +1?

            //-- Tochi
            //";

            var image = builder.LinkedResources.Add(wwwPath + @"\\img\\estateOne.jpg");
            //var image = builder.LinkedResources.Add(@"C:\\Inetpub\\vhosts\\propertynovel.com\\\wwwroot\\img\\estateOne.jpg");
            //var image = builder.LinkedResources.Add(@"C:\Users\Joey\Documents\Selfies\selfie.jpg");
            image.ContentId = MimeUtils.GenerateMessageId();
            builder.HtmlBody = string.Format(@"<div style='border: 2px solid green; padding: 20px; border-radius: 25px; height: 90%; width: 75%;'>
	        <br />
	        <div style=''>
		        <img alt='PropertyNovel.com Logo' style='border-radius: 10px; width: 120px; height: 60px' src=""cid:{0}"">
	        </div>
	
		        <p>Hey Alice,
		        <br />
		        <p>What are you up to this weekend? Monica is throwing one of her parties on
		        Saturday and I was hoping you could make it.
		        <br />
		        <p>Will you be my +1?
		        <br />
		        <p>Tochi
		
		        <br /><br />
	        <a href='https://propertynovel.com/Home/Login' style='border-radius: 20px 8px; color: white; padding: 15px 32px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px; margin: 4px 2px; cursor: pointer; background-color: green;'>Click to View More</a>
            </div>", image.ContentId);

            // Set the html version of the message text
            //builder.HtmlBody = string.Format(@"<p>Hey Alice,<br>
            //<p>What are you up to this weekend? Monica is throwing one of her parties on
            //Saturday and I was hoping you could make it.<br>
            //<p>Will you be my +1?<br>
            //<p>-- Tochi<br>
            //<center><img src=""cid:{0}""></center>", image.ContentId);

            // attachments
            


            if (email.Attachement != null)
            {
                byte[] fileByte;
                foreach (var file in email.Attachement)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileByte = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileByte);
                    }
                }
            }

            var img = builder.LinkedResources.Add(wwwPath + @"\\img\\estateOne.jpg");
            var pdf = builder.LinkedResources.Add(wwwPath + @"\\pdf\\testpdfdoc.pdf");
            builder.Attachments.Add(pdf);
            builder.Attachments.Add(img);
            message.Body = builder.ToMessageBody();
            

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                //smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                //smtp.Connect(SMTP, Port, SSL);
                //smtp.Connect(SMTP, Port, SecureSocketOptions.StartTls);
                //smtp.Authenticate(Sender, Password);
                //smtp.Send(message);
                
                smtp.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                smtp.Connect(SMTP, Port, SecureSocketOptions.Auto);
                smtp.Authenticate(SMTPUser, Password);
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                log.Error("SendMail Error for: " + email.Recipient + "\r\n" + ex);
                return Task.FromResult(ex.Message);
            }

            smtp.Disconnect(true);
            return Task.FromResult("Success");
        }
    }
}
