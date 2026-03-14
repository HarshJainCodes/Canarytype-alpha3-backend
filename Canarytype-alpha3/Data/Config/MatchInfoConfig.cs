using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Canarytype_alpha3.Data.Config
{
    public class MatchInfoConfig : IEntityTypeConfiguration<MatchInfo>
    {
        public void Configure(EntityTypeBuilder<MatchInfo> builder)
        {
            builder.ToTable("MatchInfoTable");

            builder.HasKey(x => x.RoomId);

            builder.HasOne(x => x.Player1).WithMany(x => x.MatchesAsPlayer1).HasForeignKey(x => x.Player1Id).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Player2).WithMany(x => x.MatchesAsPlayer2).HasForeignKey(x => x.Player2Id).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
