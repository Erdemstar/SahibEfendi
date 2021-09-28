using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SahibEfendi.Model.UserModel
{
    public class UserDatabaseSetting : IUserDatabaseSetting
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string Key { get; set; }
        public string Issuer { get; set; }
    }
    public interface IUserDatabaseSetting
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string Key { get; set; }
        string Issuer { get; set; }


    }
}
