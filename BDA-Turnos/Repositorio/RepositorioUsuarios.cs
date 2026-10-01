using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using BDA_Turnos.Models;

namespace BDA_Turnos.Repositorio
{
    public class RepositorioUsuarios
    {
        private readonly TurnosDBContext db;
        public RepositorioUsuarios(TurnosDBContext contextoCompartido)
        {
            db = contextoCompartido;
        }

        public Usuario? ObtenerUsuario(int id)
        {
            return db.Usuarios.FirstOrDefault(p => p.UsuarioId == id);
        }

        public async Task<Usuario?> ObtenerUsuarioAsync(int id)
        {
            return await db.Usuarios.FirstOrDefaultAsync(p => p.UsuarioId == id);
        }

        public Usuario? RecuperarUsuario(string nombreUsuario)
        {
            return db.Usuarios.FirstOrDefault(p => p.Nombre_Usuario == nombreUsuario);
        }

        public async Task<Usuario?> RecuperarUsuarioAsync(string nombreUsuario)
        {
            return await db.Usuarios.FirstOrDefaultAsync(p => p.Nombre_Usuario == nombreUsuario);
        }
    }
}
