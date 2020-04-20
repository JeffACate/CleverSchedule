using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CleverScheduleProject.Models;

namespace CleverScheduleProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Contractor> Contractors { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Comment code: 140801

            builder.Entity<Appointment>()
                .HasKey(a => new { a.DateTime, a.ContractorId });

            builder.Entity<IdentityRole>()
                .HasData(
                    new IdentityRole
                    {
                        Id = "b87a5d5c-bf70-4b57-a459-efabfd4b90e9",
                        Name = "Admin",
                        NormalizedName = "ADMIN",
                        ConcurrencyStamp = "1eb39cdf-abb7-48f9-89ea-dd8e9a146b33",
                    },
                    new IdentityRole
                    {
                        Id = "962790da-4fa3-4d83-9fd6-fefa401f69a6",
                        Name = "Client",
                        NormalizedName = "CLIENT",
                        ConcurrencyStamp = "28189e2f-6e40-465e-8202-928200b83f47",
                    },
                    new IdentityRole
                    {
                        Id = "a97e860b-3467-4fb5-b03a-523505d54e27",
                        Name = "Contractor",
                        NormalizedName = "CONTRACTOR",
                        ConcurrencyStamp = "333970ff-9262-48b0-b247-a2e9528e0156",
                    });
        }
    }
}
