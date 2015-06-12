using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace USC.GISResearchLab.Common.Core.Images
{
    public class ImageCropper
    {

        // this is from http://blog.paranoidferret.com/?p=11
        public static Image CropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            return (Image)(bmpCrop);
        }

        // this is from: http://geekswithblogs.net/SanjayU/archive/2008/09/08/asp.net-image--photo-cropper-in-c.aspx
        public static byte[] CropImageFile(byte[] imageFile, int targetW, int targetH, int targetX, int targetY)
        {
            MemoryStream imgMemoryStream = new MemoryStream();
            Image imgPhoto = Image.FromStream(new MemoryStream(imageFile));

            Bitmap bmPhoto = new Bitmap(targetW, targetH, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(72, 72);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            //Adjust settings to make this as high-quality as possible  
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;

            try
            {
                grPhoto.DrawImage(imgPhoto, new Rectangle(0, 0, targetW, targetH),
                                    targetX, targetY, targetW, targetH, GraphicsUnit.Pixel);
                //Only JPG format for this demo
                bmPhoto.Save(imgMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                imgPhoto.Dispose();
                bmPhoto.Dispose();
                grPhoto.Dispose();
            }
            return imgMemoryStream.GetBuffer();
        }
    }
}
