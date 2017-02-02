using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
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
    /// Interaction logic for UserRegistration.xaml
    /// </summary>
    public partial class UserRegistration : Window
    {
        WebCam webcam;
        UserTableDataContext db = new UserTableDataContext();
        User regUser = new User();
        string tempfilename;
        public UserRegistration()
        {
            InitializeComponent();
            webcam = new WebCam();
            webcam.InitializeWebCam(ref image1);
            webcam.Start();
        }

        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            if(db.Users.SingleOrDefault(x => x.username == txtUsername.Text) != null){
                MessageBox.Show("Username is already exist!", "Error", MessageBoxButton.OK);
                return;
            }
            regUser.username = txtUsername.Text;
            regUser.name = txtName.Text;
            regUser.password = txtPassword.Password;
            regUser.photofile = ConfigurationSettings.AppSettings.Get("photoslocation") + txtUsername.Text + ".jpg";
            Helper.SaveImageCapture((BitmapSource)image1.Source, regUser.photofile);

            db.Users.InsertOnSubmit(regUser);
            db.SubmitChanges();
            MessageBox.Show("Registration successful, please record your voice for speaker recognition login!", "Successful", MessageBoxButton.OK);
            UserRegistrationAudio mw = new UserRegistrationAudio(txtUsername.Text);
            mw.Show();
            this.Close();

        }

        public void txtPass_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password == "xxxxxxx")
            {
                txtPassword.Password = "";
            }

        }


        public void txtUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text == "Username")
            {
                txtUsername.Text = "";
            }

        }

        public void txtName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtName.Text == "Name")
            {
                txtName.Text = "";
            }

        }

        private ObservableCollection<Face> detectedFaces = new ObservableCollection<Face>();
        private async void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            tempfilename = Helper.SaveTempImageCapture((BitmapSource)image1.Source);
            webcam.Stop();
            var imageInfo = UIHelper.GetImageInfoForRendering(tempfilename);
            using (var fileStream = File.OpenRead(tempfilename))
            {
                try
                {
                    string subscriptionKey = ConfigurationSettings.AppSettings.Get("faceapikey");
                    IFaceServiceClient faceServiceClient = new FaceServiceClient(subscriptionKey);
                    var faces = await faceServiceClient.DetectAsync(fileStream, true, true, new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Glasses, FaceAttributeType.HeadPose, FaceAttributeType.FacialHair });

                    //Render rectangle in the image
                    Uri fileUri = new Uri(tempfilename);
                    BitmapImage bitmapSource = new BitmapImage();
                    bitmapSource.BeginInit();
                    bitmapSource.CacheOption = BitmapCacheOption.None;
                    bitmapSource.UriSource = fileUri;
                    bitmapSource.EndInit();
                    //image1.Source = bitmapSource;
                    Title = "Detecting Face...";
                    FaceRectangle[] faceRects = faces.Select(face => face.FaceRectangle).ToArray();
                    Title = String.Format("Detection Finished. {0} Face(s) Detected", faceRects.Length);
                    //If face detected
                    if (faceRects.Length == 1)
                    {
                        //DetectedResultsInText = string.Format("{0} face(s) has been detected", faces.Length);

                        foreach (var face in faces)
                        {
                            detectedFaces.Add(new Face()
                            {
                                ImagePath = tempfilename,
                                Left = face.FaceRectangle.Left,
                                Top = face.FaceRectangle.Top,
                                Width = face.FaceRectangle.Width,
                                Height = face.FaceRectangle.Height,
                                FaceId = face.FaceId.ToString(),
                                Gender = face.FaceAttributes.Gender,
                                Age = string.Format("{0:#} years old", face.FaceAttributes.Age),
                                IsSmiling = face.FaceAttributes.Smile > 0.0 ? "Smile" : "Not Smile",
                                Glasses = face.FaceAttributes.Glasses.ToString(),
                                FacialHair = string.Format("Facial Hair: {0}", face.FaceAttributes.FacialHair.Moustache + face.FaceAttributes.FacialHair.Beard + face.FaceAttributes.FacialHair.Sideburns > 0 ? "Yes" : "No"),
                                HeadPose = string.Format("Pitch: {0}, Roll: {1}, Yaw: {2}", Math.Round(face.FaceAttributes.HeadPose.Pitch, 2), Math.Round(face.FaceAttributes.HeadPose.Roll, 2), Math.Round(face.FaceAttributes.HeadPose.Yaw, 2))
                            });
                            lblGender.Content = "Gender: " + face.FaceAttributes.Gender;
                            regUser.gender = face.FaceAttributes.Gender;

                            lblAge.Content = string.Format("{0:#} years old", face.FaceAttributes.Age);
                            regUser.age = string.Format("{0:#}", face.FaceAttributes.Age);
                            if (!String.Equals(face.FaceAttributes.Glasses.ToString(),"NoGlasses"))
                            {
                                MessageBox.Show("Please dont wear your glasses! Thanks", "Error", MessageBoxButton.OK);
                                webcam.Start();
                                return;
                            }

                            
                        }


                        DrawingVisual visual = new DrawingVisual();
                        DrawingContext drawingContext = visual.RenderOpen();
                        drawingContext.DrawImage(bitmapSource,
                        new Rect(0, 0, bitmapSource.Width, bitmapSource.Height));
                        double dpi = bitmapSource.DpiX;
                        double resizeFactor = 96 / dpi;
                        foreach (var faceRect in faceRects)
                        {

                            drawingContext.DrawRectangle(
                            Brushes.Transparent,
                            new Pen(Brushes.LightBlue, 2),
                            new Rect(
                            faceRect.Left * resizeFactor,
                            faceRect.Top * resizeFactor,
                            faceRect.Width * resizeFactor,
                            faceRect.Height * resizeFactor
                            )
                            );
                        }
                        drawingContext.Close();
                        RenderTargetBitmap faceWithRectBitmap = new RenderTargetBitmap(
                        (int)(bitmapSource.PixelWidth * resizeFactor),
                        (int)(bitmapSource.PixelHeight * resizeFactor),
                        96,
                        96,
                        PixelFormats.Pbgra32);
                        faceWithRectBitmap.Render(visual);
                        image1.Source = faceWithRectBitmap;
                    }
                    else
                    {                        
                        if(faceRects.Length > 1)
                            MessageBox.Show("More than 1 photo detected, please capture another photo!", "Error", MessageBoxButton.OK);
                        else
                            MessageBox.Show("Face is not detected, please capture another photo!", "Error", MessageBoxButton.OK);
                        webcam.Start();
                    }
                }
                catch (FaceAPIException ex)
                {
                    GC.Collect();
                    return;
                }





                GC.Collect();
            }
        }

        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            lblAge.Content = "";
            lblGender.Content = "";
            webcam.Start();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            webcam.Stop();
            this.Close();
        }
    }
}
