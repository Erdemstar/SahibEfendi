using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using SahibEfendi.Model.UserModel;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace SahibEfendi.Service.UserService
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        private readonly string key;

        public UserService(IUserDatabaseSetting settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>("Users");
            key = settings.Key;

        }

        public List<User> GetAllUsers()
        {
            return _users.Find<User>(user => true).ToList();
        }

        public User GetUserById(string id)
        {
            return _users.Find(user => user.Id == id).FirstOrDefault();
        }

        public bool CreateUser(User user)
        {
            if (user is null)
            {
                return false;
            }
            _users.InsertOne(user);
            return true;
        }

        public string Authenticate(User u)
        {
            var user = _users.Find(user => user.Email == u.Email && user.Password == u.Password).FirstOrDefault();

            if (user is null)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {

                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),

                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
    
    }
}
