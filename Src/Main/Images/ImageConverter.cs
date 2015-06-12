using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace USC.GISResearchLab.Common.Core.Images
{
    public class ImageConverter
    {

        public static byte[] ConvertImageToByteArray(Image imageToConvert)
        {
            byte[] imgByteArray;
            try
            {


                using (MemoryStream imgMemoryStream = new MemoryStream())
                {
                    //Only JPG format for this demo
                    imageToConvert.Save(imgMemoryStream, ImageFormat.Jpeg);
                    imgByteArray = imgMemoryStream.ToArray();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return imgByteArray;
        }
    }
}
