using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SahibEfendi.Model.FileModel
{
    public class File
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string UserId { get; set; }

        public string FileName { get; set; }

        public string ChangedFileName { get; set; }

        public string ChangeFileNameWithOutExtension { get; set; }

        public string FilePath { get; set; }

        public string FilePathWithoutExtension { get; set; }

        public string FileHash { get; set; }

        public string[] FoundedFile { get; set; }

        public bool IsMaliciousCVE40444 { get; set; }
        
        public DateTime UploadedDate { get; set; }
        
        public DateTime DeletedDate { get; set; }
        
        public bool IsDeleted { get; set; }
    }
}
