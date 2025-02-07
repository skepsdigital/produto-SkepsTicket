using RestEase;
using SkepsTicket.Model;

namespace SkepsTicket.Infra.RestEase
{
    public interface IMovideskAPI
    {   
        [Get("public/v1/tickets/")]
        Task<TicketModel> GetTicketAsync([Query("token")] string token, [Query("id")] int id);

        [Post("public/v1/tickets")]
        Task<TicketModel> CreateTicketAsync(
            [Query("token")] string token,
            [Query("id")] int id,
            [Body] string ticket,
            [Header("Content-Type")] string contentType = "application/json");


        [Patch("public/v1/tickets/")]
        Task<dynamic> UpdateTicketAsync(
            [Query("token")] string token,
            [Query("id")] int id,
            [Body] string ticket,
            [Header("Content-Type")] string contentType = "application/json");

        [Get("public/v1/persons")]
        Task<List<dynamic>> GetClientAsync(
            [Query("token")] string token,
            [Query("filter")] string filter);

        [Post("public/v1/persons")]
        Task<dynamic> CreateClientAsync(
            [Query("token")] string token,
            [Body] string createClientRequest,
            [Header("Content-Type")] string contentType = "application/json");

    }
}
