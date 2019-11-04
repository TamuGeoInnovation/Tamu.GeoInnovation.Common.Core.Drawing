using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace USC.GISResearchLab.Common.Core.Images
{
    public class ImageResizer
    {
        // this is from http://forums.asp.net/t/29645.aspx
        public void ResizeFromStream(string ImageSavePath, int MaxSideSize, Stream Buffer)
        {
            int intNewWidth;
            int intNewHeight;
            Image imgInput = Image.FromStream(Buffer);

            //Determine image format
            ImageFormat fmtImageFormat = imgInput.RawFormat;

            //get image original width and height
            int intOldWidth = imgInput.Width;
            int intOldHeight = imgInput.Height;

            //determine if landscape or portrait
            int intMaxSide;

            if (intOldWidth >= intOldHeight)
            {
                intMaxSide = intOldWidth;
            }
            else
            {
                intMaxSide = intOldHeight;
            }


            if (intMaxSide > MaxSideSize)
            {
                //set new width and height
                double dblCoef = MaxSideSize / (double)intMaxSide;
                intNewWidth = Convert.ToInt32(dblCoef * intOldWidth);
                intNewHeight = Convert.ToInt32(dblCoef * intOldHeight);
            }
            else
            {
                intNewWidth = intOldWidth;
                intNewHeight = intOldHeight;
            }
            //create new bitmap
            Bitmap bmpResized = new Bitmap(imgInput, intNewWidth, intNewHeight);

            //save bitmap to disk
            bmpResized.Save(ImageSavePath, fmtImageFormat);

            //release used resources
            imgInput.Dispose();
            bmpResized.Dispose();
            Buffer.Close();
        }
    }
}
