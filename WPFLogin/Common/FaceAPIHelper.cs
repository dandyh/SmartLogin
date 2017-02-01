using Microsoft.ProjectOxford.Face;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Dandy Handoko
namespace WPFLogin.Common
{
    class FaceAPIHelper
    {
        public ObservableCollection<Face> faceResultCollection { get; set; }
        public FaceServiceClient faceServiceClient = new FaceServiceClient(ConfigurationSettings.AppSettings.Get("faceapikey"));

        public FaceAPIHelper()
        {
            faceResultCollection = new ObservableCollection<Face>();
        }

        public async Task<string> UploadOneFace(String filename)
        {
            var imageInfo = UIHelper.GetImageInfoForRendering(filename);
            // Call detection REST API, detect faces inside the image
            using (var fileStream = File.OpenRead(filename))
            {
                try
                {
                    
                    
                    var faces = await faceServiceClient.DetectAsync(fileStream);
                    
                    // Handle REST API calling error
                    if (faces.Count() == 0)
                    {
                        return "No face detected";
                    }
                    if (faces.Count() > 1)
                    {
                        return "More than 1 faces detected!";
                    }

                        // Convert detection results into UI binding object for rendering
                        foreach (var face in UIHelper.CalculateFaceRectangleForRendering(faces, 32, imageInfo))
                    {
                        // Detected faces are hosted in result container, will be used in the verification later
                        faceResultCollection.Add(face);
                    }

                    return String.Empty;
                }
                catch (FaceAPIException ex)
                {
                    return "Error found! " + ex.Message;

                }
            }
        }

        public async Task<string> Verify2Faces(ObservableCollection<Face> faceCollection1, ObservableCollection<Face> faceCollection2)
        {
            //Verify
            var res = await faceServiceClient.VerifyAsync(Guid.Parse(faceCollection1[0].FaceId), Guid.Parse(faceCollection2[0].FaceId));
            return string.Format("Confidence = {0:0.00}, {1}", res.Confidence, 
                res.IsIdentical ? "Login successful" : "Face is not recognized");
        }
    }
}
