using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SpeechRecognizerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //start/stop recognizing
        private bool check = false;

        private Settings settings = new Settings();

        private Subtitles subtitles = new Subtitles();

        List<string> history = new List<string>();

        string active = "";

        ActiveWindow activeWindow = new ActiveWindow();

        public MainWindow()
        {

            this.Closed += MainWindow_Closed;

            InitializeComponent();

            SetProcesss();

            try
            {
                var userSettings = settings.Load();

                settings.InputLanguage = userSettings.InputLanguage;

                settings.OutputLanguage = userSettings.OutputLanguage;

                settings.Color = userSettings.Color;

                settings.FontSize = userSettings.FontSize;

                settings.FromMicrophone = userSettings.FromMicrophone;

                settings.ReadyPhrases = userSettings.FromMicrophone;

                inputCombox.SelectedValue = userSettings.InputLanguageName;

                outputCombox.SelectedValue = userSettings.OutputLanguageName;

                colorComboBox.SelectedValue = settings.Color;

                fontSizeComboBox.SelectedValue = settings.FontSize;

                fromMicrophone.IsChecked = userSettings.FromMicrophone;

                readyPhrases.IsChecked = userSettings.ReadyPhrases;

                settings.Opactiy = userSettings.Opactiy;

                opacitySlider.Value = settings.Opactiy;

                settings.OnAllWindows = userSettings.OnAllWindows;

                onAllWindows.IsChecked = userSettings.OnAllWindows;
            }
            catch (Exception ex)
            {
                settings.Color = "White";

                settings.FontSize = 24;

                settings.InputLanguageName = "English";

                settings.InputLanguage = "en-US";

                settings.OutputLanguageName = "Russian";

                settings.OutputLanguage = "ru-RU";

                settings.Save();

                inputCombox.SelectedValue = settings.InputLanguageName;

                outputCombox.SelectedValue = settings.OutputLanguageName;

                colorComboBox.SelectedValue = settings.Color;

                fontSizeComboBox.SelectedValue = settings.FontSize;

            }
        }

        private void ActiveWindow_ActiveWindowChanged(object sender, string windowHeader, IntPtr hwnd)
        {
            if (check && windowHeader != active)
            {
                if (windowHeader == windowComboBox.SelectedItem.ToString())
                {
                    subtitles.Show();

                    if (windowHeader != "Subtitles")
                    {
                        active = windowHeader;
                    }
                }
                else
                {
                    if (windowHeader != "Subtitles")
                    {
                        subtitles.Hide();
                        active = windowHeader;
                    }
                }
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            subtitles.Close();
        }

        private void SetProcesss()
        {
            var processes = Process.GetProcesses();

            var listOfProcces = new List<string>();

            foreach (var process in processes)
            {
                if (process.MainWindowTitle.Length > 0 && process.MainWindowHandle != IntPtr.Zero && process.MainWindowTitle != "Settings")
                {
                    listOfProcces.Add(process.MainWindowTitle);
                }
            }

            windowComboBox.ItemsSource = listOfProcces;

            windowComboBox.SelectedItem = listOfProcces[0];

        }


        private async Task Start()
        {
            ProfanityOption profanityOption = ProfanityOption.Raw;
            var speechConfig = SpeechConfig.FromSubscription("secret key", "eastus");
            speechConfig.SetProfanity(profanityOption);
            string deviceID = "";

            if (settings.FromMicrophone)
            {
                var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
                while (check)
                {
                    await FromDevice(speechConfig, audioConfig);
                }
            }
            else
            {
                await Task.Run(() =>
                {
                    deviceID = AudioDevices.GetDeviceID()["CABLE Output (VB-Audio Virtual Cable)"];
                });

                var audioConfig = AudioConfig.FromMicrophoneInput(deviceID);

                while (check)
                {
                    await FromDevice(speechConfig, audioConfig);

                }
            }
        }

        private async Task FromDevice(SpeechConfig speechConfig, AudioConfig audioConfig)
        {
            subtitles.textBox1.FontSize = settings.FontSize;

            subtitles.textBox1.Foreground = SetColor(settings.Color);

            using (audioConfig)
            {
                using var recognizer = new SpeechRecognizer(speechConfig, settings.InputLanguage, audioConfig);

                if (settings.ReadyPhrases)
                {
                    recognizer.Recognized += async (o, e) =>
                    {
                        string res = await Translator.Translate(Resultator.GetResult(e), settings.OutputLanguage);
                        if (!String.IsNullOrWhiteSpace(res))
                        {
                            Action action = new Action(() =>
                            {
                                subtitles.textBox1.Text = res;
                                history.Add(res);

                            });

                            await subtitles.Dispatcher.BeginInvoke(action);
                        }
                    };
                    await recognizer.StartContinuousRecognitionAsync();

                    while (check)
                    {
                        await Task.Delay(1000);
                    }

                    await recognizer.StopContinuousRecognitionAsync();

                }
                else
                {
                    recognizer.Recognizing += async (o, e) =>
                    {
                        Action action = new Action(async () =>
                        {
                            subtitles.textBox1.Text = await Translator.Translate(Resultator.GetResult(e), settings.OutputLanguage);

                        });

                        await subtitles.Dispatcher.BeginInvoke(action);

                    };

                    recognizer.Recognized += async (o, e) =>
                    {
                        string res = await Translator.Translate(Resultator.GetResult(e), settings.OutputLanguage);
                        if (!String.IsNullOrWhiteSpace(res))
                        {
                            history.Add(res);
                        }
                    };

                    await recognizer.StartContinuousRecognitionAsync();

                    while (check)
                    {
                        await Task.Delay(1000);
                    }

                    await recognizer.StopContinuousRecognitionAsync();
                }
            }

        }

        private string SetLanguage(string language)
        {
            switch (language)
            {
                case "Arabic":
                    return "ar-SA";
                case "Bulgarian":
                    return "bg-BG";
                case "Catalan":
                    return "ca-ES";
                case "Chinese Simplified":
                    return "zh-Hans";
                case "Chinese Traditional":
                    return "zh-Hant";
                case "Croatian":
                    return "ru-RU";
                case "Czech":
                    return "cs-CZ";
                case "Danish":
                    return "da-DK";
                case "Dutch":
                    return "nl-NL";
                case "English":
                    return "en-US";
                case "Estonian":
                    return "et-EE";
                case "Finnish":
                    return "fi-FI";
                case "French":
                    return "fr-FR";
                case "German":
                    return "de-DE";
                case "Greek":
                    return "el-GR";
                case "Gujarati":
                    return "gu-IN";
                case "Hebrew":
                    return "he-IL";
                case "Hindi":
                    return "hi-IN";
                case "Hungarian":
                    return "hu-HU";
                case "Indonesian":
                    return "id-ID";
                case "Irish":
                    return "ga-IE";
                case "Italian":
                    return "it-IT";
                case "Japanese":
                    return "ja-JP";
                case "Korean":
                    return "ko-KR";
                case "Latvian":
                    return "lv-LV";
                case "Lithuanian":
                    return "it-LT";
                case "Malay":
                    return "ms-MY";
                case "Maltese":
                    return "mt-MT";
                case "Norwegain":
                    return "nb-NO";
                case "Polish":
                    return "pl-PL";
                case "Portuguese(Brazil)":
                    return "pt-BR";
                case "Portuguese(Portugal)":
                    return "pt-PT";
                case "Romanian":
                    return "ro-RO";
                case "Russian":
                    return "ru-RU";
                case "Slovak":
                    return "sk-SK";
                case "Slovenian":
                    return "sl-SL";
                case "Spanish":
                    return "es-ES";
                case "Swedish":
                    return "sv-SE";
                case "Thai":
                    return "th-TH";
                case "Turkish":
                    return "tr-TR";
                case "Vietnamese":
                    return "vi-VN";
                default:
                    return "en-US";
            }
        }

        private SolidColorBrush SetColor(string color)
        {
            switch (color)
            {
                case "White": return Brushes.White;

                case "Black": return Brushes.Black;

                case "Red": return Brushes.Red;

                case "Green": return Brushes.Green;

                case "Blue": return Brushes.Blue;

                case "Purple": return Brushes.Purple;

                case "Orange": return Brushes.Orange;

                default: return Brushes.White;
            }
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {

            if (check)
            {
                check = false;

                subtitles.Hide();

                startButton.Content = "Start";

                if (!settings.OnAllWindows)
                {
                    activeWindow.ActiveWindowChanged -= ActiveWindow_ActiveWindowChanged;
                }
            }
            else
            {
                check = true;

                subtitles = new Subtitles();

                startButton.Content = "Stop";

                subtitles.textBox1.IsReadOnly = true;

                subtitles.textBox1.Background = new SolidColorBrush() { Color = Colors.White, Opacity = settings.Opactiy };

                subtitles.textBox1.TextWrapping = TextWrapping.Wrap;

                int height = (int)SystemParameters.PrimaryScreenHeight;

                int width = (int)SystemParameters.PrimaryScreenWidth;

                subtitles.Height = height / 14;

                subtitles.Width = width / 2;

                subtitles.Top = height - subtitles.Height - 80;

                subtitles.Left = width / 4;

                if (settings.OnAllWindows)
                {
                    subtitles.Show();
                }
                else
                {
                    activeWindow.ActiveWindowChanged += ActiveWindow_ActiveWindowChanged;
                }

                Start();

            }

        }

        private void applyButton_Click(object sender, RoutedEventArgs e)
        {
            settings.FromMicrophone = (bool)fromMicrophone.IsChecked;

            settings.InputLanguageName = inputCombox.Text;

            settings.InputLanguage = SetLanguage(inputCombox.Text);

            settings.OutputLanguageName = outputCombox.Text;

            settings.OutputLanguage = SetLanguage(outputCombox.Text);

            settings.FontSize = int.Parse(fontSizeComboBox.Text);

            settings.Color = colorComboBox.Text;

            settings.ReadyPhrases = (bool)readyPhrases.IsChecked;

            applyButton.IsEnabled = false;

            settings.Opactiy = opacitySlider.Value;

            settings.OnAllWindows = (bool) onAllWindows.IsChecked;

            settings.Save();
        }

        private void fromMicrophone_Checked(object sender, RoutedEventArgs e)
        {
            applyButton.IsEnabled = true;
        }

        private void outputCombox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            applyButton.IsEnabled = true;
        }

        private void inputCombox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            applyButton.IsEnabled = true;
        }

        private void historyButton_Click(object sender, RoutedEventArgs e)
        {
            HistoryWindow historyWindow = new HistoryWindow(history);

            historyWindow.Show();
        }

        private void colorComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            applyButton.IsEnabled = true;
        }

        private void fontSizeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            applyButton.IsEnabled = true;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            applyButton.IsEnabled = true;
        }

        private void opacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            applyButton.IsEnabled = true;
        }

        private void onAllWindows_Checked(object sender, RoutedEventArgs e)
        {
            applyButton.IsEnabled = true;
        }
    }
}
