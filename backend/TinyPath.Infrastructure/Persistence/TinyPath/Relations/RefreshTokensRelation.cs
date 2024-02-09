using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TinyPath.Domain.Entities.TinyPath;

namespace TinyPath.Infrastructure.Persistence.TinyPath.Relations;

public class RefreshTokensRelation : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder
            .HasOne(rf => rf.User)
            .WithOne(u => u.RefreshToken)
            .HasForeignKey<RefreshToken>(rf => rf.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}