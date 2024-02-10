namespace TinyPath.Infrastructure.Stripe;

public class StripeOptions
{
    public required string PublishableKey { get; init; }
    public required string SecretKey { get; init; }
    public required string WebhookSecret { get; init; }
    public required List<string> PaymentMethodTypes { get; init; }
    public required string SuccessUrl { get; init; }
    public required string CancelUrl { get; init; }
    public required string PaymentMode { get; init; }
}