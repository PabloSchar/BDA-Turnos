using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BDA_Turnos.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Nombre_Usuario { get; set; }
        public string Clave { get; set; }
        public string EMail { get; set; }
        public string HashContraseña(string clavePlana)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(clavePlana);

            return hash;
        }

        public string CrearClaveAleatoria()
        {
            int longitud = 10;
            const string caracteresValidos = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%*";

            StringBuilder claveGenerada = new StringBuilder();

            for (int i = 0; i < longitud; i++)
            {
                int indiceAleatorio = RandomNumberGenerator.GetInt32(caracteresValidos.Length);

                claveGenerada.Append(caracteresValidos[indiceAleatorio]);
            }

            string clave = claveGenerada.ToString();

            return clave;
        }

        public bool ValidarClave(string clave)
        {
            bool iguales = BCrypt.Net.BCrypt.Verify(clave, Clave);

            return iguales;
        }

        public void NotificarCredencialesEmail(string nombreUsuario, string emailDestino, string clavePlana, bool esNuevoUsuario = false)
        {
            string asunto = esNuevoUsuario
                ? "Bienvenido al sistema - Tus credenciales de acceso"
                : "Reseteo de contraseña exitoso";

            string tituloHtml = esNuevoUsuario
                ? $"Bienvenido, {nombreUsuario}"
                : $"Hola, {nombreUsuario}";

            string mensajeHtml = esNuevoUsuario
                ? "Tu cuenta ha sido creada exitosamente. A continuación, te enviamos tus datos de acceso:"
                : "Tu clave ha sido reseteada según lo solicitado. A continuación, te enviamos tus nuevos datos de acceso:";

            string html = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; color: #333;'>
                    <h2 style='color: #2c3e50;'>{tituloHtml}</h2>
                    <p>{mensajeHtml}</p>
                    <div style='background-color: #f4f4f4; padding: 15px; border-radius: 5px; border-left: 4px solid #3498db; margin: 15px 0;'>
                        <p style='margin: 5px 0;'><b>Cuenta / Usuario:</b> {nombreUsuario}</p>
                        <p style='margin: 5px 0;'><b>Clave:</b> {clavePlana}</p>
                    </div>
                    <p><i>Por razones de seguridad, te recomendamos cambiar esta clave la próxima vez que inicies sesión.</i></p>
                </div>";

            ServicioEmail.EnviarCorreo(
                destinatario: emailDestino,
                nombreDestinatario: nombreUsuario,
                asunto: asunto,
                cuerpoHtml: html
            );
        }
    }
}
