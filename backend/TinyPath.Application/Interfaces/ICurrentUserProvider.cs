using TinyPath.Domain.Entities;
using TinyPath.Domain.Entities.TinyPath;

namespace TinyPath.Application.Interfaces;

public interface ICurrentUserProvider
{
    Task<User?> GetAuthenticatedUser();
    Task<User?> GetPremiumUser();
}