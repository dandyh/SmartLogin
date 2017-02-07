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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.CognitiveServices.SpeechRecognition;
using System.Configuration;
using System.Speech.Synthesis;

namespace WPFLogin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MicrophoneRecognitionClient micClient;
        //Voice recognition functions
        private string _speechsubscriptionKey = ConfigurationSettings.AppSettings.Get("speechapikey");
        private bool resphonsePhraseFound = false;

        public MainWindow()
        {
            InitializeComponent();


            //talk("Welcome to smarter home application");
            this.CreateMicrophoneRecoClient();
            this.micClient.StartMicAndRecognition();
            //For testing
            //txtUsername.Text = "dandy";

            //Test ss = new Test();
            //ss.Show();
            //this.Close();
            //return;
        }

        private async void talk(string words)
        {
            SpeechSynthesizer synth = new SpeechSynthesizer();

            // Configure the audio output. 
            synth.SetOutputToDefaultAudioDevice();

            // Speak a string synchronously.
            synth.SpeakAsync(words);

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
                if (usr != null)
                {
                    MessageBox.Show("Login successful", "Login successful", MessageBoxButton.OK);

                    GC.Collect();
                    MainMenu ss = new MainMenu(usr);
                    ss.Show();
                    this.Close();
                    return;
                }
                else
                {
                    MessageBox.Show("Login Failed", "Login Failed", MessageBoxButton.OK);
                }
            }

        }

        private void btnFaceLogin_Click(object sender, RoutedEventArgs e)
        {

            faceLogin();
        }

        private bool faceLogin()
        {
            if (String.Equals(txtUsername.Text, "Username") || String.Equals(txtUsername.Text.Trim(), ""))
            {
                MessageBox.Show("Please enter your username!", "Error", MessageBoxButton.OK);
                return false;
            }
            GC.Collect();
            LoginWIthFaceRecognition ss = new LoginWIthFaceRecognition(txtUsername.Text);
            ss.Show();
            this.Close();
            return true;
        }

        private void register()
        {
            GC.Collect();
            UserRegistration ss = new UserRegistration();
            ss.Show();
            this.Close();
        }

        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            register();
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
            speakerLogin();
        }

        private bool speakerLogin()
        {
            if (String.Equals(txtUsername.Text, "Username") || String.Equals(txtUsername.Text.Trim(), ""))
            {
                MessageBox.Show("Please enter your username!", "Error", MessageBoxButton.OK);
                return false; 
            }
            GC.Collect();
            LoginWithSpeakerRecognition ss = new LoginWithSpeakerRecognition(txtUsername.Text);
            ss.Show();
            this.Close();
            return true;
        }




        private void CreateMicrophoneRecoClient()
        {
            this.micClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(
                SpeechRecognitionMode.ShortPhrase,
                "en-US", _speechsubscriptionKey);
            this.micClient.AuthenticationUri = "";
            
            // Event handlers for speech recognition results
            this.micClient.OnMicrophoneStatus += this.OnMicrophoneStatus;
            this.micClient.OnPartialResponseReceived += this.OnPartialResponseReceivedHandler;

            this.micClient.OnResponseReceived += this.OnMicShortPhraseResponseReceivedHandler;

            this.micClient.OnConversationError += this.OnConversationErrorHandler;
        }

        private void OnPartialResponseReceivedHandler(object sender, PartialSpeechResponseEventArgs e)
        {
            //this.PartialWriteLine("{0}", e.PartialResult);
            this.PartialWriteLine(".", e.PartialResult);
        }

        private void PartialWriteLine(string format, params object[] args)
        {
            var formattedStr = string.Format(format, args);
            Dispatcher.Invoke(() =>
            {
                lblSpeech.Content += (formattedStr);
                
            });
        }

        private void OnConversationErrorHandler(object sender, SpeechErrorEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                //_startButton.IsEnabled = true;
                //_radioGroup.IsEnabled = true;
            });

            MessageBox.Show("Error received by OnConversationErrorHandler(): " + e.SpeechErrorText, "Error", MessageBoxButton.OK);

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
                lblSpeech.Content = "";

                this.WriteLine("--- OnMicShortPhraseResponseReceivedHandler ---");

                // we got the final result, so it we can end the mic reco.  No need to do this
                // for dataReco, since we already called endAudio() on it as soon as we were done
                // sending all the data.

                this.micClient.EndMicAndRecognition();

                this.HandleResponseResult(e);

                if (!resphonsePhraseFound)
                {
                    CreateMicrophoneRecoClient();
                    this.micClient.StartMicAndRecognition();
                }

            }));
        }


        /// <summary>
        /// Writes the response result.
        /// </summary>
        /// <param name="e">The <see cref="SpeechResponseEventArgs"/> instance containing the event data.</param>
        private void HandleResponseResult(SpeechResponseEventArgs e)
        {
            if (e.PhraseResponse.Results.Length == 0)
            {
                this.WriteLine("No phrase response is available.");
            }
            else
            {
                this.WriteLine("********* Final n-BEST Results *********");
                for (int i = 0; i < e.PhraseResponse.Results.Length; i++)
                {
                    this.WriteLine(
                        "[{0}] Confidence={1}, Text=\"{2}\"",
                        i,
                        e.PhraseResponse.Results[i].Confidence,
                        e.PhraseResponse.Results[i].DisplayText);
                }

                //Look for high confidence first
                for (int i = 0; i < e.PhraseResponse.Results.Length; i++)
                {

                    if (String.Equals(e.PhraseResponse.Results[i].Confidence.ToString(), "High"))
                    {
                        string temp = e.PhraseResponse.Results[i].DisplayText.ToLower();
                        if (temp.Contains("face"))
                        {
                            if (faceLogin())
                            {
                                resphonsePhraseFound = true;
                            }
                            else
                            {
                                txtUsername.Focus();
                            }
                            goto ends;
                        }

                        if (temp.Contains("speak") || temp.Contains("speech") || temp.Contains("peak"))
                        {
                            if (speakerLogin())
                            {
                                resphonsePhraseFound = true;
                            }
                            else
                            {
                                txtUsername.Focus();
                            }
                            goto ends;
                        }

                        if (temp.Contains("sign up") || temp.Contains("register") || temp.Contains("registration"))
                        {
                            register();
                        }
                    }
                }

                //Look for medium recog
                for (int i = 0; i < e.PhraseResponse.Results.Length; i++)
                {

                    if (String.Equals(e.PhraseResponse.Results[i].Confidence.ToString(), "Medium"))
                    {
                        string temp = e.PhraseResponse.Results[i].DisplayText.ToLower();
                        if (temp.Contains("face"))
                        {
                            if (faceLogin())
                            {
                                resphonsePhraseFound = true;
                            }
                            else
                            {
                                txtUsername.Focus();
                            }
                            goto ends;
                        }

                        if (temp.Contains("speak") || temp.Contains("speech") || temp.Contains("voice") || temp.Contains("peak"))
                        {
                            if (speakerLogin())
                            {
                                resphonsePhraseFound = true;
                            }
                            else
                            {
                                txtUsername.Focus();
                            }
                            goto ends;
                        }

                        if (temp.Contains("sign up") || temp.Contains("reg") || temp.Contains("register") || temp.Contains("registration"))
                        {
                            register();
                        }
                    }
                }
                ends:
                return;
            }
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
                else
                {
                    //this.micClient.EndMicAndRecognition();
                    //CreateMicrophoneRecoClient();
                    //this.micClient.StartMicAndRecognition();

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
                //_logText.Text += (formattedStr + "\n");
                //_logText.ScrollToEnd();
            });
        }
    }
}
