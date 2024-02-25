using Microsoft.Extensions.Options;

namespace TinyPath.Application.Services.Conformation;

public class GetConfirmationLink : IGetConfirmationLink
{
    private readonly ConfirmatioOptions _confirmatioOptions;

    public GetConfirmationLink(IOptions<ConfirmatioOptions> conformationOptions)
    {
        _confirmatioOptions = conformationOptions.Value;
    }

    public string EmailConfirmationLink(string confirmationToken)
    {
        var link = _confirmatioOptions.EmailConfirmationUrl + confirmationToken;
        return link;
    }

    public string EmailConfirmationResetPasswordLink(string confirmationToken)
    {
        var link = _confirmatioOptions.EmailConfirmationResetPasswordUrl + confirmationToken;
        return link;
    }
}