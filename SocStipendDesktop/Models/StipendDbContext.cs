using Microsoft.EntityFrameworkCore;
using System;

namespace SocStipendDesktop.Models
{
    public class StipendDbContext : DbContext
    {
        public DbSet<Student> Student { get; set; }
        public DbSet<Stipend> Stipend { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=SocStipendDB.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stipend>()
                .HasOne(s => s.Student)
                .WithMany(s => s.Stipend) // Добавить навигационное свойство
                .HasForeignKey(s => s.StudentId);
        }
    }
}