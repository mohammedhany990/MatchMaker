using MatchMaker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MatchMaker.Infrastructure.Configurations
{

    public class UserLikeConfiguration : IEntityTypeConfiguration<UserLike>
    {
        public void Configure(EntityTypeBuilder<UserLike> builder)
        {
            builder.ToTable("Likes");

            // Configure composite primary key as non-clustered
            builder.HasKey(k => new { k.SourceUserId, k.TargetUserId })
                .IsClustered(false);

            // Configure relationships with NO ACTION
            builder.HasOne(ul => ul.SourceUser)
                .WithMany(u => u.LikedUsers)
                .HasForeignKey(ul => ul.SourceUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(ul => ul.TargetUser)
                .WithMany(u => u.LikedByUsers)
                .HasForeignKey(ul => ul.TargetUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Add indexes
            builder.HasIndex(ul => ul.SourceUserId);
            builder.HasIndex(ul => ul.TargetUserId);
        }
    }

}
