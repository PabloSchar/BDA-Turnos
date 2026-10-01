using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace BDA_Turnos.Models
{
    public static class ServicioEmail
    {
        private const string RemitenteEmail = "StockyVentas23@gmail.com";
        private const string ClaveSmtp = "xierkcskwthrabcu";

        public static void EnviarCorreo(string destinatario, string nombreDestinatario, string asunto, string cuerpoHtml, string textoPlano = "", string rutaAdjunto = null)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Sistema de Gestión", RemitenteEmail));
            email.To.Add(new MailboxAddress(nombreDestinatario, destinatario));
            email.Subject = asunto;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = cuerpoHtml,
                TextBody = textoPlano
            };

            if (!string.IsNullOrEmpty(rutaAdjunto) && File.Exists(rutaAdjunto))
            {
                bodyBuilder.Attachments.Add(rutaAdjunto);
            }

            email.Body = bodyBuilder.ToMessageBody();

            using (var smtp = new SmtpClient())
            {
                try
                {
                    smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    smtp.Authenticate(RemitenteEmail, ClaveSmtp);
                    smtp.Send(email);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al enviar correo: {ex.Message}");
                }
                finally
                {
                    smtp.Disconnect(true);
                }
            }
        }
    }
}
