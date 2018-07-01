using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AppParrent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool init = false;
        Process Others = new Process();
        IntPtr id;
        public MainWindow()
        {
            InitializeComponent();
            LocationChanged += MainWindow_LocationChanged;
            SizeChanged += MainWindow_SizeChanged;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SendCommand("setPosition", $"{(int)Application.Current.MainWindow.Top + 28}", $"{(int)Application.Current.MainWindow.Left}", $"{(int)Application.Current.MainWindow.Width}", $"{(int)Application.Current.MainWindow.Height}");
        }

        private void SendCommand(params string[] args)
        {
            if (!init) return;
            string ex = string.Join(" ", args);
            Console.WriteLine("Sending Command: {0}", ex);
            Others?.StandardInput.WriteLine(ex);
        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            if (!init) return;
            SendCommand("setPosition", $"{(int)Application.Current.MainWindow.Top + 28}", $"{(int)Application.Current.MainWindow.Left}", $"598", "600");
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            Others.StartInfo = new ProcessStartInfo(@"C:\Users\mehdadi\Desktop\WebEye\StreamPlayerControl\WPF\StreamPlayerDemo\StreamPlayerDemo\bin\Debug\StreamPlayerDemo.exe");
            Others.StartInfo.RedirectStandardError = Others.StartInfo.RedirectStandardInput = Others.StartInfo.RedirectStandardOutput = true;
            Others.StartInfo.UseShellExecute = false;
            Others.Start();
            System.Threading.Thread.Sleep(2000);
            id = Others.MainWindowHandle;

            Title = "Running";
            init = true;
            MainWindow_LocationChanged(null, null);
            Others.Exited += Others_Exited;

            Console.Write(id);
            thr = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    //System.Threading.Thread.Sleep(2000);
                    string cmd = Others.StandardOutput.ReadLine();
                    Console.WriteLine(cmd);
                }
            }));
            thr.Start();

        }

        private void Others_Exited(object sender, EventArgs e)
        {

        }

        Thread thr;
        protected override void OnClosed(EventArgs e)
        {
            if (!init) return;
            Others.Kill();
            thr.Abort();
            base.OnClosed(e);
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("play");
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("stop");
        }
    }
}
