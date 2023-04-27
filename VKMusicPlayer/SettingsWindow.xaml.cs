using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VKMusicPlayer
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        bool LoadedTracksCounterAbort;
        Thread DownloadTracksThread;
        Thread LoadedTrackCounterThread;
        public SettingsWindow()
        {
            InitializeComponent();
            Closing += SettingsWindow_Closing;
            DownloadTracksThread = new Thread(DownloadTracks);
            LoadedTrackCounterThread = new Thread(LoadedTrackCounter);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TrackCountLabel.Content = MainWindow.AudioList.Count.ToString();
            LoadedTrackCountLabel.Content = GetCountDownloadedTracks().ToString();
            LoadedTracksCounterAbort = false;
            LoadedTrackCounterThread.Start();
            VersionLabel.Content = $"ver {MainWindow.Version}";
        }
        private int GetCountDownloadedTracks()
        {
            int countDownloadedTracks = 0;
            DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Music");
            foreach (FileInfo file in di.GetFiles())
            {
                if (file.Name.Contains(".mp3"))
                {
                    countDownloadedTracks++;
                }
            }
            return countDownloadedTracks;
        }

        private void DownloadTracksButton_Click(object sender, RoutedEventArgs e)
        {
            DownloadTracksThread.Start();
        }

        private void DownloadTracks()
        {
            int Delay = 0;
            Dispatcher.Invoke(() => ChangeStateLoadButtonAndDelayTextBox(false));
            Dispatcher.Invoke(() => Delay = int.Parse(DownloadDelayTextBox.Text) * 1000);
            for (int i = 0; i < MainWindow.AudioList.Count; i++)
            {
                Task.Run(() => MainWindow.LoadMusic(i));
                Thread.Sleep(Delay);
            }
            Dispatcher.Invoke(() => ChangeStateLoadButtonAndDelayTextBox(true));
        }
        private void LoadedTrackCounter()
        {
            try
            {
                while (true)
                {
                    if (!LoadedTracksCounterAbort)
                    {
                        Dispatcher.Invoke(() => LoadedTrackCountLabel.Content = GetCountDownloadedTracks().ToString());
                        Thread.Sleep(125);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch { }
        }

        private void ChangeStateLoadButtonAndDelayTextBox(bool state)
        {
            DownloadTracksButton.IsEnabled = state;
            DownloadDelayTextBox.IsEnabled = state;
        }

        private void DownloadDelayTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
            if (DownloadDelayTextBox.Text == "")
            {
                DownloadDelayTextBox.Text = "0";
            }
        }

        private void SettingsWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            LoadedTracksCounterAbort = true;
            Thread.Sleep(250);
        }

        private void ExitAccountButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.config.VKLogin = "";
            MainWindow.config.VKPassword = "";
            MainWindow.ExecuteCommand($"Taskkill /PID {Process.GetCurrentProcess().Id} /F");
        }
    }
}
