using System;
using System.Collections.Generic;
using System.Linq;
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
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using VkNet.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace VKMusicPlayer
{
    /// <summary>
    /// Логика взаимодействия для TwoFactorAuth.xaml
    /// </summary>
    public partial class TwoFactorAuth : Window
    {
        public string TwoFactorCode = "";
        public TwoFactorAuth()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TwoFactorCode = TwoFactorCodeTextBox.Text;
            if (TwoFactorCode != "")
            {
                DialogResult = true;
            } 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TwoFactorCodeTextBox.Focus();
        }
    }
}
