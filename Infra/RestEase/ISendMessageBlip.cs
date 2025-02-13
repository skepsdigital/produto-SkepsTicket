using RestEase;
using SkepsTicket.Model;

namespace SkepsTicket.Infra.RestEase
{
    public interface ISendMessageBlip
    {
        [Post("criar-conta")]
        Task<string> CriarAccount(
            [Body] string request,
            [Header("Content-Type")] string contentType = "application/json");

        [Post("enviar-mensagem")]
        Task<string> EnviarMensagem(
             [Body] string request,
             [Header("Content-Type")] string contentType = "application/json");
    }
}
