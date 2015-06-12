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

        private string _SRS;
        private double _X;
        private double _XMax;
        private double _Y;
        private double _YMax;
        private Image _Image;
        private string _ImageFormat;
        private int _Width;
        private int _Height;
        private double _Resolution;

        public double Resolution
        {
            get { return _Resolution; }
            set { _Resolution = value; }
        }

        public int Width
        {
            get { return _Width; }
            set { _Width = value; }
        }

        public int Height
        {
            get { return _Height; }
            set { _Height = value; }
        }

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

        public string ImageFormat
        {
            get { return _ImageFormat; }
            set { _ImageFormat = value; }
        }

        public Image Image
        {
            get { return _Image; }
            set { _Image = value; }
        }

        public string SRS
        {
            get { return _SRS; }
            set { _SRS = value; }
        }

        public double X
        {
            get { return _X; }
            set { _X = value; }
        }

        public double Y
        {
            get { return _Y; }
            set { _Y = value; }
        }

        public double XMax
        {
            get { return _XMax; }
            set { _XMax = value; }
        }

        public double YMax
        {
            get { return _YMax; }
            set { _YMax = value; }
        }

        #endregion
    }
}