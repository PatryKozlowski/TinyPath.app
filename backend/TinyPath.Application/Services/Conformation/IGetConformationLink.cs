namespace TinyPath.Application.Services.Conformation;

public interface IGetConformationLink
{
    string EmailConfirmationLink(string confirmationToken);
}