using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Toodeloo.Entity
{
    public partial class ToodelooContext : DbContext
    {
        private string connString = "";

        public ToodelooContext()
        {
            this.connString = "Host=localhost;Database=toodeloo;Username=postgres;Password=postgres";
        }

        public ToodelooContext( string connectionString ) {
            this.connString = connectionString;
        }

        public ToodelooContext(DbContextOptions<ToodelooContext> options)
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
