using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace USC.GISResearchLab.Common.Core.Images
{

    // this is from: http://danbystrom.se/2009/01/12/thumbnails-with-glass-table-reflection-in-gdi/
    public class ImageReflectorWithShear : IDisposable
    {
        public Bitmap ThumbnailImage;
        public int ImageBottom;

        public Size MaxSize;
        public int Skew;
        public int FrameWidth;
        public int MaxReflectionLength;

        public Pen BorderPen = Pens.Gray;
        public Brush FrameBrush = Brushes.White;

        private struct Pixel
        {
            public byte B;
            public byte G;
            public byte R;
            public byte A;
        }

        public ImageReflectorWithShear()
            : this(new Size(128, 128), 20, 3, 80)
        {
        }

        public ImageReflectorWithShear(Size maxSize, int skew, int frameWidth, int maxReflectionLength)
        {
            MaxSize = maxSize;
            Skew = skew;
            FrameWidth = frameWidth;
            MaxReflectionLength = maxReflectionLength;
        }

        public void Dispose()
        {
            DisposeBitmap();
        }

        public void DisposeBitmap()
        {
            if (ThumbnailImage != null)
            {
                ThumbnailImage.Dispose();
                ThumbnailImage = null;
            }
        }

        public byte[] Create(byte[] rawData)
        {
            Bitmap bitMap = (Bitmap)Bitmap.FromStream(new MemoryStream(rawData));
            return Create(bitMap);
        }

        public byte[] Create(Bitmap image)
        {
            MemoryStream imgMemoryStream = new MemoryStream();
            DisposeBitmap();

            Size sz = AdaptProportionalSize(new Size(MaxSize.Width - FrameWidth * 2, MaxSize.Height - FrameWidth * 2), image.Size);
            sz.Width += FrameWidth * 2;
            sz.Height += FrameWidth * 2;

            ImageBottom = sz.Height;

            int reflectionLength = Math.Min(sz.Height, MaxReflectionLength);
            ThumbnailImage = new Bitmap(sz.Width, sz.Height + reflectionLength + Skew);

            using (Bitmap
                bmpFramed = CreateFramedBitmap(image, sz),
                bmpReflection = CreateReflectedBitmap(bmpFramed, reflectionLength))

            using (Graphics g = Graphics.FromImage(ThumbnailImage))
            {
                // draw the reflected image to the resulting image
                // using a shear transform
                System.Drawing.Drawing2D.Matrix m = g.Transform;
                m.Shear(0, (float)Skew / sz.Width);
                m.Translate(0, sz.Height - Skew - 1);
                g.Transform = m;
                g.DrawImage(bmpReflection, Point.Empty);
                g.ResetTransform();

                // draw the real (framed) image to the resulting image
                // one column at a time, slightly altering the height
                // of the destination column in order to create the
                // "half sheared" transform
                for (int x = 0; x < sz.Width; x++)
                    g.DrawImage(
                        bmpFramed,
                        new RectangleF(x, 0, 1, sz.Height - Skew * (float)(sz.Width - x) / sz.Width),
                        new RectangleF(x, 0, 1, sz.Height),
                        GraphicsUnit.Pixel);

                ThumbnailImage.Save(imgMemoryStream, ImageFormat.Png);
            }

            return imgMemoryStream.GetBuffer();

        }

        protected virtual Bitmap CreateFramedBitmap(Bitmap bmpSource, Size szFull)
        {
            Bitmap bmp = new Bitmap(szFull.Width, szFull.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(FrameBrush, 0, 0, szFull.Width, szFull.Height);
                g.DrawRectangle(BorderPen, 0, 0, szFull.Width - 1, szFull.Height - 1);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                g.DrawImage(
                    bmpSource,
                    new Rectangle(FrameWidth, FrameWidth, szFull.Width - FrameWidth * 2, szFull.Height - FrameWidth * 2),
                    new Rectangle(Point.Empty, bmpSource.Size),
                    GraphicsUnit.Pixel);
            }
            return bmp;
        }

        private static unsafe byte GaussBlur(byte* p, int width)
        {
            unchecked
            {
                return (byte)((p[-width - 4] + 2 * p[-width] + p[-width + 4] +
                                    2 * p[-4] + 4 * p[0] + 2 * p[4] +
                                    p[width - 4] + 2 * p[width] + p[width + 4]) / 16);
            }
        }

        protected virtual double CalculateAlphaFallout(double f)
        {
            return f * f * 0.8;
        }

        protected virtual Bitmap CreateReflectedBitmap(Bitmap bmpFramed, int height)
        {
            Bitmap bmpResult = new Bitmap(bmpFramed.Width, height);

            BitmapData bdS = bmpFramed.LockBits(new Rectangle(Point.Empty, bmpFramed.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bdD = bmpResult.LockBits(new Rectangle(Point.Empty, bmpResult.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            unchecked
            {
                unsafe
                {
                    int nWidthInPixels = bdD.Width * 4;
                    for (int y = height - 1; y >= 0; y--)
                    {
                        byte alpha = (byte)(255 * CalculateAlphaFallout((double)(height - y) / height));
                        Pixel* pS = (Pixel*)bdS.Scan0.ToPointer() + bdS.Width * (bdS.Height - y - FrameWidth - 1);
                        Pixel* pD = (Pixel*)bdD.Scan0.ToPointer() + bdD.Width * y;
                        for (int x = bdD.Width; x > 0; x--, pD++, pS++)
                        {
                            int R = GaussBlur(&pS->R, nWidthInPixels);
                            int G = GaussBlur(&pS->G, nWidthInPixels);
                            int B = GaussBlur(&pS->B, nWidthInPixels);
                            pD->R = (byte)((R * 3 + G * 2 + B * 2) / 7);
                            pD->G = (byte)((R * 2 + G * 3 + B * 2) / 7);
                            pD->B = (byte)((R * 2 + G * 2 + B * 3) / 7);
                            pD->A = alpha;
                        }
                    }
                }
            }
            bmpFramed.UnlockBits(bdD);
            bmpResult.UnlockBits(bdD);

            return bmpResult;
        }

        public static Size AdaptProportionalSize(Size szMax, Size szReal)
        {
            int nWidth;
            int nHeight;
            double sMaxRatio;
            double sRealRatio;

            if (szMax.Width < 1 || szMax.Height < 1 || szReal.Width < 1 || szReal.Height < 1)
            {
                return Size.Empty;
            }

            sMaxRatio = (double)szMax.Width / (double)szMax.Height;
            sRealRatio = (double)szReal.Width / (double)szReal.Height;

            if (sMaxRatio < sRealRatio)
            {
                nWidth = Math.Min(szMax.Width, szReal.Width);
                nHeight = (int)Math.Round(nWidth / sRealRatio);
            }
            else
            {
                nHeight = Math.Min(szMax.Height, szReal.Height);
                nWidth = (int)Math.Round(nHeight * sRealRatio);
            }

            return new Size(nWidth, nHeight);
        }
    }
}
