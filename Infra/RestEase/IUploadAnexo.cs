using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using RestEase;

namespace SkepsTicket.Infra.RestEase
{
    public interface IUploadAnexo
    {
        [Post("Prod/cliente/upload-img")]
        Task<string> UploadAnexo([Body] MultipartFormDataContent upload);
    }
}
