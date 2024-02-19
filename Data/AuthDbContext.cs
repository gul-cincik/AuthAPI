using AuthAPI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Data
{
    public class AuthDbContext : IdentityDbContext<AppUser>
    {

        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) 
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            string[] roles = { "Admin", "Sales Manager", "Sales Advisor" };

            foreach (var roleName in roles)
            {
                builder.Entity<IdentityRole>().HasData(new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                });
            }
        }
        
    }
}
