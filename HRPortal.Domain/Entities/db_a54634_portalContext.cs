﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace HRPortal.Domain.Entities
{
    public partial class db_a54634_portalContext : DbContext
    {
        public db_a54634_portalContext()
        {
        }

        public db_a54634_portalContext(DbContextOptions<db_a54634_portalContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual DbSet<TblCandidateApplication> TblCandidateApplications { get; set; }
        public virtual DbSet<TblCompetency> TblCompetencies { get; set; }
        public virtual DbSet<TblSkill> TblSkills { get; set; }
        public virtual DbSet<TblSummary> TblSummaries { get; set; }
        public virtual DbSet<TblVacancyAdvert> TblVacancyAdverts { get; set; }

        //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //        {
        //            if (!optionsBuilder.IsConfigured)
        //            {
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        //                optionsBuilder.UseSqlServer("Server=SQL6010.site4now.net;Database=db_a54634_portal; User Id=db_a54634_portal_admin; password=Florence@40391");
        //            }
        //        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId, "IX_AspNetUserRoles_RoleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<TblCandidateApplication>(entity =>
            {
                entity.ToTable("tbl_candidate_application");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Availability)
                    .HasMaxLength(50)
                    .HasColumnName("availability");

                entity.Property(e => e.CompletedNysc)
                    .HasMaxLength(50)
                    .HasColumnName("completed_Nysc");

                entity.Property(e => e.CountryOfResidence).HasColumnName("country_of_residence");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("date");

                entity.Property(e => e.Email).HasColumnName("email");

                entity.Property(e => e.Firstname).HasColumnName("firstname");

                entity.Property(e => e.Lastname).HasColumnName("lastname");

                entity.Property(e => e.Nationality).HasColumnName("nationality");

                entity.Property(e => e.PermanentStaffNsia)
                    .HasMaxLength(50)
                    .HasColumnName("permanentStaff_Nsia");

                entity.Property(e => e.Phonenumber)
                    .HasMaxLength(50)
                    .HasColumnName("phonenumber");

                entity.Property(e => e.ProfessionalQualification)
                    .HasColumnType("text")
                    .HasColumnName("professionalQualification");

                entity.Property(e => e.StateOfResidence).HasColumnName("state_of_residence");

                entity.Property(e => e.UserId)
                    .HasMaxLength(450)
                    .HasColumnName("user_id");

                entity.Property(e => e.WorkExperience)
                    .HasColumnType("text")
                    .HasColumnName("work_experience");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblCandidateApplications)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_tbl_candidate_application_AspNetUsers");
            });

            modelBuilder.Entity<TblCompetency>(entity =>
            {
                entity.ToTable("tbl_competency");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.UserId)
                    .HasMaxLength(450)
                    .HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblCompetencies)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_tbl_competency_AspNetUsers");
            });

            modelBuilder.Entity<TblSkill>(entity =>
            {
                entity.ToTable("tbl_skills");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Skills).HasColumnName("skills");
            });

            modelBuilder.Entity<TblSummary>(entity =>
            {
                entity.ToTable("tbl_summary");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CompanySummary)
                    .HasColumnType("text")
                    .HasColumnName("company_summary");

                entity.Property(e => e.EqualityStatement)
                    .HasColumnType("text")
                    .HasColumnName("equality_statement");
            });

            modelBuilder.Entity<TblVacancyAdvert>(entity =>
            {
                entity.ToTable("tbl_vacancy_advert");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CompanySummary)
                    .HasColumnType("text")
                    .HasColumnName("company_summary");

                entity.Property(e => e.ContractType).HasColumnName("contract_type");

                entity.Property(e => e.DateOfAdvert)
                    .HasColumnType("date")
                    .HasColumnName("date_of_advert");

                entity.Property(e => e.Datecreated)
                    .HasColumnType("datetime")
                    .HasColumnName("datecreated");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("endDate");

                entity.Property(e => e.EqualityStatement)
                    .HasColumnType("text")
                    .HasColumnName("equality_statement");

                entity.Property(e => e.JobCode).HasColumnName("job_code");

                entity.Property(e => e.JobObjectives)
                    .HasColumnType("text")
                    .HasColumnName("job_objectives");

                entity.Property(e => e.JobTitle).HasColumnName("job_title");

                entity.Property(e => e.Location).HasColumnName("location");

                entity.Property(e => e.PostedBy).HasColumnName("postedBy");

                entity.Property(e => e.RelevantRequirement)
                    .HasColumnType("text")
                    .HasColumnName("relevant_requirement");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("startDate");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.Property(e => e.TravelRequirements)
                    .HasMaxLength(50)
                    .HasColumnName("travel_requirements");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
