using SkepsTicket.Infra.Interfaces;
using System.Text;
using System.Text.Json;

namespace SkepsTicket.Infra
{
    public class Email : IEmail
    {
        private readonly string _authorizationKey;
        private readonly string _endpointUrl;
        private readonly HttpClient _httpClient;

        public Email()
        {
            _authorizationKey = "c2V1Y29kaWdvc2tlcHM6RG4ySTNNYUxxOHMwbTFhZVlkUEI=";
            _endpointUrl = "https://augusto-almeida-kepeh.http.msging.net/messages";
            _httpClient = new HttpClient();

            // Configurando o cabeçalho de autorização
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Key", _authorizationKey);
        }



        public async Task SendMessageAsync(string recipient, string content)
        {
            var message = new
            {
                to = Uri.EscapeDataString(recipient) + "@mailgun.gw.msging.net",
                type = "text/plain",
                content
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(message),
                Encoding.UTF8,
                "application/json");

            try
            {
                var response = await _httpClient.PostAsync(_endpointUrl, jsonContent);
                response.EnsureSuccessStatusCode();

                Console.WriteLine("Mensagem enviada com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar mensagem: {ex.Message}");
                throw;
            }
        }
    }
}
