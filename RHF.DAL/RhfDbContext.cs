using System;
using System.Collections.Generic;
using RHF.Shared;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RHF.DAL;

public partial class RhfDbContext : IdentityDbContext
{
    public RhfDbContext()
    {
    }

    public RhfDbContext(DbContextOptions<RhfDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Benefactor> Benefactors { get; set; }

    public virtual DbSet<BenefactorContribution> BenefactorContributions { get; set; }

    public virtual DbSet<DonationsDetail> DonationsDetails { get; set; }

    public virtual DbSet<DonationsHeader> DonationsHeaders { get; set; }
    public virtual DbSet<ProjectTasks> ProjectTasks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySQL("Server=localhost;Database=RHF_db;Uid=root;Pwd=mvemjsunp;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Benefactor>(entity =>
        {
            entity.ToTable("Benefactor");

            entity.HasIndex(e => e.Id, "IX_Benefactor_Id").IsUnique();

            entity.HasIndex(e => e.Nrc, "IX_Benefactor_NRC").IsUnique();

            entity.Property(e => e.Nrc).HasColumnName("NRC");
        });

        modelBuilder.Entity<BenefactorContribution>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_BenefactorContributions_Id").IsUnique();

            entity.HasOne(d => d.Benefactor).WithMany(p => p.BenefactorContributions)
                .HasForeignKey(d => d.BenefactorId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DonationsDetail>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_DonationsDetails_Id").IsUnique();

            entity.HasOne(d => d.DonationsHeader).WithMany(p => p.DonationsDetails)
                .HasForeignKey(d => d.DonationsHeaderId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DonationsHeader>(entity =>
        {
            entity.ToTable("DonationsHeader");

            entity.HasIndex(e => e.Id, "IX_DonationsHeader_Id").IsUnique();
        });

        modelBuilder.Entity<ProjectTasks>(entity =>
        {
            entity.ToTable("ProjectTasks");

            entity.HasIndex(e => e.ID, "IX_ProjectTasks_Id").IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
