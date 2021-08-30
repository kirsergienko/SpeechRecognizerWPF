using Microsoft.CognitiveServices.Speech;

namespace SpeechRecognizerWPF
{
    class Resultator
    {
        public static string GetResult(SpeechRecognitionEventArgs e)
        {
            string result = e.Result.Text;

            return result;
        }
    }
}
