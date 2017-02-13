using Microsoft.CognitiveServices.SpeechRecognition;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
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

        private MicrophoneRecognitionClient micClient;
        //Voice recognition functions
        private string _speechsubscriptionKey = ConfigurationSettings.AppSettings.Get("speechapikey");
        private string AuthenticationUri = ConfigurationSettings.AppSettings["AuthenticationUri"];

        private bool isResponded = false;
        public User user { get; set; }
        public MainMenu(User user)
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
            lblTitle.Content += "\nWhat can I do for you?";
            talk(lblTitle.Content.ToString());
            startMicRecog();
        }

        protected override void OnClosed(EventArgs e)
        {

            if (null != this.micClient)
            {
                this.micClient.Dispose();
            }

            base.OnClosed(e);
        }

        private void disposeMic()
        {
            this.micClient.EndMicAndRecognition();
            this.micClient.Dispose();
            this.micClient = null;
        }
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            logout();
        }

        private void logout()
        {
            //GC.Collect();
            disposeMic();
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }

        private void btnSeePeopleAtHome_Click(object sender, RoutedEventArgs e)
        {

        }



        private void CreateMicrophoneRecoClient()
        {
            this.micClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(
                SpeechRecognitionMode.ShortPhrase,
                "en-US", _speechsubscriptionKey);
            this.micClient.AuthenticationUri = this.AuthenticationUri;

            // Event handlers for speech recognition results
            this.micClient.OnMicrophoneStatus += this.OnMicrophoneStatus;

            this.micClient.OnPartialResponseReceived += this.OnPartialResponseReceivedHandler;

            this.micClient.OnResponseReceived += this.OnMicShortPhraseResponseReceivedHandler;

            this.micClient.OnConversationError += this.OnConversationErrorHandler;
        }

        private void OnPartialResponseReceivedHandler(object sender, PartialSpeechResponseEventArgs e)
        {

            this.setResponsePartial("{0}", e.PartialResult);            
        }

        private void setResponsePartial(string format, params object[] args)
        {
            var formattedStr = string.Format(format, args);
            Dispatcher.Invoke(() =>
            {
                lblResponse.Content = (formattedStr + "\n");
            });
        
        }

        private void OnConversationErrorHandler(object sender, SpeechErrorEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                //_startButton.IsEnabled = true;
                //_radioGroup.IsEnabled = true;
            });

            this.WriteLine("--- Error received by OnConversationErrorHandler() ---");
            this.WriteLine("Error code: {0}", e.SpeechErrorCode.ToString());
            this.WriteLine("Error text: {0}", e.SpeechErrorText);
            this.WriteLine();
        }

        /// <summary>
        /// Called when a final response is received;
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SpeechResponseEventArgs"/> instance containing the event data.</param>
        private void OnMicShortPhraseResponseReceivedHandler(object sender, SpeechResponseEventArgs e)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                this.WriteLine("--- OnMicShortPhraseResponseReceivedHandler ---");

                // we got the final result, so it we can end the mic reco.  No need to do this
                // for dataReco, since we already called endAudio() on it as soon as we were done
                // sending all the data.
                this.micClient.EndMicAndRecognition();

                this.WriteResponseResult(e);

                if (!isResponded)
                {

                    startMicRecog();
                }

            }));
        }


        /// <summary>
        /// Writes the response result.
        /// </summary>
        /// <param name="e">The <see cref="SpeechResponseEventArgs"/> instance containing the event data.</param>
        private void WriteResponseResult(SpeechResponseEventArgs e)
        {
            if (e.PhraseResponse.Results.Length == 0)
            {
                this.WriteLine("No phrase response is available.");
            }
            else
            {
                //this.WriteLine("********* Final n-BEST Results *********");
                //for (int i = 0; i < e.PhraseResponse.Results.Length; i++)
                //{
                //    this.WriteLine(
                //        "[{0}] Confidence={1}, Text=\"{2}\"",
                //        i,
                //        e.PhraseResponse.Results[i].Confidence,
                //        e.PhraseResponse.Results[i].DisplayText);
                //    speakResponse(e.PhraseResponse.Results[i].DisplayText);
                //}

                //this.WriteLine();

                for (int i = 0; i < e.PhraseResponse.Results.Length; i++)
                {

                    if (String.Equals(e.PhraseResponse.Results[i].Confidence.ToString(), "High"))
                    {
                        speakResponse(e.PhraseResponse.Results[i].DisplayText);
                        goto ends;
                    }
                }

                //Look for medium recog
                for (int i = 0; i < e.PhraseResponse.Results.Length; i++)
                {

                    if (String.Equals(e.PhraseResponse.Results[i].Confidence.ToString(), "Medium"))
                    {
                        speakResponse(e.PhraseResponse.Results[i].DisplayText);
                        goto ends;
                    }
                }                
            }
            ends:
            return;
        }

        /// <summary>
        /// Called when the microphone status has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MicrophoneEventArgs"/> instance containing the event data.</param>
        private void OnMicrophoneStatus(object sender, MicrophoneEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                WriteLine("--- Microphone status change received by OnMicrophoneStatus() ---");
                WriteLine("********* Microphone status: {0} *********", e.Recording);
                if (e.Recording)
                {
                    WriteLine("Please start speaking.");
                }

                WriteLine();
            });
        }

        private void WriteLine()
        {
            this.WriteLine(string.Empty);
        }

        private void WriteLine(string format, params object[] args)
        {
            var formattedStr = string.Format(format, args);
            Dispatcher.Invoke(() =>
            {
                _logText.Text += (formattedStr + "\n");
                _logText.ScrollToEnd();
            });
        }

        private void startMicRecog()
        {
            this.CreateMicrophoneRecoClient();
            this.micClient.StartMicAndRecognition();
            if (String.IsNullOrEmpty(lblAI.Content.ToString()))
            {
                lblAI.Content = "Waiting for your command....";
            }
            
        }

        private void speakResponse(string text)
        {
            string howWarmQuestion = "How warm do you want your home to be?";
            
            if ((text.ToLower().Contains("degree") || text.ToLower().Contains("temperature")) 
                //(text.ToLower().Contains("has") || text.ToLower().Contains("house") || text.ToLower().Contains("home"))
                )
            {
                string temp = Regex.Match(text.ToLower(), @"\d+").Value;
                if (String.IsNullOrEmpty(temp))
                {
                    lblAI.Content = howWarmQuestion;                    
                }
                else
                {
                    lblAI.Content = "Your default temprature has been set to " + temp + " degrees. Thank you!";                    
                    isResponded = true;
                }
                talk(lblAI.Content.ToString());

            }
            else if (text.ToLower().Contains("log") && text.ToLower().Contains("out")) {
                logout();                
            }
            else
            {
                if (!String.IsNullOrEmpty(Regex.Match(text.ToLower(), @"\d+").Value) && String.Equals(howWarmQuestion, lblAI.Content.ToString()))
                {
                    string temp = Regex.Match(text.ToLower(), @"\d+").Value;
                    lblAI.Content = "Your default temprature has been set to " + temp + " degrees. Thank you!";
                    isResponded = true;
                    talk(lblAI.Content.ToString());
                } else
                {
                    lblAI.Content = "";
                    MessageBox.Show("Please notice: The only feature available at the moment is to set your home temperature.", "Warning", MessageBoxButton.OK);                    
                }
            }

            if (isResponded)
            {
                string temp = Regex.Match(text.ToLower(), @"\d+").Value;
                UserTableDataContext dataContext = new UserTableDataContext();
                User usr = dataContext.Users.SingleOrDefault(x => x.username == user.username);
                usr.defaulthometemprature = int.Parse(temp);
                dataContext.SubmitChanges();
            }
            
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
    }
}
