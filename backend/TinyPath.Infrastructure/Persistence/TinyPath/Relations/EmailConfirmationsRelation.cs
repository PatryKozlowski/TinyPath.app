using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TinyPath.Domain.Entities.TinyPath;

namespace TinyPath.Infrastructure.Persistence.TinyPath.Relations;

public class EmailConfirmationsRelation : IEntityTypeConfiguration<EmailConfirmation>
{
    public void Configure(EntityTypeBuilder<EmailConfirmation> builder)
    {
        builder
            .HasOne(ec => ec.User)
            .WithOne(u => u.EmailConfirmation)
            .HasForeignKey<EmailConfirmation>(ec => ec.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}