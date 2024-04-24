using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageScaler.Storer
{
    public class ImageStorer
    {
        public string BasePath { get; set; }
        public string SaveDirectory { get; set; }
        public bool Compressed { get; set; } = false;

        public ImageStorer(string basePath) 
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

            if (File.Exists(filePath))
            {
                using (FileStream fileStream = File.OpenRead(filePath))
                {
                    string directory = Path.Combine(BasePath, SaveDirectory);
                    Directory.CreateDirectory(directory);
                    string fileName = Path.GetFileName(filePath);
                    string savedFilePath = Path.Combine(directory, fileName);
                    using (FileStream outputStream = File.Create(savedFilePath))
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
            var subDirectories = SaveDirectory.Split("\\");
            var subDirectoryPath = BasePath;
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
