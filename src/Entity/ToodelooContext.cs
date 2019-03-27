using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Entity.Entity
{
    public partial class toodelooContext : DbContext
    {
        private string connString = "";

        public toodelooContext()
        {
        }

        public toodelooContext( string connectionString ) {
            this.connString = connectionString;
        }

        public toodelooContext(DbContextOptions<toodelooContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Todo> Todo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(this.connString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity<Todo>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });
        }
    }
}
