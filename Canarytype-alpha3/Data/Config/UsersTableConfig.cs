using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Canarytype_alpha3.Data.Config
{
    public class UsersTableConfig : IEntityTypeConfiguration<UserInfo>
    {
        public void Configure(EntityTypeBuilder<UserInfo> builder)
        {
            builder.ToTable("UsersTable");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.UserEmail).IsUnique();

            builder.Property(x => x.UserName).IsRequired();

            builder.Property(x => x.UserEmail).IsRequired();
        }
    }
}
