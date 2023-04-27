using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using VkNet.AudioBypassService.Extensions;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace VKMusicPlayer
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        bool SavePass = false;
        MainWindow mainWindow = new MainWindow();
        string captchaKey = null;
        long? captchaSid = null;
        public LoginWindow()
        {
            InitializeComponent();
            Closing += new CancelEventHandler(Window_Closing);
            VKSilentLogin();
        }
        private void VKSilentLogin()
        {
            if (MainWindow.config.VKLogin != "")
            {
                try
                {
                    VKLogin(MainWindow.config.VKLogin, MainWindow.config.VKPassword);
                }
                catch
                {
                    MessageBox.Show("Не удалось авторизоваться, проверьте правильность введённых данных или повторите попытку ещё раз.", "VKMusicPlayer", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void VKLogin(string Login, string Password)
        {
            try
            {
                MainWindow.vk.Auth(Login, Password);
                mainWindow.Show();
                this.Close();
            }
            catch (Exception e)
            {
                //MessageBox.Show($"Не удалось авторизоваться, проверьте правильность введённых данных или повторите попытку ещё раз.\n{e.Message}", "VKMusicPlayer", MessageBoxButton.OK, MessageBoxImage.Error);
                ServiceCollection service = new ServiceCollection();
                service.AddAudioBypass();
                VkNet.VkApi vkApi = new VkNet.VkApi(service);
                vkApi.Authorize(new ApiAuthParams
                {
                    Login = Login,
                    Password = Password,
                    ApplicationId = 51502358,
                    Settings = Settings.Audio | Settings.Groups | Settings.Offline,
                    TwoFactorAuthorization = TheSecondAuth
                });
                MainWindow.vk.vkApi = vkApi;
                mainWindow.Show();
                this.Close();
            }
        }

        private void LoginEnter_Click(object sender, RoutedEventArgs e)
        {
            string Login = VKLoginBox.Text;
            string Password = VKPassBox.Password;
            if (SavePass)
            {
                MainWindow.config.VKLogin = Login;
                MainWindow.config.VKPassword = Password;
            }
            VKLogin(Login, Password);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Данные для авторизации будут храниться в папке с программой, если компьютером пользуется кто-то ещё, то не рекомендуется оставлять данную функцию", "VKMusicPlayer", MessageBoxButton.OK, MessageBoxImage.Warning);
            SavePass = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SavePass = false;
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!mainWindow.IsActive)
            {
                MainWindow.ExecuteCommand($"Taskkill /PID {Process.GetCurrentProcess().Id} /F");
            }
        }

        private string TheSecondAuth()
        {
            string result = "";

            Thread t = new Thread(() =>
            {
                TwoFactorAuth dlg = new TwoFactorAuth();

                if (dlg.ShowDialog() == true)
                {
                    result = dlg.TwoFactorCode;
                }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;

            t.Start();
            t.Join();

            return result;
        }
    }
}
