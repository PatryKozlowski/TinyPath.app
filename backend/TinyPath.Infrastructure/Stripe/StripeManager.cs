using CacheManager.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Domain.Enums;

namespace TinyPath.Infrastructure.Stripe;

public class StripeManager : IStripeManager
{
    private readonly StripeOptions _options;
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<StripeManager> _logger;
    private readonly IAuthDataProvider _authDataProvider;
    private readonly IEmailSchema _emailSchema;
    private readonly IEmailSender _emailSender;

    public StripeManager(IOptions<StripeOptions> options, IApplicationDbContext dbContext, ILogger<StripeManager> logger, IAuthDataProvider authDataProvider, IEmailSchema emailSchema, IEmailSender emailSender)
    {
        _dbContext = dbContext;
        _logger = logger;
        _authDataProvider = authDataProvider;
        _emailSchema = emailSchema;
        _emailSender = emailSender;
        _options = options.Value;
    }

    public async Task<string> CreateCheckoutSession(string customerId, string planId)
    {
        var paymentMethodTypes = _options.PaymentMethodTypes;
        var returnUrl = _options.SuccessUrl;
        var cancelUrl = _options.CancelUrl;
        
        var options = new SessionCreateOptions
        {
            Customer = customerId,
            PaymentMethodTypes = paymentMethodTypes,
            LineItems =
            [
                new SessionLineItemOptions
                {
                    Price = planId,
                    Quantity = 1
                }
            ],
            Mode = "subscription",
            SuccessUrl = returnUrl,
            CancelUrl = cancelUrl
        };
        
        var service = new SessionService();
        var session = await service.CreateAsync(options);
        
        return session.Url;
    }

    public async Task HandleWebhook()
    {
        var json = await _authDataProvider.GetStripeRequestBody();
        var signature = await _authDataProvider.GetStripeSignatureHeader();
        Event stripeEvent;
        
        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, signature, _options.WebhookSecret);
            switch (stripeEvent.Type)
            {
                case "customer.created":
                    await HandleCreateStripeCustomer(stripeEvent);
                    break;
                case "customer.subscription.created":
                    await HandleCheckoutSessionCompleted(stripeEvent);
                    break;
                case "invoice.paid":
                    await HandleInvoicePaid(stripeEvent);
                    break;
                case "invoice.payment_failed":
                    Console.WriteLine("Invoice payment failed!");
                    break;
                default:
                    _logger.LogError($"Unhandled event type: {stripeEvent.Type}");
                    break;
            }
        }
        catch (StripeException e)
        {
            _logger.LogError($"Stripe error: {e.Message}");
        }
    }

    public async Task<Customer> CreateCustomer(string email)
    {
       var options = new CustomerCreateOptions
       {
           Email = email
       };
       
       var service = new CustomerService();
       var customer = await service.CreateAsync(options);

       return customer;
    }

    private async Task HandleCreateStripeCustomer(Event stripeEvent)
    {
        var customer = stripeEvent.Data.Object as Customer;
        
        if (customer is null)
        {
            _logger.LogError("Stripe event data is not a customer");
            return;
        }
        
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == customer.Email);
        
        if (user is null)
        {
            _logger.LogError("User not found");
            throw new ErrorException("UserNotFound");
        }

        var stripeCustomer = new Domain.Entities.TinyPath.Customer
        {
            UserId = user.Id,
            CustomerId = customer.Id,
            Email = customer.Email,
            User = user
        };
        
        await _dbContext.Customers.AddAsync(stripeCustomer);
        
        await _dbContext.SaveChangesAsync();
    }
    
    private async Task HandleCheckoutSessionCompleted(Event stripeEvent)
    {
        var subscription = stripeEvent.Data.Object as Subscription;
        
        if (subscription is null)
        {
            _logger.LogError("Stripe event data is not a session");
            return;
        }
        
        var user = await _dbContext
            .Users
            .Include(u => u.Customer)
            .FirstOrDefaultAsync(x => x.Customer.CustomerId == subscription.CustomerId);
        
        if (user is null)
        {
            _logger.LogError("Customer not found");
            throw new ErrorException("CustomerNotFound");
        }
        
        var subscriptionEntity = new Domain.Entities.TinyPath.Subscription
        {
            SubscriptionId = subscription.Id,
            UserId = user.Id,
            User = user,
            Status = subscription.Status,
            Expires = new DateTimeOffset(subscription.CurrentPeriodEnd, TimeSpan.Zero)
        };
        
        await _dbContext.Subscriptions.AddAsync(subscriptionEntity);
        
        await _dbContext.SaveChangesAsync();
    }
    
    private async Task HandleInvoicePaid(Event stripeEvent)
    {
        var invoice = stripeEvent.Data.Object as Invoice;
        
        if (invoice is null)
        {
            _logger.LogError("Stripe event data is not an invoice");
        }
        
        var subscription = await _dbContext.Subscriptions
            .FirstOrDefaultAsync(x => x.SubscriptionId == invoice!.SubscriptionId);

        if (subscription is null)
        {
            _logger.LogError("Subscription not found");
        }
        
        subscription!.Status = invoice!.Status;
        
        await _dbContext.SaveChangesAsync();
        
        var linkToInvoice = invoice.HostedInvoiceUrl;
        
        var emailSchema = _emailSchema.GetSchema(EmailSchemas.SubscriptionInvoiceEmail,linkToInvoice);
        
        try
        {
            await _emailSender.SendEmailAsync(invoice.CustomerEmail, emailSchema.subject, emailSchema.content);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "ErrorSendingEmail");
        }
    }
}