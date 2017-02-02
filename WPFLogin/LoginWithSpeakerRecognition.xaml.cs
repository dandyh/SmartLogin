using Microsoft.ProjectOxford.SpeakerRecognition;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Verification;
using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WPFLogin
{
    /// <summary>
    /// Interaction logic for LoginWithSpeakerRecognition.xaml
    /// </summary>
    public partial class LoginWithSpeakerRecognition : Window
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

        public LoginWithSpeakerRecognition(string username)
        {
            try
            {
                InitializeComponent();
                dataContext = new UserTableDataContext();
                this.username = username;
                label1.Content = "Hello " + username;
                user = dataContext.Users.SingleOrDefault(x => x.username == this.username);
                if (user == null)
                {

                    //Delay 2 seconds to check whether user exists or not
                    var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
                    timer.Start();
                    timer.Tick += (sender, args) =>
                    {
                        timer.Stop();
                        MessageBox.Show("User not found", "Error", MessageBoxButton.OK);
                        MainWindow mw = new MainWindow();
                        mw.Show();
                        this.Close();
                        return;
                    };
                } else
                {
                    string _savedSpeakerId = user.speakerguid;
                    if (_savedSpeakerId != null)
                    {
                        _speakerId = new Guid(_savedSpeakerId);
                    }
                    initializeRecorder();
                    txtPhraseText.Text = user.speakerphrase;
                    _serviceClient = new SpeakerVerificationServiceClient(_subscriptionKey);
                }

                

            }
            catch (Exception gexp)
            {
                MessageBox.Show("Error: " + gexp.Message, "Error", MessageBoxButton.OK);
                btnRecord.IsEnabled = false;
            }
        }

        
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
            verifySpeaker(_stream);
        }

        /// <summary>
        /// A method that's called whenever there's a chunk of recorded audio is recorded
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

        private async void verifySpeaker(Stream audioStream)
        {
            try
            {
                lblStatus.Content = "Verifying..";                
                Verification response = await _serviceClient.VerifyAsync(audioStream, _speakerId);
                
                //statusResTxt.Text = response.Result.ToString();
                //confTxt.Text = response.Confidence.ToString();
                if (response.Result == Result.Accept)
                {
                    MessageBox.Show("Login successful, with confidence: " + response.Confidence.ToString(), "Successful", MessageBoxButton.OK);
                    lblStatus.Content = "Verification successful";
                }
                else
                {
                    MessageBox.Show("Login failed, with confidence: " + response.Confidence.ToString(), "Failed", MessageBoxButton.OK);
                    lblStatus.Content = "Verification failed";
                }
            }
            catch (VerificationException exception)
            {
                lblStatus.Content = "Cannot verify speaker: " + exception.Message;
            }
            catch (Exception e)
            {
                lblStatus.Content = "Error: " + e;
            }
        }

        private void btnRecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (WaveIn.DeviceCount == 0)
                {
                    throw new Exception("Cannot detect microphone.");
                }
                _waveIn.StartRecording();
                lblStatus.Content = "Recording...";
            }
            catch (Exception ge)
            {
                lblStatus.Content = "Error: " + ge.Message;
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            btnRecord.IsEnabled = true;
            _waveIn.StopRecording();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();            
            this.Close();
        }
    }
}
