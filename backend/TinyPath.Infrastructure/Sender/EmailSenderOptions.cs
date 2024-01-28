namespace TinyPath.Infrastructure.Sender;

public class EmailSenderOptions
{
    public required string Mail { get; init; }
    public required string Password { get; init; }
    public required string Host { get; init; }
    public required int Port { get; init; }
    public required bool EnableSSL { get; init; }
    public required string DisplayName { get; init; }
}