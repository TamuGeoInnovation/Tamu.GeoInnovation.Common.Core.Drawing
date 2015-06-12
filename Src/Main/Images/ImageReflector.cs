using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace USC.GISResearchLab.Common.Core.Images
{
    public class ImageReflector
    {

        // this is from: http://www.codeproject.com/KB/GDI-plus/Image-Glass-Reflection.aspx
        public static Image DrawReflection(Image image, Color backgroundColor, int reflectivity)
        {
            // Calculate the size of the new image
            int height = (int)(image.Height + (image.Height * ((float)reflectivity / 255)));
            Bitmap newImage = new Bitmap(image.Width, height, PixelFormat.Format24bppRgb);
            newImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                // Initialize main graphics buffer
                graphics.Clear(backgroundColor);
                graphics.DrawImage(image, new Point(0, 0));
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                Rectangle destinationRectangle = new Rectangle(0, image.Size.Height,
                                                 image.Size.Width, image.Size.Height);

                // Prepare the reflected image
                int reflectionHeight = (image.Height * reflectivity) / 255;
                Image reflectedImage = new Bitmap(image.Width, reflectionHeight);

                // Draw just the reflection on a second graphics buffer
                using (Graphics gReflection = Graphics.FromImage(reflectedImage))
                {
                    gReflection.DrawImage(image,
                       new Rectangle(0, 0, reflectedImage.Width, reflectedImage.Height),
                       0, image.Height - reflectedImage.Height, reflectedImage.Width,
                       reflectedImage.Height, GraphicsUnit.Pixel);
                }
                reflectedImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle imageRectangle =
                    new Rectangle(destinationRectangle.X, destinationRectangle.Y,
                    destinationRectangle.Width,
                    (destinationRectangle.Height * reflectivity) / 255);

                // Draw the image on the original graphics
                graphics.DrawImage(reflectedImage, imageRectangle);

                // Finish the reflection using a gradiend brush
                LinearGradientBrush brush = new LinearGradientBrush(imageRectangle,
                       Color.FromArgb(255 - reflectivity, backgroundColor),
                        backgroundColor, 90, false);
                graphics.FillRectangle(brush, imageRectangle);
            }

            return newImage;
        }
    }
}
