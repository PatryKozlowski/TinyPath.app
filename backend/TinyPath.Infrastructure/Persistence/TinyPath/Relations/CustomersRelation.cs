using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TinyPath.Domain.Entities.TinyPath;

namespace TinyPath.Infrastructure.Persistence.TinyPath.Relations;

public class CustomersRelation : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder
            .HasOne(cu => cu.User)
            .WithOne(u => u.Customer)
            .HasForeignKey<Customer>(cu => cu.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}