namespace TinyPath.Application.Services.Conformation;

public interface IGetConfirmationLink
{
    string EmailConfirmationLink(string confirmationToken);
    string RedirectConfirmationLink();
}