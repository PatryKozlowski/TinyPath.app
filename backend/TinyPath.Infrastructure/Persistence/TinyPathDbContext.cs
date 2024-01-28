using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Interfaces;
using TinyPath.Domain.Common;
using TinyPath.Domain.Entities;

namespace TinyPath.Infrastructure.Persistence;

public class TinyPathDbContext : DbContext, IApplicationDbContext
{
    public TinyPathDbContext(DbContextOptions<TinyPathDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<EmailConfirmation> EmailConfirmations { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Guest> Guests { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Link> Links { get; set; }
    public DbSet<LinkStats> LinksStats { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TinyPathDbContext).Assembly);
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<DomainEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.Created = DateTime.UtcNow;
                    break;
                
                case EntityState.Modified:
                    entry.Entity.Updated = DateTime.UtcNow;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}