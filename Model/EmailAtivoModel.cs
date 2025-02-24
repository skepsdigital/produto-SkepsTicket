using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace SkepsTicket.Model
{
    public class EmailAtivoModel
    {
        [FromForm]
        public string Email { get; set; }
        [FromForm]
        public string Subject { get; set; }
        [FromForm]
        public string Category { get; set; }
        [FromForm]
        public string Description { get; set; }
        [FromForm]
        public string? CC { get; set;}
        [FromForm]
        public string Attendant { get; set; }
        [FromForm]
        public string Name { get; set; }
        [FromForm]
        public IFormFile? file { get; set; }
        [FromForm]
        public string ClientID { get; set; }
    }
}
