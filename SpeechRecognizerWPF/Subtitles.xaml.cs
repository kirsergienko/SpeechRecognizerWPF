using System;

using System.Runtime.InteropServices;

using System.Windows;
using System.Windows.Controls;

using System.Windows.Interop;

namespace SpeechRecognizerWPF
{
    /// <summary>
    /// Interaction logic for Subtitles.xaml
    /// </summary>
    public partial class Subtitles : Window
    {
        public Subtitles()
        {
            InitializeComponent();

            Loaded += Sample2_Loaded;

            Deactivated += Sample2_Deactivated;

            this.ShowInTaskbar = false;
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox1.ScrollToEnd();

        }

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1); 
        private const UInt32 SWP_NOSIZE = 0x0001; 
        private const UInt32 SWP_NOMOVE = 0x0002; 
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);


        private void Sample2_Loaded(object sender, RoutedEventArgs e)
        {
            SetWindowPos(new WindowInteropHelper(this).Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }

        private void Sample2_Deactivated(object sender, EventArgs e)
        {
            SetWindowPos(new WindowInteropHelper(this).Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }
    }
}
