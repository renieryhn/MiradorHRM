using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace PlanillaPM.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task EnviarEmail(string sDestinatario, string sAsunto, string sMensaje)
        {
            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(_configuration["SMTP:User"], _configuration["SMTP:Pwd"]);
                    smtpClient.EnableSsl = bool.Parse(_configuration["SMTP:EnableSSL"]);
                    smtpClient.Port = int.Parse(_configuration["SMTP:Port"]);
                    smtpClient.Host = _configuration["SMTP:Server"];

                    using (var mail = new MailMessage())
                    {
                        mail.From = new MailAddress(_configuration["SMTP:UserEmail"]);
                        mail.To.Add(sDestinatario);
                        mail.Subject = sAsunto;
                        mail.Body = sMensaje;
                        mail.IsBodyHtml = true;
                        mail.Priority = MailPriority.High;

                        smtpClient.Send(mail);
                    }

                    return Task.CompletedTask;
                }
            }
            catch (SmtpException ex)
            {
                // Manejar la excepción aquí o lanzarla nuevamente
                throw ex;
            }
        }
    }
}