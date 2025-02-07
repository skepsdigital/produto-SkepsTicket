using Microsoft.Extensions.Options;
using SkepsTicket.Infra.RestEase;
using SkepsTicket.Model;
using SkepsTicket.Strategy.Interfaces;

namespace SkepsTicket.Strategy
{
    public class TicketStrategyFactory
    {
        private readonly Dictionary<string, ITicketStrategy> _strategies;

        public TicketStrategyFactory(IOptions<EmpresasConfig> empresasConfig, ISendMessageBlip sendMessageBlip, Func<string, IBlipSender> senderBlip)
        {
            _strategies = new()
            {
                {"Blip - food to save", new FoodToSaveStrategy(empresasConfig)},
                {"PixBet", new PixBetStrategy(empresasConfig, sendMessageBlip, senderBlip)},
            };
        }

        public ITicketStrategy GetStrategy(string name)
        {
            if (name.IndexOf("pixbet", StringComparison.OrdinalIgnoreCase) >= 0 ||
                name.IndexOf("ganhabet", StringComparison.OrdinalIgnoreCase) >= 0 ||
                name.IndexOf("betdasorte", StringComparison.OrdinalIgnoreCase) >= 0 ||
                name.IndexOf("betvip", StringComparison.OrdinalIgnoreCase) >= 0 ||
                name.IndexOf("flabet", StringComparison.OrdinalIgnoreCase) >= 0 ||
                name.IndexOf("skeps", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                name = "PixBet";
            }

            if (name.IndexOf("food", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                name = "Blip - food to save";
            }

            return _strategies[name];
        }
    }
}
