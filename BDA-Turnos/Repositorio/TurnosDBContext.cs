using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDA_Turnos.Models;
using Microsoft.EntityFrameworkCore;

namespace BDA_Turnos.Repositorio
{
    public class TurnosDBContext : DbContext
    {
        public TurnosDBContext(DbContextOptions<TurnosDBContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<EstadoTurno> EstadosTurnos { get; set; }
        public DbSet<Turno> Turnos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuarios");
                entity.HasKey(e => e.UsuarioId).HasName("pk_usuarios");

                entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(150).IsRequired();
                entity.Property(e => e.Telefono).HasColumnName("telefono").HasMaxLength(30);
                entity.Property(e => e.Nombre_Usuario).HasColumnName("nombre_usuario").HasMaxLength(50).IsRequired();
                entity.Property(e => e.Clave).HasColumnName("clave").HasMaxLength(255).IsRequired();
                entity.Property(e => e.EMail).HasColumnName("email").HasMaxLength(150).IsRequired();

                entity.HasIndex(e => e.Nombre_Usuario).IsUnique();
                entity.HasIndex(e => e.EMail).IsUnique();
            });

            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.ToTable("pacientes");
                entity.HasKey(e => e.UsuarioId).HasName("pk_pacientes");

                entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
                entity.Property(e => e.DNI).HasColumnName("dni").IsRequired();
                entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento").IsRequired();
                entity.Property(e => e.Direccion).HasColumnName("direccion").HasMaxLength(200);

                entity.HasIndex(e => e.DNI).IsUnique();

                entity.HasOne(e => e.Usuario)
                      .WithOne()
                      .HasForeignKey<Paciente>(e => e.UsuarioId)
                      .HasConstraintName("fk_pacientes_usuarios")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Medico>(entity =>
            {
                entity.ToTable("medicos");
                entity.HasKey(e => e.UsuarioId).HasName("pk_medicos");

                entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
                entity.Property(e => e.Matricula).HasColumnName("matricula").IsRequired();

                entity.HasIndex(e => e.Matricula).IsUnique();

                entity.HasOne(e => e.Usuario)
                      .WithOne()
                      .HasForeignKey<Medico>(e => e.UsuarioId)
                      .HasConstraintName("fk_medicos_usuarios")
                      .OnDelete(DeleteBehavior.Cascade);

                // Relación N:M Medico <-> Especialidad con la tabla intermedia
                entity.HasMany(e => e.Especialidades)
                      .WithMany()
                      .UsingEntity<Dictionary<string, object>>(
                          "medicos_especialidades",
                          r => r.HasOne<Especialidad>().WithMany().HasForeignKey("especialidad_id").HasConstraintName("fk_med_esp_especialidades").OnDelete(DeleteBehavior.Restrict),
                          l => l.HasOne<Medico>().WithMany().HasForeignKey("medico_id").HasConstraintName("fk_med_esp_medicos").OnDelete(DeleteBehavior.Cascade),
                          je =>
                          {
                              je.ToTable("medicos_especialidades");
                              je.HasKey("medico_id", "especialidad_id").HasName("pk_medicos_especialidades");
                          }
                      );
            });

            modelBuilder.Entity<Especialidad>(entity =>
            {
                entity.ToTable("especialidades");
                entity.HasKey(e => e.EspecialidadId).HasName("pk_especialidades");

                entity.Property(e => e.EspecialidadId).HasColumnName("especialidad_id");
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();

                entity.HasIndex(e => e.Nombre).IsUnique();
            });

            modelBuilder.Entity<EstadoTurno>(entity =>
            {
                entity.ToTable("estados_turnos");
                entity.HasKey(e => e.EstadoTurnoId).HasName("pk_estados_turnos");

                entity.Property(e => e.EstadoTurnoId).HasColumnName("estado_turno_id");
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();

                entity.HasIndex(e => e.Nombre).IsUnique();
            });

            modelBuilder.Entity<Turno>(entity =>
            {
                entity.ToTable("turnos");
                entity.HasKey(e => e.TurnoId).HasName("pk_turnos");

                entity.Property(e => e.TurnoId).HasColumnName("turno_id");
                entity.Property(e => e.PacienteId).HasColumnName("paciente_id");
                entity.Property(e => e.MedicoId).HasColumnName("medico_id");
                entity.Property(e => e.EspecialidadId).HasColumnName("especialidad_id");
                entity.Property(e => e.EstadoTurnoId).HasColumnName("estado_turno_id");
                entity.Property(e => e.Fecha).HasColumnName("fecha").IsRequired();
                entity.Property(e => e.Hora).HasColumnName("hora").IsRequired();

                entity.HasOne(e => e.Paciente)
                      .WithMany()
                      .HasForeignKey(e => e.PacienteId)
                      .HasConstraintName("fk_turnos_pacientes")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Medico)
                      .WithMany()
                      .HasForeignKey(e => e.MedicoId)
                      .HasConstraintName("fk_turnos_medicos")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Especialidad)
                      .WithMany()
                      .HasForeignKey(e => e.EspecialidadId)
                      .HasConstraintName("fk_turnos_especialidades")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EstadoTurno)
                      .WithMany()
                      .HasForeignKey(e => e.EstadoTurnoId)
                      .HasConstraintName("fk_turnos_estados")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.MedicoId, e.Fecha, e.Hora }).IsUnique().HasDatabaseName("uq_medico_fecha_hora");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
