using System;
using System.Drawing;

namespace USC.GISResearchLab.Common.GeoreferencedImages
{
    /// <summary>
    /// Summary description for ImageResult.
    /// </summary>
    public class GeoreferencedImage
    {
        #region Properties


        public double Resolution { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Size
        {
            get
            {
                int ret = 0;
                if (Height != Width)
                {
                    throw new Exception("Image Size property is only applicable to square images - height: " + Height + " width: " + Width);
                }
                else
                {
                    ret = Width;
                }
                return ret;
            }
            set
            {
                Height = value;
                Width = value;
            }
        }

        public string ImageFormat { get; set; }
        public Image Image { get; set; }
        public string SRS { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double XMax { get; set; }
        public double YMax { get; set; }

        #endregion
    }
}