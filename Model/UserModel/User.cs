using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SahibEfendi.Model.UserModel
{
    public class User
    {

        public User()
        {
            CreateDate = DateTime.Now;
            UpdateDate = DateTime.Now;
            isDeleted = false;
            Role = "User";
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public string Id { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        [JsonIgnore]
        public string Role { get; set; }

        [JsonIgnore]
        public DateTime CreateDate { get; set; }

        [JsonIgnore]
        public DateTime UpdateDate { get; set; }

        [JsonIgnore]
        public bool isDeleted { get; set; }
    }
}
