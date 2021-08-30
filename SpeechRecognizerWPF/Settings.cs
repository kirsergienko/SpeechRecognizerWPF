using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


namespace SpeechRecognizerWPF
{
    [DataContract]
    public class Settings
    {
        [DataMember]
        public string InputLanguageName { get; set; }

        [DataMember]
        public string OutputLanguageName { get; set; }

        [DataMember]
        public string InputLanguage { get; set; }

        [DataMember]
        public string OutputLanguage { get; set; }

        [DataMember]
        public bool FromMicrophone { get; set; }

        [DataMember]
        public string Color { get; set; }

        [DataMember]
        public int FontSize { get; set; }

        public void Save()
        {
            var json = new DataContractJsonSerializer(typeof(Settings));

            using (var file = new FileStream("settings.json", FileMode.Create))
            {
                json.WriteObject(file, this);
            }
        }

        public Settings Load()
        {
            Settings settings;

            var json = new DataContractJsonSerializer(typeof(Settings));

            using (var file = new FileStream("settings.json", FileMode.OpenOrCreate))
            {
                settings = json.ReadObject(file) as Settings;
            }
            return settings;
        }
    }
}
