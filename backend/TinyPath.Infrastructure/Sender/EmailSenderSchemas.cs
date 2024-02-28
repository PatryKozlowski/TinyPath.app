namespace TinyPath.Infrastructure.Sender;

public abstract class EmailSenderSchemas
{
    public static (string subject, string content) EmailConfirmationSchema(string link)
    {
        var subject = "Confirm your email";
        
        var content = $"Dear User,<br /><br />" +
                      $"Thank you for registering an account with us. " +
                      $"To complete the registration process, please click on the link below:<br /><br />" +
                      $"<a href=\"{link}\">Confirm your email</a><br /><br />" +
                      $"This link will expire in 15 minutes.<br /><br />" +
                      $"Sincerely,<br />Your Application Team";
        
        return (subject, content);
    }
    
    public static (string subject, string content) EmailWithLinkViewsCountSchema(string link)
    {
        var subject = "Check the number of views for your link";

        var content = $"Dear User,<br /><br />" +
               $"You can find the link to check the number of views below:<br /><br />" +
               $"<a href=\"{link}\">Click to check your link</a><br /><br />" +
               $"Sincerely,<br />Your Application Team";
        
        return (subject, content);
    }
    
    public static (string subject, string content) EmailWithSubscriptionInvoice(string link)
    {
        var subject = "Your subscription invoice";
        
        var content = $"Dear User,<br /><br />" +
                      $"You can find the link to your subscription invoice below:<br /><br />" +
                      $"<a href=\"{link}\">Click to check your invoice</a><br /><br />" +
                      $"Sincerely,<br />Your Application Team";
        
        return (subject, content);
    }
    
    public static (string subject, string content) EmailWithResetPasswordLink(string link)
    {
        var subject = "Reset your password";
        
        var content = $"Dear User,<br /><br />" +
                      $"You can reset your password by clicking on the link below:<br /><br />" +
                      $"<a href=\"{link}\">Reset your password</a><br /><br />" +
                      $"This link will expire in 15 minutes.<br /><br />" +
                      $"Sincerely,<br />Your Application Team";
        
        return (subject, content);
    }
    
    public static (string subject, string content) EmailWithDeleteAccountCode(int code)
    {
        var subject = "Delete your account";
        
        var content = $"Dear User,<br /><br />" +
                      $"To delete your account, please use the following code:<br /><br />" +
                      $"{code}<br /><br />" +
                      $"Sincerely,<br />Your Application Team";
        
        return (subject, content);
    }
}