
using SkepsTicket.Model;

namespace SkepsTicket.Services.Interfaces
{
    public interface IMovideskService
    {
        Task<string> CriarTicketAtivo(EmailAtivoModel emailAtivo);
        Task ProcessarNovoTicketReceptivo(string ticketMovideskId);
        Task TranscreverTicket(TranscreverTicketModel transcrever);
    }
}