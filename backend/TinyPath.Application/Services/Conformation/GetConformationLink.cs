using Microsoft.Extensions.Options;

namespace TinyPath.Application.Services.Conformation;

public class GetConformationLink : IGetConformationLink
{
    private readonly ConformationOptions _conformationOptions;

    public GetConformationLink(IOptions<ConformationOptions> conformationOptions)
    {
        _conformationOptions = conformationOptions.Value;
    }

    public string EmailConfirmationLink(string confirmationToken)
    {
        var link = _conformationOptions.EmailConfirmationUrl + confirmationToken;
        return link;
    }
}