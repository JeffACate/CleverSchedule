using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CleverScheduleProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Comment code: 140801

            builder.Entity<IdentityRole>()
                .HasData(
                    new IdentityRole
                    {
                        Id = "",
                        Name = "Admin",
                        NormalizedName = "ADMIN",
                        ConcurrencyStamp = "",
                    },
                    new IdentityRole
                    {
                        Id = "",
                        Name = "Client",
                        NormalizedName = "CLIENT",
                        ConcurrencyStamp = "",
                    },
                    new IdentityRole
                    {
                        Id = "",
                        Name = "Contractor",
                        NormalizedName = "CONTRACTOR",
                        ConcurrencyStamp = "",
                    });
        }
    }
}
