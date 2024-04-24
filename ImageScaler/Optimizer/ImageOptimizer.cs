using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageScaler.Optimizer
{
    public static class ImageOptimizer
    {
        public static void CompressImage(string imagePath, string outputFilePath)
        {
            using (Image image = Image.FromFile(imagePath))
            {
                string extension = Path.GetExtension(imagePath);
                ImageFormat format = GetImageFormatFromExtension(extension);

                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 50L); // Adjust quality as needed

                ImageCodecInfo codecInfo = GetEncoder(format);
                if (codecInfo != null)
                {
                    image.Save(outputFilePath, codecInfo, encoderParameters);
                }
                else
                {
                    throw new ArgumentException("Encoder not found for the specified format.");
                }
            }
        }
        
        private static ImageFormat GetImageFormatFromExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                throw new ArgumentException("Extension cannot be null or empty.");

            switch (extension.ToLower())
            {
                case ".bmp":
                    return ImageFormat.Bmp;
                case ".jpg":
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case ".gif":
                    return ImageFormat.Gif;
                case ".png":
                    return ImageFormat.Png;
                case ".tiff":
                case ".tif":
                    return ImageFormat.Tiff;
                case ".wmf":
                    return ImageFormat.Wmf;
                default:
                    throw new ArgumentException("Unsupported image format.");
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }

    }
}
