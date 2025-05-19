using Microsoft.AspNetCore.Identity.UI.Services;
namespace LibraryInfrastructure
{
    public class DEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // No actual email sending
            Console.WriteLine($"Pretending to send email to {email}: {subject}");
            return Task.CompletedTask;
        }
    }
}
