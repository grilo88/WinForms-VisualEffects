using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace VisualEffects.Extensions
{
    public static class ImageExtensions
    {
        public static bool IsBlank( this Image img, int threshold, int checkOnePixelEveryN = 1 )
        {
            if( checkOnePixelEveryN <= 0 )
                throw new ArgumentException( "Must check at least one pixel every 1 pixel (every pixel)" );

            return img.GetStandardDeviation() < threshold;
        }

        /// <summary>
        /// Get the standard deviation of pixel values.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="checkOnePixelEveryN"></param>
        /// <returns>Standard deviation.</returns>
        private static double GetStandardDeviation( this Image img, int checkOnePixelEveryN = 1 )
        {
            if( checkOnePixelEveryN <= 0 )
                throw new ArgumentException( "Must check at least one pixel every 1 pixel (every pixel)" );

            double total = 0, totalVariance = 0;
            var count = 1;
            double stdDev = 0;

            // First get all the bytes
            using( var bmp = new Bitmap( img ) )
            {
                var bmData = bmp.LockBits( new Rectangle( 0, 0, bmp.Width, bmp.Height ), ImageLockMode.ReadOnly, bmp.PixelFormat );
                var stride = bmData.Stride;
                var scan0 = bmData.Scan0;
                unsafe
                {
                    var pointer = (byte*)scan0;
                    var nOffset = stride - bmp.Width * 3;

                    for( var y = 0; y < bmp.Height; pointer += nOffset, y += checkOnePixelEveryN )
                    {
                        for( var x = 0; x < bmp.Width; count++, pointer += 3, x += checkOnePixelEveryN )
                        {
                            var blue = pointer[ 0 ];
                            var green = pointer[ 1 ];
                            var red = pointer[ 2 ];

                            var pixelValue = Color.FromArgb( 0, red, green, blue ).ToArgb();
                            total += pixelValue;

                            var avg = total / count;
                            totalVariance += Math.Pow( pixelValue - avg, 2 );
                            stdDev = Math.Sqrt( totalVariance / count );
                        }
                    }
                }

                bmp.UnlockBits( bmData );
            }

            return stdDev;
        }

        /// <summary>
        /// Resize an image by a percentage factor
        /// </summary>
        /// <param name="image">Source image</param>
        /// <param name="percent"></param>
        /// <returns>Scaled image</returns>
        public static Image ScaleByPercentage( this Image image, int percent )
        {
            var nPercent = (float)percent / 100;

            var destWidth = (int)( image.Width - ( image.Width * nPercent ) );
            var destHeight = (int)( image.Height - ( image.Height * nPercent ) );

            var bmp = new Bitmap( destWidth, destHeight, PixelFormat.Format24bppRgb );
            bmp.SetResolution( image.HorizontalResolution, image.VerticalResolution );

            using( var graphics = Graphics.FromImage( bmp ) )
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                var destRect = new Rectangle( 0, 0, destWidth, destHeight );
                var srcRect = new Rectangle( 0, 0, image.Width, image.Height );

                graphics.DrawImage( image, destRect, srcRect, GraphicsUnit.Pixel );
            }

            return bmp;
        }

        public static Image Resize( this Image image, Size newSize )
        {
            var bmp = new Bitmap( newSize.Width, newSize.Height );
            using( var gx = Graphics.FromImage( bmp ) )
            {
                gx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gx.DrawImage( image, 0, 0, newSize.Width, newSize.Height );
            }

            return bmp;
        }

        public static Image ToGrayscale( this Image source )
        {
            var bmp = new Bitmap( source.Width, source.Height );

            using( var graphics = Graphics.FromImage( bmp ) )
            {
                var colorMatrix = new ColorMatrix
                (
                    new[]
                    { 
                        new[] { 0.5f, 0.5f, 0.5f, 0, 0 }, 
                        new[] { 0.5f, 0.5f, 0.5f, 0, 0 }, 
                        new[] { 0.5f, 0.5f, 0.5f, 0, 0 }, 
                        new[] { 0, 0, 0, 1.0f, 0, 0 }, 
                        new[] { 0, 0, 0, 0, 1.0f, 0 }, 
                        new[] { 0, 0, 0, 0, 0, 1.0f } 
                    }
                );

                var imageAttrs = new ImageAttributes();
                imageAttrs.SetColorMatrix( colorMatrix, ColorMatrixFlag.SkipGrays );

                var destRect = new Rectangle( 0, 0, source.Width, source.Height );
                graphics.DrawImage( source, destRect, 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, imageAttrs );
            }

            return bmp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="opacity">A value between 0 (full transparency) and 100 (full opacity)</param>
        /// <returns></returns>
        public static Image ChangeOpacity( this Image image, int opacity )
        {
            opacity = opacity < 0 ? 0 : opacity;
            opacity = opacity > 100 ? 100 : opacity;

            //create a Bitmap the size of the image provided  
            var bmp = new Bitmap( image.Width, image.Height );

            //create a graphics object from the image  
            using( var gfx = Graphics.FromImage( bmp ) )
            {
                //create a color matrix object  
                var matrix = new ColorMatrix
                {//set the opacity  
                    Matrix33 = (float) opacity / 100
                };
                
                //create image attributes  
                var attributes = new ImageAttributes();

                //set the color (opacity) of the image  
                attributes.SetColorMatrix( matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap );

                //now draw the image  
                gfx.DrawImage( image, new Rectangle( 0, 0, bmp.Width, bmp.Height ), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes );
            }

            return bmp;
        }

        public static Image ToBlackWhite( this Image image, float threshold = 0.7f )
        {
            var outImage = new Bitmap( image.Width, image.Height, PixelFormat.Format32bppArgb );
            using( var gr = Graphics.FromImage( outImage ) )
            {
                var grayMatrix = new ColorMatrix( new[]
                { 
                    new[] { 0.299f, 0.299f, 0.299f, 0, 0 }, 
                    new[] { 0.587f, 0.587f, 0.587f, 0, 0 }, 
                    new[] { 0.114f, 0.114f, 0.114f, 0, 0 }, 
                    new float[] { 0,      0,      0,      1, 0 }, 
                    new float[] { 0,      0,      0,      0, 1 } 
                } );

                var ia = new ImageAttributes();
                ia.SetColorMatrix( grayMatrix );
                ia.SetThreshold( threshold );

                var rc = new Rectangle( 0, 0, image.Width, image.Height );
                gr.DrawImage( image, rc, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, ia );

                return outImage;
            }
        }

        public static MemoryStream ToStream( this Image image, ImageFormat format )
        {
            var stream = new MemoryStream();
            image.Save( stream, format );
            stream.Position = 0;

            return stream;
        }

        public static bool Equals( this Bitmap bmp1, Bitmap bmp2 )
        {
            var rect = new Rectangle( 0, 0, bmp1.Width, bmp1.Height );
            var bmpData1 = bmp1.LockBits( rect, ImageLockMode.ReadOnly, bmp1.PixelFormat );
            var bmpData2 = bmp2.LockBits( rect, ImageLockMode.ReadOnly, bmp2.PixelFormat );

            try
            {
                unsafe
                {
                    var ptr1 = (byte*)bmpData1.Scan0.ToPointer();
                    var ptr2 = (byte*)bmpData2.Scan0.ToPointer();
                    var width = rect.Width * 3; // for 24bpp pixel data

                    for( var y = 0; y < rect.Height; y++ )
                    {
                        for( var x = 0; x < width; x++ )
                        {
                            if( *ptr1 != *ptr2 )
                                return false;

                            ptr1++;
                            ptr2++;
                        }
                        ptr1 += bmpData1.Stride - width;
                        ptr2 += bmpData2.Stride - width;
                    }
                }
            }
            finally
            {
                bmp1.UnlockBits( bmpData1 );
                bmp2.UnlockBits( bmpData2 );
            }

            return true;
        }
    }
}
