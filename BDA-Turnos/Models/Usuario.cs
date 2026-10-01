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

        public bool ValidarClave(string clave)
        {
            bool iguales = BCrypt.Net.BCrypt.Verify(clave, Clave);

            return iguales;
        }
    }
}
