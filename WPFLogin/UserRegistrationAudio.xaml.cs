using Microsoft.ProjectOxford.SpeakerRecognition;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Verification;
using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for UserRegistrationAudio.xaml
    /// </summary>
    public partial class UserRegistrationAudio : Window
    {
        private string _subscriptionKey = ConfigurationSettings.AppSettings.Get("speakerapikey");
        private Guid _speakerId = Guid.Empty;
        private User user;
        private string username = "";
        private WaveIn _waveIn;
        private WaveFileWriter _fileWriter;
        private Stream _stream;
        private SpeakerVerificationServiceClient _serviceClient;
        UserTableDataContext dataContext;
        public UserRegistrationAudio(string username)
        {
            InitializeComponent();
            try
            {
                InitializeComponent();

                this.username = username;
                dataContext = new UserTableDataContext();
                user = dataContext.Users.SingleOrDefault(x => x.username == this.username);

                _serviceClient = new SpeakerVerificationServiceClient(_subscriptionKey);
                initializeRecorder();
                initializeSpeaker();

            }
            catch (Exception gexp)
            {
                MessageBox.Show("Error: " + gexp.Message, "Error", MessageBoxButton.OK);                
                btnRecord.IsEnabled = false;
                btnReset.IsEnabled = false;
                
            }
        }

        private async void initializeSpeaker()
        {
            btnRecord.IsEnabled = false;
            bool created = await createProfile();           
            if (created)
            {
                refreshPhrases();
            }           
            btnReset.IsEnabled = false;
        }

        /// <summary>
        /// Initialize NAudio recorder instance
        /// </summary>
        private void initializeRecorder()
        {
            _waveIn = new WaveIn();
            _waveIn.DeviceNumber = 0;
            int sampleRate = 16000; // 16 kHz
            int channels = 1; // mono
            _waveIn.WaveFormat = new WaveFormat(sampleRate, channels);
            _waveIn.DataAvailable += waveIn_DataAvailable;
            _waveIn.RecordingStopped += waveSource_RecordingStopped;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (WaveIn.DeviceCount == 0)
                {
                    throw new Exception("Cannot detect microphone.");
                }
                lblStatus.Content = "Recording...";
                _waveIn.StartRecording();
                //btnRecord.IsEnabled = false;
               // btnStop.IsEnabled = true;
                
            }
            catch (Exception ge)
            {
                lblStatus.Content = "Error: " + ge.Message;
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            
            //btnStop.IsEnabled = false;
            _waveIn.StopRecording();
            lblStatus.Content = "Enrolling...";
        }

        /// <summary>
        /// A listener called when the recording stops
        /// </summary>
        /// <param name="sender">Sender object responsible for event</param>
        /// <param name="e">A set of arguments sent to the listener</param>
        private void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            _fileWriter.Dispose();
            _fileWriter = null;
            _stream.Seek(0, SeekOrigin.Begin);
            //Dispose recorder object
            _waveIn.Dispose();
            initializeRecorder();
            enrollSpeaker(_stream);
        }

        /// <summary>
        /// A method that's called whenever there's a chunk of audio is recorded
        /// </summary>
        /// <param name="sender">The sender object responsible for the event</param>
        /// <param name="e">The arguments of the event object</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (_fileWriter == null)
            {
                _stream = new IgnoreDisposeStream(new MemoryStream());
                _fileWriter = new WaveFileWriter(_stream, _waveIn.WaveFormat);
            }
            _fileWriter.Write(e.Buffer, 0, e.BytesRecorded);
        }


        /// <summary>
        /// Enrolls the audio of the speaker
        /// </summary>
        /// <param name="audioStream">The audio stream</param>
        private async void enrollSpeaker(Stream audioStream)
        {
            try
            {
                //Stopwatch sw = Stopwatch.StartNew();
                Enrollment response = await _serviceClient.EnrollAsync(audioStream, _speakerId);
                //sw.Stop();
                lblStatus.Content = "Enrollment Done"; //+ sw.Elapsed;
                txtRemainingEnrollment.Text = response.RemainingEnrollments.ToString();


                txtPhraseText.Text = response.Phrase;

                if (response.RemainingEnrollments == 0)
                {
                    MessageBox.Show("You have now completed the minimum number of enrollments. You may perform verification or add more enrollments", "Speaker enrolled");
                    user.speakerguid = _speakerId.ToString();
                    dataContext.SubmitChanges();

                }                
            }
            catch (EnrollmentException exception)
            {
                lblStatus.Content = "Cannot enroll speaker: " + exception.Message;                
            }
            catch (Exception gexp)
            {
                lblStatus.Content = "Error: " + gexp.Message;                
            }
        }

        /// <summary>
        /// Creates a speaker profile
        /// </summary>
        /// <returns>A boolean indicating the success/failure of the process</returns>
        private async Task<bool> createProfile()
        {
            lblStatus.Content  = "Creating Profile...";
            try
            {
                //Stopwatch sw = Stopwatch.StartNew();
                CreateProfileResponse response = await _serviceClient.CreateProfileAsync("en-us");
                //sw.Stop();
                //lblStatus.Content = "Profile Created, Elapsed Time: " + sw.Elapsed;                
                _speakerId = response.ProfileId;

                Profile profile = await _serviceClient.GetProfileAsync(_speakerId);

                txtRemainingEnrollment.Text = profile.RemainingEnrollmentsCount.ToString();
                lblStatus.Content = "Ready for recording.";
                return true;
            }
            catch (CreateProfileException exception)
            {
                MessageBox.Show("Cannot create profile:", "Error", MessageBoxButton.OK);                
                return false;
            }
            catch (Exception gexp)
            {
                MessageBox.Show("Error: " + gexp.Message, "Error", MessageBoxButton.OK);
                return false;
            }
        }

        private async void refreshPhrases()
        {            
            btnRecord.IsEnabled = false;
            try
            {
                VerificationPhrase[] phrases = await _serviceClient.GetPhrasesAsync("en-us");
                foreach (VerificationPhrase phrase in phrases)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = phrase.Phrase;
                    phrasesListbox.Items.Add(item);
                }
            }
            catch (PhrasesException exp)
            {
                MessageBox.Show("Cannot retrieve phrases: " + exp.Message, "Error", MessageBoxButton.OK);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message, "Error", MessageBoxButton.OK);
            }
            btnRecord.IsEnabled = true;
        }

        private async void btnReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lblStatus.Content = "Resetting profile: " + _speakerId;
                await _serviceClient.ResetEnrollmentsAsync(_speakerId);
                lblStatus.Content = "Profile reset";                
                btnReset.IsEnabled = false;

                Profile profile = await _serviceClient.GetProfileAsync(_speakerId);

                txtRemainingEnrollment.Text = profile.RemainingEnrollmentsCount.ToString();                      
            }
            catch (ResetEnrollmentsException exp)
            {
                lblStatus.Content = "Cannot reset Profile: " + exp.Message;
            }
            catch (Exception gexp)
            {
                lblStatus.Content = "Error: " + gexp.Message;
                
            }
        }
    }
}
