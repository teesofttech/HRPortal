using HRPortal.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HRPortal.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {


        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<ApplicationUser>();

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .Property(e => e.FirstName)
                .HasMaxLength(1000);

            modelBuilder.Entity<ApplicationUser>()
                .Property(e => e.LastName)
                .HasMaxLength(1000);

            modelBuilder.Entity<ApplicationUser>()
              .Property(e => e.Role)
              .HasMaxLength(250);

        }
    }
}
