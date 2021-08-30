using System.Collections.Generic;
using VisioForge.Shared.NAudio.CoreAudioApi;

namespace SpeechRecognizerWPF
{
    class AudioDevices
    {
        public static Dictionary<string, string> GetDeviceID()
        {
            Dictionary<string, string> devices = new Dictionary<string, string>();
            var enumerator = new MMDeviceEnumerator();
            foreach (var endpoint in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
            {
                devices.Add(endpoint.FriendlyName, endpoint.ID);
            }
            return devices;
        }
    }
}
