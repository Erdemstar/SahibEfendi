using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SahibEfendi.Model.FileModel
{
    public class FileDatabaseSetting : IFileDatabaseSetting
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
    public interface IFileDatabaseSetting
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }

}
