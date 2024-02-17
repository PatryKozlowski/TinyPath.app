namespace TinyPath.Application.Services.Conformation;

public class ConfirmatioOptions
{
    public required string EmailConfirmationUrl { get; init; }
    public required string RedirectConfirmationUrl { get; init; }
}