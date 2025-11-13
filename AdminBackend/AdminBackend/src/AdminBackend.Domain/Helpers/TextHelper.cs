using System.Text.RegularExpressions;

namespace AdminBackend.Domain.Helpers
{
    public static class TextHelper
    {
        public static string NormalizeText(this string text)
        {
            text = Regex.Replace(text,@"\s+", " ").Trim();
            text = text.Replace(".,", ".").Replace("..", ".").Replace(". .", ".").Replace("\n", "").Replace("&nbsp;", " ").Replace("??", " ").Trim();
            return text;
        }
    }
}
