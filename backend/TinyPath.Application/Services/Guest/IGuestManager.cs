namespace TinyPath.Application.Services.Guest;

public interface IGuestManager
{
    Task<Domain.Entities.TinyPath.Guest> CreateGuestUser();

    Task<Domain.Entities.TinyPath.Guest?> GetGuestUser();
    bool ValidateGuestUserCreationLink(Domain.Entities.TinyPath.Guest? guestUser);
    Task UnblockGuestUser(Guid guestId);
}