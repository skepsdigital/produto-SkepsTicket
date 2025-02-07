using RestEase;
using SkepsTicket.Model;

namespace SkepsTicket.Infra.RestEase
{
    public interface IBlipSender
    {
        [Post("commands")]
        Task<dynamic> SendContactAsync(
           [Body] string body,
           [Header("Authorization")] string authorization,
           [Header("Content-Type")] string contentType = "application/json"
       );

        [Post("commands")]
        Task<dynamic> SendCommandAsync(
           [Body] string body,
           [Header("Authorization")] string authorization,
           [Header("Content-Type")] string contentType = "application/json"
       );

        [Post("messages")]
        Task<dynamic> SendCommandMessageAsync(
           [Body] string body,
           [Header("Authorization")] string authorization,
           [Header("Content-Type")] string contentType = "application/json"
       );
    }
}
