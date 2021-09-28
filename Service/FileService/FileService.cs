using MongoDB.Driver;
using SahibEfendi.Model.FileModel;
using SahibEfendi.Model.UserModel;
using System.Collections.Generic;

namespace SahibEfendi.Service.FileService
{
    public class FileService
    {
        private readonly IMongoCollection<File> _files;

        private readonly IMongoCollection<User> _users;

        
        // File Service Function

        public FileService(IFileDatabaseSetting settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _files = database.GetCollection<File>("ZipFiles");
            _users = database.GetCollection<User>("Users");
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

        public List<File> GetAllFilebyUserId(string userId)
        {
            // Tüm verileri getir ve buna ek olarak silinmemiş olsun
            return _files.Find<File>(f => f.UserId == userId && f.IsDeleted == false).ToList();
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

        // UserServiceFunction
        
        public User GetUserById(string id)
        {
            // kullanıcı silinmemiş ve Id'sı mevcutsa getir.
            return _users.Find(user => user.Id == id && user.isDeleted == false).FirstOrDefault();
        }


    }
    
}
