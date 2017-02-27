using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Speech.Synthesis;
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
    /// Interaction logic for MainMenuWIthLights.xaml
    /// </summary>
    public partial class MainMenuWIthLights : Window
    {
        public User user { get; set; }
        string SmarthomeServerAPIURL = String.Empty;
        public MainMenuWIthLights(User user)
        {
            InitializeComponent();
            this.user = user;
            if (String.Equals(user.gender, "male"))
            {
                lblTitle.Content = "Welcome to the application Mr. " + user.name;
            }
            else
            {
                lblTitle.Content = "Welcome to the application Mrs. " + user.name;
            }

            talk(lblTitle.Content.ToString());

            if (user.defaulthometemprature != null)
            {
                txtSetHouseTemperature.Text = user.defaulthometemprature.ToString();
            }

            SmarthomeServerAPIURL = "http://smarterhomeserveriot.azurewebsites.net/api/{0}";
        }

        SpeechSynthesizer synth = null;
        private async void talk(string words)
        {
            if (synth != null)
                synth.Dispose();

            synth = new SpeechSynthesizer();

            synth.SetOutputToDefaultAudioDevice();

            // Speak a string synchronously.
            synth.SpeakAsync(words);
        }

        bool livingRoomOn = false;
        private void btnTurnOnLivingRoom_Click(object sender, RoutedEventArgs e)
        {
            if (!livingRoomOn)
            {
                btnTurnOnLivingRoom.Content = "Turn off the lights in the living room";
                turnOnOffLights(true, "living room");
                livingRoomOn = true;
            }
            else
            {
                btnTurnOnLivingRoom.Content = "Turn on the lights in the living room";
                turnOnOffLights(false, "living room");
                livingRoomOn = false;
            }
            
        }

        bool bedroomOn = false;
        private void btnTurnOnBedroom_Click(object sender, RoutedEventArgs e)
        {
            if (!bedroomOn)
            {
                btnTurnOnBedroom.Content = "Turn off the lights in the bedroom";
                turnOnOffLights(true, "bedroom");
                bedroomOn = true;
            }
            else
            {
                btnTurnOnBedroom.Content = "Turn on the lights in the bedroom";
                turnOnOffLights(false, "bedroom");
                bedroomOn = false;
            }
        }

        public async Task<string> turnOnOffLights(bool turnOn, string roomType)
        {
            string queryString = String.Format("lights?{0}&{1}", "turnon=" + turnOn.ToString(), "roomtype=" + roomType);
            string requestURL = String.Format(SmarthomeServerAPIURL, queryString);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestURL);
            WebResponse response = await request.GetResponseAsync();
            //WebResponse response = await request.GetResponseAsync();
            //Stream resStream = response.GetResponseStream();            
            return string.Empty;
        }

        private void txtSetHouseTemperature_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSetHouseTemperature.Text == "Set House Temperature")
            {
                txtSetHouseTemperature.Text = "";
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserTableDataContext dataContext = new UserTableDataContext();
                user = dataContext.Users.SingleOrDefault(x => x.username == user.username);
                user.defaulthometemprature = int.Parse(txtSetHouseTemperature.Text);
                dataContext.SubmitChanges();
                MessageBox.Show("Temperature setting has been updated!", "Successful");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Invalid temperature format!\n" + ex.Message, "Failed", MessageBoxButton.OK);
            }

        }
    }
}
