using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RestarauntApp
{
    public partial class RestarauntContext : DbContext
    {
        public RestarauntContext()
        {
        }

        public RestarauntContext(DbContextOptions<RestarauntContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Dish> Dishes { get; set; } = null!;
        public virtual DbSet<DishList> DishLists { get; set; } = null!;
        public virtual DbSet<Ingridient> Ingridients { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-932LPHA\\SQLEXPRESS;Database=Restaraunt;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dish>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<DishList>(entity =>
            {
                entity.ToTable("DishList");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DishId).HasColumnName("DishID");

                entity.Property(e => e.IngridientId).HasColumnName("IngridientID");

                entity.HasOne(d => d.Dish)
                    .WithMany(p => p.DishLists)
                    .HasForeignKey(d => d.DishId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DishList_Dishes");

                entity.HasOne(d => d.Ingridient)
                    .WithMany(p => p.DishLists)
                    .HasForeignKey(d => d.IngridientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DishList_Ingridients1");
            });

            modelBuilder.Entity<Ingridient>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
