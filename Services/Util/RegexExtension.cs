using System.Text.RegularExpressions;

namespace SkepsTicket.Services.Util
{
    public static class RegexExtension
    {
        public static string ExtractEmail(string text)
        {
            string pattern = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}";
            Match match = Regex.Match(text, pattern);

            return match.Success ? match.Value : "E-mail não encontrado";
        }
    }
}
