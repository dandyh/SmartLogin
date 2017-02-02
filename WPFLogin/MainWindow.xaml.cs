﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFLogin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text == "Username")
            {
                txtUsername.Text = "";
            }

        }

        public void txtPass_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password == "xxxxxxx")
            {
                txtPassword.Password = "";
            }

        }

        private void btnNormalLogin_Click(object sender, RoutedEventArgs e)
        {
            if (String.Equals(txtUsername.Text, "Username") || String.Equals(txtUsername.Text.Trim(), ""))
            {
                MessageBox.Show("Please enter your username!", "Error", MessageBoxButton.OK);
            }
            else
            {
                //Login
                UserTableDataContext dataContext = new UserTableDataContext();
                var usr = dataContext.Users.SingleOrDefault(x => x.username == txtUsername.Text && x.password == txtPassword.Password);
                if(usr != null)
                {
                    MessageBox.Show("Login successful", "Login successful", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show("Login Failed", "Login Failed", MessageBoxButton.OK);
                }
            }

        }

        private void btnFaceLogin_Click(object sender, RoutedEventArgs e)
        {
            if (String.Equals(txtUsername.Text, "Username") || String.Equals(txtUsername.Text.Trim(), ""))
            {
                MessageBox.Show("Please enter your username!", "Error", MessageBoxButton.OK);
                return;
            }
            
            LoginWIthFaceRecognition ss = new LoginWIthFaceRecognition(txtUsername.Text);
            ss.Show();
            this.Close();
            

        }

        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            UserRegistration ss = new UserRegistration();
            ss.Show();
            this.Close();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            UserRegistrationAudio ss = new UserRegistrationAudio(txtUsername.Text);
            ss.Show();
            this.Close();
        }
        public int counter;

        private void btnSpeakerRecognitionLogin_Click(object sender, RoutedEventArgs e)
        {
            if (String.Equals(txtUsername.Text, "Username") || String.Equals(txtUsername.Text.Trim(), ""))
            {
                MessageBox.Show("Please enter your username!", "Error", MessageBoxButton.OK);
                return;
            }
            LoginWithSpeakerRecognition ss = new LoginWithSpeakerRecognition(txtUsername.Text);
            ss.Show();
            this.Close();
        }
    }
}
