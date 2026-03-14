using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Canarytype_alpha3.Data.Config
{
    public class UsersSubmissionsTableConfig : IEntityTypeConfiguration<UserSubmissions>
    {
        public void Configure(EntityTypeBuilder<UserSubmissions> builder)
        {
            builder.ToTable("UsersSubmissionsTable");

            builder.HasKey(t => t.Id);

            builder.Property(n => n.SubmissionDate).IsRequired();

            builder.Property(n => n.typingSpeedPerSecond).IsRequired();

            builder.Property(n => n.rawTypingSpeedPerSecond).IsRequired();

            builder.Property(n => n.Score).IsRequired();

            builder.HasOne(n => n.UserInfo).WithMany(n => n.UserSubmissions).HasForeignKey(n => n.UserInfoId).HasConstraintName("FK_SUBMISSION_USER");
        }
    }
}
