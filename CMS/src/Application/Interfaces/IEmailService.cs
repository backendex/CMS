namespace CMS.src.Application.Interfaces
{
    public interface IEmailService
    {

        Task SendEmailAsync(string to, string subject, string html);
        Task SendWelcomeEmail(string email,string fullName, string tempPassword, string confirmationLink);

    }
}
