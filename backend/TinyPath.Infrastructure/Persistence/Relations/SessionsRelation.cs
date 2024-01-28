using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TinyPath.Domain.Entities;

namespace TinyPath.Infrastructure.Persistence.Relations;

public class SessionsRelation : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder
            .HasOne(s => s.User)
            .WithOne(u => u.Session)
            .HasForeignKey<Session>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}