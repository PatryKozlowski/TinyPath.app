using Microsoft.EntityFrameworkCore;
using TinyPath.Domain.Entities;

namespace TinyPath.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<EmailConfirmation> EmailConfirmations { get; set; }
    DbSet<RefreshToken> RefreshTokens { get; set; }
    DbSet<Session> Sessions { get; set; }
    DbSet<Subscription> Subscriptions { get; set; }
    DbSet<Guest> Guests { get; set; }
    DbSet<Customer> Customers { get; set; }
    DbSet<Link> Links { get; set; }
    DbSet<LinkStats> LinksStats { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}