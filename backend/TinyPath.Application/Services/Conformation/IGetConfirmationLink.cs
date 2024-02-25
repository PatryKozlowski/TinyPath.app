namespace TinyPath.Application.Services.Conformation;

public interface IGetConfirmationLink
{
    string EmailConfirmationLink(string confirmationToken);
    string EmailConfirmationResetPasswordLink(string confirmationToken);
}