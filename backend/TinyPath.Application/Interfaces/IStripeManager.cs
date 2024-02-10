using Stripe;

namespace TinyPath.Application.Interfaces;

public interface IStripeManager
{
    Task<string> CreateCheckoutSession(string customerId, string planId);
    Task HandleWebhook();
    Task<Customer> CreateCustomer(string email);
}