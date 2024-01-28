using TinyPath.Application.Interfaces;

namespace TinyPath.Application.Logic.Abstractions;

public abstract class BaseCommandHandler
{
    protected readonly IApplicationDbContext _dbContext;
    
    protected BaseCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}