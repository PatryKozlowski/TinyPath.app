using Stripe;

namespace TinyPath.Application.Interfaces;

public interface IStripeManager
{
    Task<string> CreateCheckoutSession(string customerId, string planId);
    Task<string> CreateBillingPortal(string customerId);
    Task<bool> CancelSubscription(string subscriptionId);
    Task HandleWebhook();
    Task<Customer> CreateCustomer(string email);
}