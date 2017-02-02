using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using WebCam_Capture;
using System.Globalization;
using System.IO;
using System.Windows.Interop;
using WPFLogin;
using Microsoft.ProjectOxford.Face;
using System.Collections.ObjectModel;
using System.Configuration;
using WPFLogin.Common;
using System.Data;

namespace WPFLogin
{
    /// <summary>
    /// Interaction logic for LoginWIthFaceRecognition.xaml
    /// </summary>
    public partial class LoginWIthFaceRecognition : Window
    {
        WebCam webcam;
        string tempfilename;
        public string username { get; set; }

        public void redo()
        {
            webcam.Start();
            image1.Visibility = Visibility.Visible;
            image2.Visibility = Visibility.Hidden;
        }
        public LoginWIthFaceRecognition(string username)
        {
            InitializeComponent();
            this.username = username;
            label1.Content = "Hello " + username;
            // TODO: Add event handler implementation here.
            webcam = new WebCam();
            webcam.InitializeWebCam(ref image1);
            webcam.Start();
        }

        private async void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            image2.Visibility = Visibility.Visible;
            image2.Source = image1.Source;
            image1.Visibility = Visibility.Hidden;

            tempfilename = Helper.SaveTempImageCapture((BitmapSource)image2.Source);
            webcam.Stop();

            //Login
            UserTableDataContext dataContext = new UserTableDataContext();
            var usr = dataContext.Users.SingleOrDefault(x => x.username == this.username);

            ////label.Content = "Please wait...";
            if (usr != null)
            {
                FaceAPIHelper tempImage = new FaceAPIHelper();
                string result = await tempImage.UploadOneFace(tempfilename);
                if (String.IsNullOrEmpty(result))
                {
                    tempFaceCollection = tempImage.faceResultCollection;
                }
                else
                {
                    MessageBox.Show(result, "Error", MessageBoxButton.OK);
                    redo();
                    return;
                }


                //Get comparison face for verify
                //CHANGE HERE TO BE DYNAMIC
                string imagepath = usr.photofile; // dt.Rows[0][0].ToString();
                FaceAPIHelper realImage = new FaceAPIHelper();
                result = await realImage.UploadOneFace(imagepath);
                if (String.IsNullOrEmpty(result))
                {
                    databaseFaceCollection = realImage.faceResultCollection;
                }
                else
                {
                    MessageBox.Show(result, "Error", MessageBoxButton.OK);
                    redo();
                    return;
                }

                //Verify                
                string res = await realImage.Verify2Faces(tempFaceCollection, databaseFaceCollection);
                if (res.ToLower().Contains("successful"))
                {
                    MessageBox.Show(res, "Successful", MessageBoxButton.OK);
                    webcam.Stop();
                }
                else
                {
                    MessageBox.Show(res, "Error", MessageBoxButton.OK);
                    redo();
                    return;
                }
                
            }
            else
            {
                MessageBox.Show("User not found", "Error", MessageBoxButton.OK);
                MainWindow mw = new MainWindow();
                mw.Show();
                webcam.Stop();
                this.Close();
            }

        }

        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            redo();
        }

        private ObservableCollection<Face> tempFaceCollection = new ObservableCollection<Face>();
        private ObservableCollection<Face> databaseFaceCollection = new ObservableCollection<Face>();

        //private async void btnLogin_Click(object sender, RoutedEventArgs e)
        //{

        //    UserTableDataContext dataContext = new UserTableDataContext();
        //    var usr = dataContext.Users.SingleOrDefault(x => x.username == this.username );
                        
        //    ////label.Content = "Please wait...";
        //    if(usr != null)
        //    {
        //        FaceAPIHelper tempImage = new FaceAPIHelper();
        //        string result = await tempImage.UploadOneFace(tempfilename);
        //        if (String.IsNullOrEmpty(result))
        //        {
        //            tempFaceCollection = tempImage.faceResultCollection;
        //        }
        //        else
        //        {
        //            MessageBox.Show(result, "Error", MessageBoxButton.OK);
        //            redo();
        //            return;
        //        }


        //        //Get comparison face for verify
        //        //CHANGE HERE TO BE DYNAMIC
        //        string imagepath = usr.photofile; // dt.Rows[0][0].ToString();
        //        FaceAPIHelper realImage = new FaceAPIHelper();
        //        result = await realImage.UploadOneFace(imagepath);
        //        if (String.IsNullOrEmpty(result))
        //        {
        //            databaseFaceCollection = realImage.faceResultCollection;
        //        }
        //        else
        //        {
        //            MessageBox.Show(result, "Error", MessageBoxButton.OK);
        //            return;
        //        }

        //        //Verify
        //        FaceAPIHelper verification = new FaceAPIHelper();
        //        string res = await verification.Verify2Faces(tempFaceCollection, databaseFaceCollection);
        //        MessageBox.Show(res, "Successful", MessageBoxButton.OK);
        //        webcam.Stop();
        //    }
        //    else
        //    {
        //        MessageBox.Show("User not found", "Error", MessageBoxButton.OK);                
        //        MainWindow mw = new MainWindow();
        //        mw.Show();
        //        webcam.Stop();
        //        this.Close();
        //    }
            
                        
        //}

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            webcam.Stop();
            this.Close();
        }
    }
}
