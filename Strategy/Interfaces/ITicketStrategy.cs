using SkepsTicket.Model;

namespace SkepsTicket.Strategy.Interfaces
{
    public interface ITicketStrategy
    {
        Task Processar(TicketModel ticket);

        Task ProcessarAtivo(dynamic obj);
    }
}
