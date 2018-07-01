using System;
using System.Windows;
using Microsoft.Win32;
using System.Linq;

namespace StreamPlayerDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        System.Threading.Thread Tmsg;
        public MainWindow()
        {
            InitializeComponent();
            Tmsg = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                while (true)
                {
                    //System.Threading.Thread.Sleep(2000);
                    string command = Console.ReadLine();
                    Console.WriteLine("command recieved: {0}", command);
                    string[] cmds = command.Split(' ');
                    switch(cmds[0])
                    {
                        case "play":
                            Dispatcher.Invoke(() =>
                            {
                                HandlePlayButtonClick(null, null);
                            });
                            break;
                        case "setPosition":
                            Dispatcher.Invoke(() =>
                            {
                                SetPosition(cmds);
                            });
                            break;
                        default:
                            Console.WriteLine("Command not supported.");
                            break;
                    }
                }
            }));
            Tmsg.Start();
        }

        private void SetPosition(string[] cmds)
        {
            string[] d = new string[cmds.Length - 1];
            Array.Copy(cmds, 1, d, 0, cmds.Length - 1);
            int[] values = Array.ConvertAll(d, int.Parse);
            this.Top = values[0];
            this.Left = values[1];
            this.Width = values[2];
            this.Height = values[3];
            BringIntoView();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Tmsg.Abort();
        }

        private void HandlePlayButtonClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("command executing: Connecting {0}", _urlTextBox.Text);
            var uri = new Uri(_urlTextBox.Text);
            try
            {
                _streamPlayerControl.StartPlay(uri);
                _statusLabel.Text = "Connecting...";
            }
            catch (Exception ex)
            {
                Console.WriteLine("command Failed: Connecting {0}", ex);
            }
        }

        private void HandleStopButtonClick(object sender, RoutedEventArgs e)
        {
            _streamPlayerControl.Stop();
        }

        private void HandleImageButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog { Filter = "Bitmap Image|*.bmp" };
            if (dialog.ShowDialog() == true)
            {
                _streamPlayerControl.GetCurrentFrame().Save(dialog.FileName);
            }
        }

        private void UpdateButtons()
        {
            //    _playButton.IsEnabled = !_streamPlayerControl.IsPlaying; 
            //    _stopButton.IsEnabled = _streamPlayerControl.IsPlaying;
            //    _imageButton.IsEnabled = _streamPlayerControl.IsPlaying;
        }

        private void HandlePlayerEvent(object sender, RoutedEventArgs e)
        {
            UpdateButtons();

            if (e.RoutedEvent.Name == "StreamStarted")
            {
                _statusLabel.Text = "Playing";
            }
            else if (e.RoutedEvent.Name == "StreamFailed")
            {
                _statusLabel.Text = "Failed";

                MessageBox.Show(
                    ((WebEye.Controls.Wpf.StreamPlayerControl.StreamFailedEventArgs)e).Error,
                    "Stream Player Demo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else if (e.RoutedEvent.Name == "StreamStopped")
            {
                _statusLabel.Text = "Stopped";
            }
        }
    }
}
