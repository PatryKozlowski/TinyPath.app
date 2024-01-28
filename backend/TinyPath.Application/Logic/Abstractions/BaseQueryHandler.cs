using TinyPath.Application.Interfaces;

namespace TinyPath.Application.Logic.Abstractions;

public abstract class BaseQueryHandler
{
    protected readonly IApplicationDbContext _dbContext;
    
    protected BaseQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}