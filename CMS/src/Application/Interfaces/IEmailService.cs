namespace CMS.src.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendWelcomeEmail(string email, string tempPassword, string confirmationLink);
    }
}
