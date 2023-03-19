using System.Threading.Tasks;

namespace Comp1640.EmailService
{
    public interface ISendMailService
    {
        Task SendMail(MailContent mailContent);

        Task SendEmailAsync(string email, string subject, string message);
    }
}
