using ImageScaler.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageScaler.Reader
{
    public class ImageReader
    {
        public string BasePath { get; }
        public string SaveDirectory { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public ImageReader(string basePath)
        {
            BasePath = basePath;
            if (!Directory.Exists(BasePath))
            {
                throw new Exception($"Directory({BasePath}) Not Found");
            }
        }

        public FileDataModel ReadImage(string filePath)
        {
            if (!File.Exists(filePath))
                throw new Exception("File Not Found");

            // Determine the content type based on the file extension
            string ImageName = Path.GetFileName(filePath);
            string contentType = GetContentType(ImageName);
            byte[] imageBytes;

            using (Image image = Image.FromFile(filePath))
            {
                if (Width > 0 && Height > 0)
                {
                    using (Bitmap newImage = new Bitmap(Width, Height))
                    {
                        using (Graphics graphics = Graphics.FromImage(newImage))
                        {
                            graphics.DrawImage(image, 0, 0, Width, Height);
                        }

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            newImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                            imageBytes = memoryStream.ToArray();
                        }
                    }
                }
                else
                {

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                        imageBytes = memoryStream.ToArray();
                    }
                }
            }
            return new FileDataModel(imageBytes, contentType);
        }
        
        private string GetContentType(string fileName)
        {
            // You can add more content types based on the supported image formats
            if (fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                return "image/jpeg";
            }
            else if (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                return "image/png";
            }
            else
            {
                // Default to application/octet-stream if the content type is unknown
                return "application/octet-stream";
            }
        }
    }
}
