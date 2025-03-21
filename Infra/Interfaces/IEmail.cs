
namespace SkepsTicket.Infra.Interfaces
{
    public interface IEmail
    {
        Task SendMessageAsync(string recipient, string content);
    }
}
