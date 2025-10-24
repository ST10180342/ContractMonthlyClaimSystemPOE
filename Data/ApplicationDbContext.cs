using System.Collections.Generic;
using System.Reflection.Emit;
using ContractMonthlyClaimSystemPOE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContractMonthlyClaimSystemPOE.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Claim> Claims { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
          
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Lecturer", NormalizedName = "LECTURER" },
                new IdentityRole { Id = "2", Name = "Coordinator", NormalizedName = "COORDINATOR" },
                new IdentityRole { Id = "3", Name = "Manager", NormalizedName = "MANAGER" }
            );
        }
    }
}