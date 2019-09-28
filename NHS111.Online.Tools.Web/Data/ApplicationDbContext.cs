using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NHS111.Online.Tools.Models;

namespace NHS111.Online.Tools.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>(config =>
            {
                config.Property(p => p.Status).HasColumnType("int");
            });
            builder.Entity<ApplicationRole>();
        }
    }
}
