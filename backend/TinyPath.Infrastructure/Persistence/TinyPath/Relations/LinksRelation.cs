using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TinyPath.Domain.Entities.TinyPath;

namespace TinyPath.Infrastructure.Persistence.TinyPath.Relations;

public class LinksRelation : IEntityTypeConfiguration<Link>
{
    public void Configure(EntityTypeBuilder<Link> builder)
    {
        builder
            .HasOne(l => l.User)
            .WithMany(u => u.Links)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(l => l.Guest)
            .WithMany(gu => gu.GuestLinks)
            .HasForeignKey(l => l.GuestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}