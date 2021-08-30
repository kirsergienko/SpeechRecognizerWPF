using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SpeechRecognizerWPF
{
    /// <summary>
    /// Interaction logic for HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {
        private List<string> history;

        public HistoryWindow(List<string> vs)
        {
            InitializeComponent();

            history = vs;

            this.ShowInTaskbar = false;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Textbox.TextWrapping = TextWrapping.Wrap;

            foreach (string text in history)
            {
                Textbox.Text += text + "\n";
            }
        }
    }
}
