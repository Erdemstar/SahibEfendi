using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace SahibEfendi.Handler.File
{
    public class FileHandler
    {
        public static string MainPath() {

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Environment.CurrentDirectory + "\\UploadedFile\\";
            }
            return Environment.CurrentDirectory + "//UploadedFile//";
        }
        
        public static String[] AllowedExtesion = { ".xlsx", ".xlsm", ".xlsb", ".xltx" , ".xltm" , ".docx"};

        public static bool ExtensionControl(string FileName)
        {
            if (AllowedExtesion.Contains( Path.GetExtension(FileName)) )
            {
                return true;
            }
            return false;
        }

        public static string CalculateHash(string FilePath)
        {
            SHA256 Sha256 = SHA256.Create();

            using (FileStream stream = System.IO.File.OpenRead(FilePath))
            {
                var bytes = Sha256.ComputeHash(stream);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public static string RemoveCharAndMakeLower(string name , string chars)
        {
            return name.Replace(chars, "").ToLower();
        }
        
        public static bool ExtractZip(string FilePath, string DestinationPath)
        {
            // Git zip'i extract et bir problem yaşarsan ve bir DestinationPath  folder'ini sil
            try
            {
                ZipFile.ExtractToDirectory(FilePath, DestinationPath);
                return true;
            }
            catch
            {
                try
                {
                    Directory.Delete(DestinationPath);
                }
                catch
                {
                    return false;
                }
                return false;
            }
        }

        public static string[] DirectoryList(string DestinationDirectory)
        {
            //Verilen Folder içerisindeki dosyaları tek tek elde et eğer bir hata varsa null dön
             string[] files = { };
   
            try
            {
                files = Directory.GetFiles(DestinationDirectory, "*.*", SearchOption.AllDirectories);
                return files;
            }
            catch (Exception excpt)
            {
                return null;
            }
        }

        public static string SizeConverter(long bytes)
        {
            var fileSize = new decimal(bytes);
            var kilobyte = new decimal(1024);
            var megabyte = new decimal(1024 * 1024);
            var gigabyte = new decimal(1024 * 1024 * 1024);

            switch (fileSize)
            {
                case var _ when fileSize < kilobyte:
                    return $"Less then 1KB";
                case var _ when fileSize < megabyte:
                    return $"{Math.Round(fileSize / kilobyte, 0, MidpointRounding.AwayFromZero):##,###.##}KB";
                case var _ when fileSize < gigabyte:
                    return $"{Math.Round(fileSize / megabyte, 2, MidpointRounding.AwayFromZero):##,###.##}MB";
                case var _ when fileSize >= gigabyte:
                    return $"{Math.Round(fileSize / gigabyte, 2, MidpointRounding.AwayFromZero):##,###.##}GB";
                default:
                    return "n/a";
            }
        }
    }
}
