using Flurl.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VKMusicPlayer;
using VkNet.Enums.Filters;
using VkNet.Model.Attachments;
using VkNet.Utils;

namespace VKMusicPlayer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string Version = "1.0.0";
        public static VK vk = new VK();
        public static Config config = new Config("Config.xml");
        public static VkCollection<Audio> AudioList;
        public static Random random = new Random();
        MediaPlayer Player = new MediaPlayer();
        List<Thread> ThreadsList = new List<Thread>();
        bool DurationSliderPressed = false;
        int CurrentTrackID = -1;
        int RandomCurrentTrackID = -1;
        List<int> RandomTrackIDList = new List<int>();
        bool RandomPlay = false;
        bool RepeatPlay = false;
        bool isPaused = false;
        SettingsWindow settingsWindow = new SettingsWindow();
        public MainWindow()
        {
            InitializeComponent();
            Closing += new CancelEventHandler(Window_Closing);
            TitleLabel.Content = "";
            ArtistLabel.Content = "";
            AlbumNameLabel.Content = "";
            if (!Directory.Exists("Music"))
            {
                Directory.CreateDirectory("Music");
            }
            Player.MediaOpened += Player_MediaOpened; ;
            Player.MediaEnded += Player_MediaEnded;
            DurationSlider.PreviewMouseDown += DurationSlider_MouseDown;
            DurationSlider.PreviewMouseUp += DurationSlider_MouseUp;
            Thread AlbumsLoadThread = new Thread(AlbumsLoad);
            Thread SetCurrentTimeThread = new Thread(SetCurrentTime);
            ThreadsList.Add(AlbumsLoadThread);
            ThreadsList.Add(SetCurrentTimeThread);
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AudioList = vk.GetAudio();
            foreach (Thread item in ThreadsList)
            {
                item.Start();
            }
            foreach (var audio in AudioList)
            {
                Button button = new Button()
                {
                    Content = $"{audio.Title}",
                    Width = 185
                };
                button.Tag = audio.Id.ToString();
                button.Click += MusicClick;
                ToolTip toolTip = new ToolTip();
                StackPanel toolTipPanel = new StackPanel();
                toolTipPanel.Children.Add(new TextBlock { Text = $"{audio.Title} {audio.Subtitle}", FontSize = 16 });
                toolTipPanel.Children.Add(new TextBlock { Text = audio.Artist });
                toolTip.Content = toolTipPanel;
                button.ToolTip = toolTip;
                button.Foreground = Brushes.White;
                button.Background = Brushes.Gray;
                MusicListBox.Items.Add(button);
            }
            MusicListBox.ScrollIntoView(MusicListBox.Items[0]);
        }
        private void MusicClick(object sender, EventArgs e)
        {
            int Index = MusicListBox.Items.IndexOf(sender);
            //PlayMusic(Index);
            Task.Run(() => PlayMusic(Index));
        }
        private async void PlayMusic(int ID)
        {
            Dispatcher.Invoke(() => Player.Stop());
            Audio audio = AudioList[ID];
            CurrentTrackID = ID;
            if (RandomPlay)
            {
                RandomCurrentTrackID = RandomTrackIDList.IndexOf(ID);
            }
            string Subtitle = audio.Subtitle;
            Dispatcher.Invoke(() => TitleLabel.Content = audio.Title + " " + audio.Subtitle);
            Dispatcher.Invoke(() => ArtistLabel.Content = audio.Artist);
            if (audio.Album != null)
            {
                Dispatcher.Invoke(() => AlbumNameLabel.Content = audio.Album.Title);
                Dispatcher.Invoke(() => AlbumImage.Source = SetImage($"Albums\\{audio.Album.Id}.png"));
            }
            else
            {
                Dispatcher.Invoke(() => AlbumNameLabel.Content = "Отсутствует");
                Dispatcher.Invoke(() => AlbumImage.Source = null);
            }
            await Task.Run(() => LoadMusic(ID));
            if (CurrentTrackID == ID)
            {
                try
                {
                    Dispatcher.Invoke(() => Player.Open(new Uri($"{Directory.GetCurrentDirectory()}\\Music\\{audio.Title}_{Subtitle}_{audio.Artist}.mp3")));
                    Dispatcher.Invoke(() => Player.Play());
                }
                catch { }
            }
        }
        public static async Task LoadMusic(int ID)
        {
            Audio audio = AudioList[ID];
            string Subtitle = audio.Subtitle;
            string Title = audio.Title;
            string Artist = audio.Artist;
            string FileName;
            if (!File.Exists($"Music\\{Title}_{Subtitle}_{Artist}.mp3"))
            {
                try
                {
                    FileName = $"{Title.Substring(0, 2)}{Artist.Substring(0, 2)}{random.Next(0, 999999)}";
                }
                catch
                {
                    FileName = $"{random.Next(0, 999999)}";
                }
                ExecuteCommand($"streamlink {audio.Url} live --output {FileName}.ts");
                ExecuteCommand($"ffmpeg -i {FileName}.ts -vn -ar 44100 -ac 2 -b:a 192k {FileName}.mp3");
                try
                {
                    File.Delete($"{FileName}.ts");
                }
                catch { }
                try
                {
                    File.Move($"{FileName}.mp3", $"Music\\{Title}_{Subtitle}_{Artist}.mp3");
                }
                catch { }
            }
        }
        private void AlbumsLoad()
        {
            WebClient webClient = new WebClient();
            if (!Directory.Exists("Albums"))
            {
                Directory.CreateDirectory("Albums");
            }
            foreach (var item in AudioList)
            {
                if (item.Album != null)
                {
                    if (!File.Exists($"Albums\\{item.Album.Id}"))
                    {
                        try
                        {
                            webClient.DownloadFile(item.Album.Thumb.Photo600, $"Albums\\{item.Album.Id}.png");
                        }
                        catch { }
                    }
                }
            }
        }
        private void Player_MediaOpened(object sender, EventArgs e)
        {
            SetEndTime(sender, e);
            PlayImage.Source = new BitmapImage(new Uri(@"/Resources/Pause.png", UriKind.Relative));
            isPaused = false;
            Task.Run(() => LoadMusic(GetNextID()));
        }

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            if (!RepeatPlay)
            {
                Task.Run(() => PlayMusic(GetNextID()));
            }
            else
            {
                Task.Run(() => PlayMusic(CurrentTrackID));
            }
        }
        private int GetNextID()
        {
            int NextTrackID = 0;
            if (!RandomPlay)
            {
                if (CurrentTrackID + 1 < MusicListBox.Items.Count)
                {
                    NextTrackID = CurrentTrackID + 1;
                }
            }
            else
            {
                if (RandomCurrentTrackID + 1 < RandomTrackIDList.Count)
                {
                    NextTrackID = RandomTrackIDList[RandomCurrentTrackID + 1];
                }
                else
                {
                    NextTrackID = RandomTrackIDList[0];
                }
            }
            return NextTrackID;
        }
        private int GetLastID()
        {
            int LastTrackID = 0;
            if (!RandomPlay)
            {
                if (CurrentTrackID - 1 != -1)
                {
                    LastTrackID = CurrentTrackID - 1;
                }
                else
                {
                    LastTrackID = MusicListBox.Items.Count - 1;
                }
            }
            else
            {
                if (RandomCurrentTrackID - 1 != -1)
                {
                    LastTrackID = RandomTrackIDList[RandomCurrentTrackID - 1];
                }
                else
                {
                    LastTrackID = RandomTrackIDList[RandomTrackIDList.Count - 1];
                }
            }
            return LastTrackID;
        }
        private void SetEndTime(object Object, EventArgs e)
        {
            int Seconds = (int)Player.NaturalDuration.TimeSpan.TotalSeconds;
            DurationSlider.Maximum = Seconds;
            int Minutes = Seconds / 60;
            Seconds -= Minutes * 60;
            string strSeconds = $"{Seconds}";
            if (Seconds < 10)
            {
                strSeconds = $"0{Seconds}";
            }
            string EndTime = $"{Minutes}:{strSeconds}";
            TimeEndInfoLabel.Content = EndTime;
        }
        private void SetCurrentTime()
        {
            while (true)
            {
                try
                {
                    if (!DurationSliderPressed)
                    {
                        if (Dispatcher.Invoke(() => Player.HasAudio))
                        {
                            int Seconds = Dispatcher.Invoke(() => (int)Player.Position.TotalSeconds);
                            Dispatcher.Invoke(() => DurationSlider.Value = Seconds);
                            int Minutes = Seconds / 60;
                            Seconds -= Minutes * 60;
                            string strSeconds = $"{Seconds}";
                            if (Seconds < 10)
                            {
                                strSeconds = $"0{Seconds}";
                            }
                            string CurrentTime = $"{Minutes}:{strSeconds}";
                            Dispatcher.Invoke(() => TimeCurrentInfoLabel.Content = CurrentTime);
                            Thread.Sleep(125);
                        }
                    }
                }
                catch { }
            }
        }
        private void DurationSlider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Player.Position = TimeSpan.FromSeconds(DurationSlider.Value);
            DurationSliderPressed = false;
        }

        private void DurationSlider_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DurationSliderPressed = true;
            int Seconds = (int)DurationSlider.Value;
            int Minutes = Seconds / 60;
            Seconds -= Minutes * 60;
            string strSeconds = $"{Seconds}";
            if (Seconds < 10)
            {
                strSeconds = $"0{Seconds}";
            }
            string CurrentTime = $"{Minutes}:{strSeconds}";
            TimeCurrentInfoLabel.Content = CurrentTime;
        }
        private List<int> GenerateRandomQueue(int FirstTrackID = -1)
        {
            List<int> RandomQueue = new List<int>();
            List<int> DefaultQueue = new List<int>();
            for (int i = 0; i < MusicListBox.Items.Count; i++)
            {
                DefaultQueue.Add(i);
            }
            while (DefaultQueue.Count != 0)
            {
                int Index = random.Next(0, DefaultQueue.Count);
                RandomQueue.Add(DefaultQueue[Index]);
                DefaultQueue.RemoveAt(Index);
            }
            if (FirstTrackID != -1)
            {
                RandomQueue.Remove(FirstTrackID);
                RandomQueue.Insert(0, FirstTrackID);
            }
            return RandomQueue;
        }
        public static void ExecuteCommand(string command)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow= true;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C " + command;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
        public static BitmapImage SetImage(string filepath)
        {
            var bi = new BitmapImage();

            if (File.Exists(filepath))
            {
                using (var fs = new FileStream(filepath, FileMode.Open))
                {
                    bi.BeginInit();
                    bi.StreamSource = fs;
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.EndInit();
                }

                bi.Freeze(); //Important to freeze it, otherwise it will still have minor leaks
            }
            else
            {
                bi = null;
            }

            return bi;
        }
        private void ChangePauseState()
        {
            if (CurrentTrackID != -1)
            {
                isPaused = !isPaused;
                if (isPaused)
                {
                    PlayImage.Source = new BitmapImage(new Uri(@"/Resources/Play.png", UriKind.Relative));
                    Player.Pause();
                }
                else
                {
                    PlayImage.Source = new BitmapImage(new Uri(@"/Resources/Pause.png", UriKind.Relative));
                    Player.Play();
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            foreach (var item in Directory.GetFiles(Directory.GetCurrentDirectory()))
            {
                if (item.Contains(".ts") || item.Contains(".mp3"))
                {
                    try
                    {
                        File.Delete(item);
                    }
                    catch { }
                }
            }
            ExecuteCommand($"Taskkill /PID {Process.GetCurrentProcess().Id} /F");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentTrackID != -1)
            {
                Task.Run(() => PlayMusic(GetLastID()));
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            ChangePauseState();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => PlayMusic(GetNextID()));
        }

        private void Random_Click(object sender, RoutedEventArgs e)
        {
            RandomPlay = !RandomPlay;
            if (RandomPlay)
            {
                Random.Opacity = 1;
                RandomTrackIDList = GenerateRandomQueue(CurrentTrackID);
                if (CurrentTrackID == -1)
                {
                    RandomCurrentTrackID = 0;
                    Task.Run(() => PlayMusic(GetNextID()));
                }
                else
                {
                    RandomCurrentTrackID = RandomTrackIDList.IndexOf(CurrentTrackID);
                    Task.Run(() => LoadMusic(GetNextID()));
                }
            }
            else
            {
                Task.Run(() => LoadMusic(GetNextID()));
                Random.Opacity = 0.5;
            }
        }

        private void Repeat_Click(object sender, RoutedEventArgs e)
        {
            RepeatPlay = !RepeatPlay;
            if (RepeatPlay)
            {
                Repeat.Opacity = 1;
            }
            else
            {
                Repeat.Opacity = 0.5;
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!settingsWindow.IsActive)
                {
                    settingsWindow.Show();
                }
            }
            catch
            {
                settingsWindow = new SettingsWindow();
                settingsWindow.Show();
            }
        }
    }
}
