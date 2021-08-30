using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SpeechRecognizerWPF
{
    class Translator
    {
        private const string Key = "433770b2539d47718585deff9642867d";

        private static readonly HttpClient client = new HttpClient
        {
            DefaultRequestHeaders = { { "Ocp-Apim-Subscription-Key", Key } }
        };

        public static async Task<string> Translate(string text, string language)
        {
            var encodedTesxt = WebUtility.UrlEncode(text);

            var url = "https://api.microsofttranslator.com/V2/Http.svc/Translate?" +
                       $"to={language}&text={encodedTesxt}";

            var result = await client.GetStringAsync(url);

            return XElement.Parse(result).Value;
        }
    }
}
