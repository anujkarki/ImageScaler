using ImageScaler.Optimizer;
using System.Text;

namespace ImageScaler.Writer
{
    public class ImageWriter
    {
        public string[] PermittedExtensions { get; private set; } = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" };
        public string BasePath { get; } = string.Empty;
        public string SaveDirectory { get; set; } = string.Empty;
        public string TempPath { get; set; } = string.Empty;
        public string OutputFilePath { get; set; } = string.Empty;
        public string OutputFileName { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public bool SetNewFileName { get; set; } = false;
        public bool Compressed { get; set; } = false;
        public int FileNameLength { get; set; } = 50;

        public ImageWriter(string basePath)
        {
            BasePath = basePath;
            if (!Directory.Exists(BasePath))
            {
                throw new Exception($"Directory({BasePath}) Not Found");
            }
        }

        public void UpdatePermittedExtensions(string[] NewPermittedExtensions)
        {
            PermittedExtensions = NewPermittedExtensions;
        }

        public void SaveImage(Stream imageStream, string fileName)
        {
            OriginalFileName = fileName;
            if (imageStream == null)
                throw new Exception("Stream cannot be Null");
            // Check the file extension
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || Array.IndexOf(PermittedExtensions, extension) < 0)
            {
                throw new Exception("Invalid File Extension");
            }

            SaveImage(imageStream);
        }

        public void SaveImage(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                throw new Exception("File Not Found");
            }
            // Check the file extension
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || Array.IndexOf(PermittedExtensions, extension) < 0)
            {
                throw new Exception("Invalid File Extension");
            }
            OriginalFileName = Path.GetFileName(filePath);
            SaveImage(File.OpenRead(filePath));
        }

        private void SaveImage(Stream fileStream)
        {
            PrepareFileName();
            if (Compressed)
            {
                CreateTempDirectory();
                string tempFilePath = Path.Combine(TempPath, OutputFileName);
                using (FileStream outputStream = File.Create(tempFilePath))
                {
                    fileStream.CopyTo(outputStream);
                }
                ImageOptimizer.CompressImage(tempFilePath, OutputFilePath);
                DeleteTempFile(tempFilePath);
            }
            else
            {
                using (FileStream outputStream = File.Create(OutputFilePath))
                {
                    fileStream.CopyTo(outputStream);
                }
            }
        }

        private void CreateSubDirectories()
        {
            if (string.IsNullOrWhiteSpace(SaveDirectory))
                SaveDirectory = string.Empty;
            CreateSubDirectories(Path.Combine(BasePath, SaveDirectory));
        }

        private void CreateTempDirectory()
        {
            if (string.IsNullOrWhiteSpace(TempPath))
                TempPath = Path.Combine(BasePath, SaveDirectory, "Temp");
            CreateSubDirectories(TempPath);
        }

        private void CreateSubDirectories(string DirPath)
        {
            var subDirectories = DirPath.Split("\\");
            var subDirectoryPath = string.Empty;
            foreach (var dirName in subDirectories)
            {
                subDirectoryPath = Path.Combine(subDirectoryPath, dirName);
                if (!Directory.Exists(subDirectoryPath))
                {
                    Directory.CreateDirectory(subDirectoryPath);
                }
            }
        }

        private void PrepareFileName()
        {
            CreateSubDirectories();
            string directory = Path.Combine(BasePath, SaveDirectory);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            if (SetNewFileName)
            {
                string fileExtension = Path.GetExtension(OriginalFileName);
                OutputFileName = GetUniqueFileName(directory, fileExtension);
            }
            else
            {
                OutputFileName = OriginalFileName;
            }

            OutputFilePath = Path.Combine(directory, OutputFileName);
        }

        private string GetUniqueFileName(string directory, string extension)
        {
            string fileName = string.Empty;
            string FullPath = string.Empty;
            do
            {
                fileName = GenerateRandomString(FileNameLength - extension.Length).Trim() + extension;
                FullPath = Path.Combine(directory, fileName);
            } while (File.Exists(FullPath));
            return fileName;
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }
            return result.ToString();
        }

        private void DeleteTempFile(string fileName)
        {
            if(File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }
    }
}
