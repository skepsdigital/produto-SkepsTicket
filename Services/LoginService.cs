using SkepsTicket.Infra.Interfaces;
using SkepsTicket.Model;
using SkepsTicket.Mongo.Interfaces;
using SkepsTicket.Mongo.Model;
using SkepsTicket.Services.Interfaces;
using System.Text;

namespace SkepsTicket.Services
{
    public class LoginService : ILoginService
    {
        private readonly IMongoService _mongoService;
        private readonly IEmail _email;
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public LoginService(IMongoService mongoService, IEmail email)
        {
            _mongoService = mongoService;
            _email = email;
        }

        public async Task<EmpresasInfoMongo> VerifyCode(string email, string code)
        {
            var login = await _mongoService.GetLoginCodeByCodeAsync(code);
            if (login == null || login.Email != email || login.Validate < DateTime.UtcNow.AddHours(-3))
            {
                return null;
            }
            
            if(login.Email == email && login.Code == code)
            {
                var empresa = await _mongoService.GetEmpresasInfoAsync();

                return empresa.FirstOrDefault(e => e.Atendentes.Contains(email));
            }

            return null;
        }

        public async Task<string?> GenerateCode(string email)
        {
            var result = new StringBuilder();
            var random = new Random();

            var empresa = await _mongoService.GetEmpresasInfoAsync();

            var empresaLocalizada = empresa.FirstOrDefault(e => e.Atendentes.Contains(email));

            if(empresaLocalizada is null)
                { return null; }

            for (int i = 0; i < 6; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            var code = result.ToString();

            await _mongoService.InserirLoginCode(new LoginMongo { Code = code, Email = email, Validate = DateTime.UtcNow.AddHours(-3).AddHours(1) });

            await _email.SendMessageAsync(email, code);

            return code;
        }

    }
}
