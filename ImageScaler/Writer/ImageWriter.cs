using ImageScaler.Optimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageScaler.Writer
{
    public class ImageWriter
    {
        public string BasePath { get; }
        public string SaveDirectory { get; set; }
        public bool Compressed { get; set; } = false;
        public string TempPath { get; set; }

        public ImageWriter(string basePath)
        {
            BasePath = basePath;
            if (!Directory.Exists(BasePath))
            {
                throw new Exception($"Directory({BasePath}) Not Found");
            }
        }

        public void SaveImage(string filePath)
        {
            CreateSubDirectories();

            string directory = Path.Combine(BasePath, SaveDirectory);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            string fileName = Path.GetFileName(filePath);
            string savedFilePath = Path.Combine(directory, fileName);
            if (Compressed)
            {
                CreateTempDirectory();
                string tempFilePath = Path.Combine(TempPath, fileName);
                SaveImage(filePath, tempFilePath);
                ImageOptimizer.CompressImage(tempFilePath, savedFilePath);
            }
            else
            {
                SaveImage(filePath, savedFilePath);
            }
        }

        private void SaveImage(string filePath, string OutputFilePath)
        {
            if (File.Exists(filePath))
            {
                using (FileStream fileStream = File.OpenRead(filePath))
                {
                    using (FileStream outputStream = File.Create(OutputFilePath))
                    {
                        fileStream.CopyTo(outputStream);
                    }
                }
            }
            else
            {
                throw new Exception("File Not Found");
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
    }
}
