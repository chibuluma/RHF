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
    public virtual DbSet<ProjectTasks> ProjectTasks { get; set; }
    public virtual DbSet<Benefactor> Benefactors { get; set; }
    public virtual DbSet<BenefactorContribution> BenefactorContributions { get; set; }
    public virtual DbSet<DonationsDetail> DonationsDetails { get; set; }
    public virtual DbSet<DonationsHeader> DonationsHeaders { get; set; }
    public virtual DbSet<FinancialYear> FinancialYear { get; set; }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //     => optionsBuilder.UseMySQL("Server=localhost;Database=RHF_db;Uid=root;Pwd=mvemjsunp;");
     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
         => optionsBuilder.UseSqlite("Data Source=../RHF_db.db");
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectTasks>(entity =>
        {
            entity.ToTable("ProjectTasks");
            
            entity.HasIndex(e => e.ID, "IX_ProjectTasks_Id").IsUnique();
        });

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

            entity.HasOne(d => d.FinancialYear)
                .WithMany(p => p.BenefactorContributions)
                .HasForeignKey(d => d.FinancialYearId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Benefactor)
                .WithMany(p => p.BenefactorContributions)
                .HasForeignKey(d => d.BenefactorId)
                .OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<DonationsDetail>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_DonationsDetails_Id").IsUnique();

            entity.HasOne(d => d.DonationsHeader)
            .WithMany(p => p.DonationsDetails)
                .HasForeignKey(d => d.DonationsHeaderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DonationsHeader>(entity =>
        {
            entity.ToTable("DonationsHeader");
            entity.HasMany(p=>p.DonationsDetails)
            .WithOne(c=>c.DonationsHeader)
            .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Id, "IX_DonationsHeader_Id")
            .IsUnique();
         });

        base.OnModelCreating(modelBuilder);
    }

}
