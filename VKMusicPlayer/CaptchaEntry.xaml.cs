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

namespace VKMusicPlayer
{
    /// <summary>
    /// Логика взаимодействия для CaptchaEntry.xaml
    /// </summary>
    public partial class CaptchaEntry : Window
    {
        public static string CaptchaText = "";
        public static Uri Img;
        public CaptchaEntry()
        {
            InitializeComponent();
            MainImage.Source = new BitmapImage(Img);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CaptchaText = TextBox.Text;
            DialogResult = true;
        }
    }
}
