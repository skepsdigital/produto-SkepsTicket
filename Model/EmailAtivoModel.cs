using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace SkepsTicket.Model
{
    public class EmailAtivoModel
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string? CC { get; set;}
        public string Attendant { get; set; }
        public string Name { get; set; }
        public IFormFile? file { get; set; }
        public string ClientID { get; set; }
    }
}
