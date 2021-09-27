using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using File = SahibEfendi.Model.FileModel.File;

namespace SahibEfendi.Analyzer.FileAnalyzer
{
    public class CVE40444
    {
        public string BlackListRegex = @"oleObject.*mhtml";

        public bool Control(File file)
        {
            //Git file içerisindeki path'leri tek tek al ve içerisindeki tek tek oku. Bu path'ler içerisinde oleObject varsa zafiyet vardır diye düşün
            // zafiyet tespit edildiği için true dön

            var file_paths = file.FoundedFile;

            foreach(var item in file_paths)
            {
                using(StreamReader sr = new StreamReader(item)) 
                {
                    string contents = sr.ReadToEnd().ToLower();
                    if (System.Text.RegularExpressions.Regex.IsMatch(contents, BlackListRegex, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    {
                        return true;
                    }

                }
            }
            return false;
        }
    }
}
