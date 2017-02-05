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

namespace WPFLogin
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        public User user { get; set; }
        public MainMenu(User user)
        {
            InitializeComponent();
            this.user = user;
            if (String.Equals(user.gender, "male"))
            {
                label.Content = "Welcome to the application Mr. " + user.name;
            }
            else
            {
                label.Content = "Welcome to the application Mrs. " + user.name;
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }

        private void btnSeePeopleAtHome_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
