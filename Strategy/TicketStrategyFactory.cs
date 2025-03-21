using Microsoft.Extensions.Options;
using SkepsTicket.Infra.RestEase;
using SkepsTicket.Model;
using SkepsTicket.Mongo.Interfaces;
using SkepsTicket.Strategy.Interfaces;

namespace SkepsTicket.Strategy
{
    public class TicketStrategyFactory
    {
        private readonly Dictionary<string, ITicketStrategy> _strategies;

        public TicketStrategyFactory(IOptions<EmpresasConfig> empresasConfig, ISendMessageBlip sendMessageBlip, Func<string, IBlipSender> senderBlip, IMongoService mongoService)
        {
            _strategies = new()
            {
                {"Blip - food to save", new FoodToSaveStrategy(empresasConfig)},
                {"PixBet", new PixBetStrategy(empresasConfig, sendMessageBlip, senderBlip, mongoService)},
                {"B2C", new B2CStrategy(empresasConfig, sendMessageBlip, senderBlip, mongoService) },
                {"EstanteMagica", new EstanteMagicaStrategy(empresasConfig, sendMessageBlip, senderBlip, mongoService) }
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

            if (name.IndexOf("b2c", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                name = "B2C";
            }

            if (name.IndexOf("estantemagica", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                name = "EstanteMagica";
            }

            return _strategies[name];
        }
    }
}
