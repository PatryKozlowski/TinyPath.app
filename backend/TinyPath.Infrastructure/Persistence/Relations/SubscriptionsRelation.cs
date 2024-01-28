using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TinyPath.Domain.Entities;

namespace TinyPath.Infrastructure.Persistence.Relations;

public class SubscriptionsRelation : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder
            .HasOne(sub => sub.User)
            .WithOne(u => u.Subscription)
            .HasForeignKey<Subscription>(su => su.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}