using MongoDB.Driver;
using SahibEfendi.Model.FileModel;
using System.Collections.Generic;

namespace SahibEfendi.Service.FileService
{
    public class FileService
    {
        private readonly IMongoCollection<File> _files;

        public FileService(IFileDatabaseSetting settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _files = database.GetCollection<File>("ZipFiles");
        }

        public File GetFileById(string id)
        {
            // İlgili id'yi getir ve buna ek olarak silinmemiş olsun
            return _files.Find<File>(f => f.Id == id && f.IsDeleted == false).FirstOrDefault();
            
        }

        public File GetFileByHash(string FileHash)
        {
            return _files.Find<File>(f => f.FileHash == FileHash).FirstOrDefault();
            
        }

        public List<File> GetAllFile()
        {
            // Tüm verileri getir ve buna ek olarak silinmemiş olsun
            return _files.Find<File>(f => true && f.IsDeleted == false).ToList();
        }

        public File UpdateFile(string id, File file)
        {
            // ilgili id'yi getir ve buna ek olarak silinmemiş olsun
            return _files.FindOneAndReplace<File>(f => f.Id == id && f.IsDeleted == false, file);
        }

        public File DeleteFile(string id)
        {
            try
            {
                var file = GetFileById(id);
                file.IsDeleted = true;
                UpdateFile(id, file);
                return file;
            }
            catch
            {
                return null;
            }
            
        }

        public File Create(File file)
        {
            _files.InsertOne(file);
            return file;
        }



    }
    
}
