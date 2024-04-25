using ImageScaler.Optimizer;
using System.Text;

namespace ImageScaler.Writer
{
    public class ImageWriter
    {
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

        public void SaveImage(Stream imageStream, string fileName)
        {
            OriginalFileName = fileName;
            if (imageStream == null)
                throw new Exception("Stream cannot be Null");

            SaveImage(imageStream);
        }

        public void SaveImage(string filePath)
        {

            OriginalFileName = Path.GetFileName(filePath);
            if (!File.Exists(filePath))
                throw new Exception("File Not Found");

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
    }
}
