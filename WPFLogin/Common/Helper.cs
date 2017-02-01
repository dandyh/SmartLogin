using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Configuration;
//Dandy Handoko
namespace WPFLogin
{
    //Design by Pongsakorn Poosankam
    class Helper
    {
        //Block Memory Leak
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr handle);
        public static BitmapSource bs;
        public static IntPtr ip;
        public static string tempPath = ConfigurationSettings.AppSettings.Get("tempphoto");
        public static BitmapSource LoadBitmap(System.Drawing.Bitmap source)
        {

            ip = source.GetHbitmap();

            bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, System.Windows.Int32Rect.Empty,

                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            DeleteObject(ip);

            return bs;

        }
        public static void SaveImageCapture(BitmapSource bitmap, string filename)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.QualityLevel = 100;

            if (File.Exists(filename))
                File.Delete(filename);

            using (FileStream fstream = new FileStream(filename, FileMode.Create))
            {
                encoder.Save(fstream);
                fstream.Close();
            }
            
            

        }

        public static string SaveTempImageCapture(BitmapSource bitmap)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.QualityLevel = 100;

            // Save Image
            Guid id = Guid.NewGuid();
            string filename = tempPath + id.ToString() + ".jpg";

            if (File.Exists(filename))
                File.Delete(filename);

            using (FileStream fstream = new FileStream(filename, FileMode.Create))
            {
                encoder.Save(fstream);
                fstream.Close();
            }

            return filename;
            
        }

    }
}
