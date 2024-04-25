namespace ImageScaler.Models
{
    public class FileDataModel
    {
        public byte[] Data;
        public string ContentType;
        public FileDataModel(byte[] data, string contentType)
        {
            Data = data;
            ContentType = contentType;
        }
    }
}
