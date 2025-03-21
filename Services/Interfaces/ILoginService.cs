
using SkepsTicket.Mongo.Model;

namespace SkepsTicket.Services.Interfaces
{
    public interface ILoginService
    {
        Task<string?> GenerateCode(string email);
        Task<EmpresasInfoMongo> VerifyCode(string email, string code);
    }
}
