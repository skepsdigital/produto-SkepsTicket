using SkepsTicket.Model;

namespace SkepsTicket.Services.Interfaces
{
    public interface IBlipService
    {
        Task ProcessarRespostaAtendente(BlipCloseTicketResponse blipCloseTicket);
    }
}
