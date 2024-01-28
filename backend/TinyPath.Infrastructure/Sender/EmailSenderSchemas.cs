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
                      $"This link will expire in 24 hours.<br /><br />" +
                      $"Sincerely,<br />Your Application Team";
        
        return (subject, content);
    }
    
    public static (string subject, string content) EmailWithLinkViewsCountSchema(string link)
    {
        var subject = "Confirm your email";

        var content = $"Dear User,<br /><br />" +
               $"You can find the link to check the number of views below:<br /><br />" +
               $"<a href=\"{link}\">Click to check your link</a><br /><br />" +
               $"Sincerely,<br />Your Application Team";
        
        return (subject, content);
    }
}