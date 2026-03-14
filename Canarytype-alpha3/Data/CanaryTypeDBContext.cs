using Canarytype_alpha3.Data.Config;
using Microsoft.EntityFrameworkCore;

namespace Canarytype_alpha3.Data
{
    public class CanaryTypeDBContext : DbContext
    {
        public CanaryTypeDBContext(DbContextOptions<CanaryTypeDBContext> options) : base(options)
        {
            
        }

        public DbSet<UserInfo> UsersTable { get; set; }
        public DbSet<UserSubmissions> UsersSubmissionsTable { get; set; }
        public DbSet<MatchInfo> MatchInfoTable { get; set; }
        public DbSet<VerificationCodes> VerificationCodes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsersTableConfig());
            modelBuilder.ApplyConfiguration(new MatchInfoConfig());
            modelBuilder.ApplyConfiguration(new VerificationCodesConfig());
        }
    }
}
