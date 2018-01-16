using System.Linq;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Lightweight.Business.Helpers
{
    public class ImageHelper
    {
        /// <summary>
        /// Resizes an image (down) or returns the original if no resize is needed.
        /// </summary>
        /// <param name="contents">The image stream to resize.</param>
        /// <param name="size">The desired size of the image (highest value between height and width).</param>
        /// <param name="resizefactor">The factor to use for sizing down the image. Only used if size is 0.</param>
        /// <param name="quality">The quality of the resized image, between 1 and 100.</param>
        /// <returns></returns>
        public static Stream ResizeImage(Stream contents, int size, decimal resizefactor, long quality)
        {
            Stream output;

            if (resizefactor <= 0 || resizefactor > 100)
                resizefactor = 100;

            decimal coefficient = resizefactor / 100;

            if (size <= 0 && (coefficient > 1 || coefficient < 0)) // cannot scale up or to a negative value, return original
                return contents;

            using (System.Drawing.Image original = System.Drawing.Image.FromStream(contents))
            {
                //calculate new width and height based on size or coefficient
                int width, height;
                if (original.Height > original.Width)
                {
                    if (size <= 0)
                        size = (int)(original.Height * coefficient);

                    height = size;
                    width = (int)(original.Width * ((float)size / (float)original.Height));
                }
                else
                {
                    if (size <= 0)
                        size = (int)(original.Width * coefficient);

                    width = size;
                    height = (int)(original.Height * ((float)size / (float)original.Width));
                }

                using (System.Drawing.Bitmap resized = new System.Drawing.Bitmap(width, height, original.PixelFormat))
                {
                    resized.SetResolution(original.HorizontalResolution, original.VerticalResolution);
                    using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(resized))
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphics.CompositingQuality = CompositingQuality.HighQuality;

                        graphics.DrawImage(original, new System.Drawing.Rectangle(0, 0, width, height), 0, 0, original.Width, original.Height, System.Drawing.GraphicsUnit.Pixel);
                    }

                    #region codec info

                    //if (contentType == "image/gif")
                    //{
                    //    using (thumbnail)
                    //    {
                    //        OctreeQuantizer quantizer = new OctreeQuantizer(255, 8);
                    //        using (Bitmap quantized = quantizer.Quantize(bitmap))
                    //        {
                    //            Response.ContentType = "image/gif";
                    //            quantized.Save(Response.OutputStream, ImageFormat.Gif);
                    //        }
                    //    }
                    //}

                    //if (contentType == "image/jpeg")
                    //{
                    //    var info = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
                    //    EncoderParameters encoderParameters;
                    //    encoderParameters = new EncoderParameters(1);
                    //    encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                    //    Response.ContentType = "image/jpeg";
                    //    thumbnail.Save(Response.OutputStream, info[1], encoderParameters);
                    //}

                    #endregion

                    //encode the image
                    ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
                    Encoder myEncoder = Encoder.Quality;
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
                    myEncoderParameters.Param[0] = myEncoderParameter;

                    //save it to server

                    output = new MemoryStream();
                    resized.Save(output, jgpEncoder, myEncoderParameters);
                }
            }

            return output;
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }
    }
}