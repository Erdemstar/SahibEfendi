using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SahibEfendi.Analyzer.FileAnalyzer;
using SahibEfendi.Handler;
using SahibEfendi.Model.FileModel;
using SahibEfendi.Service.FileService;
using System;
using System.IO;
using File = SahibEfendi.Model.FileModel.File;

namespace SahibEfendi.Controllers
{
    
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FileController : ControllerBase
    {

        private readonly FileService _fileService;

        public FileController(FileService fileService)
        {
            _fileService = fileService;
        }


        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public IActionResult GetAllFile()
        {
            //JWT içerisinden elde ettiğin kullanıcı getir.
            var user = _fileService.GetUserById(UserHandler.FindUserIDFromJWTToken(Request.Headers["Authorization"].ToString().Split(" ")[1]));

            if (user is null)
            {
                return BadRequest("There is an error while getting all files");
            }
            else if (user.Role == "Admin")
            {
                return Ok(_fileService.GetAllFile());
            }
            else {
                return Ok(_fileService.GetAllFilebyUserId(user.Id));
            }

        }

        [Route("{id:length(24)}")]
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public IActionResult GetFileById(string id)
        {
            /**
             * Kullanıcı kontrol edilir eğer yetkisi admin değilse çağırdığı dosya içerisindeki userId mevcut ile eşleşiyorsa izin ver 
             * diğer türlü kız
             */

            //JWT içerisinden elde ettiğin kullanıcı getir.
            var user = _fileService.GetUserById(UserHandler.FindUserIDFromJWTToken(Request.Headers["Authorization"].ToString().Split(" ")[1]));

            //İstek içerisinden aldığın id'de dosya ara ve getir
            var file = _fileService.GetFileById(id);

            if(user is null || file is null)
            {
                return BadRequest("There is an error while getting user or getting in FileController");
            }
            //Eğer kullanıcı yetkisi admin ise file'ı dön veya kullanıcı yetkisi User ve ilgili file'ı sahipse ilgili dosyayı 
            else if (user.Role == "Admin" || (user.Role == "User" && user.Id == file.UserId))
            {
                return Ok(file);
            }

            //Diğer türlü hata ver
            return Unauthorized("There you permission to see that file");
        }

        [Route("{id:length(24)}")]
        [HttpDelete]
        [Authorize(Roles = "Admin,User")]
        public IActionResult DeleteFile(string id)
        {
            /**
             * Kullanıcı admin yetkisiydeyse bu dosya silinecekdiğer türlü mevcut kullanıcı bu dosyaya sahipse silinecektir.
             */

            //JWT içerisinden elde ettiğin kullanıcı getir.
            var user = _fileService.GetUserById(UserHandler.FindUserIDFromJWTToken(Request.Headers["Authorization"].ToString().Split(" ")[1]));

            //İstek içerisinden aldığın id'de dosya ara ve getir
            var file = _fileService.GetFileById(id);

            //Mevcutluğunu kontrol et
            if (user is null || file is null)
            {
                return BadRequest("There is an error while getting user or getting file in FileController");
            }
            //Eğer kullanıcı yetkisi admin ise file'ı dön veya kullanıcı yetkisi User ve ilgili file'ı sahipse ilgili dosyayı sil
            else if (user.Role == "Admin" || (user.Role == "User" && user.Id == file.UserId))
            {
                return Ok(_fileService.DeleteFile(id) );
            }

            //Diğer türlü hata ver
            return Unauthorized("There you permission to delete that file");
        }

        [Route("{id:length(24)}")]
        [HttpPut]
        [Authorize(Roles = "Admin,User")]
        public IActionResult UpdateFile(string id, File Updatefile)
        {

            /**
             * Kullanıcı admin yetkisiydeyse bu dosya güncellenecektikr  türlü mevcut kullanıcı bu dosyaya sahipse güncellenecektir..
             * 
             * File update yapılacağı zaman attack burada gidip File UserID başka bir kullanıcı yapabilir. Burada Ya file userID update verisi
             * olarak kullanıcıdan alınmamalıdır yada sadece kullanıcılak alanlar update işlemine tabi tutulmalıdır. [Risk]
             */

            //JWT içerisinden elde ettiğin kullanıcı getir.
            var user = _fileService.GetUserById(UserHandler.FindUserIDFromJWTToken(Request.Headers["Authorization"].ToString().Split(" ")[1]));

            //Mevcutluğunu kontrol et
            if (user is null)
            {
                return BadRequest("There is an error while getting user in FileController");
            }

            //İstek içerisinden aldığın id'de dosya ara ve getir
            var file = _fileService.GetFileById(id);

            //Mevcutluğunu kontrol et
            if (file is null)
            {
                return BadRequest("There is an error while getting file");
            }

            //Eğer kullanıcı yetkisi admin ise file'ı dön veya kullanıcı yetkisi User ve ilgili file'ı sahipse ilgili dosyayı sil
            if (user.Role == "Admin" || (user.Role == "User" && user.Id == file.UserId))
            {
                return Ok( _fileService.UpdateFile(id, Updatefile) );
            }


            //Diğer türlü hata ver
            return Unauthorized("There you permission to delete that file");
        }

        [Route("{id:length(24)}")]
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public IActionResult FileAnalize(string id)
        {
            //GelenID'nin DB içerisinde varlığını kontrol et varsa ata

            //JWT içerisinden elde ettiğin kullanıcı getir.
            var user = _fileService.GetUserById(UserHandler.FindUserIDFromJWTToken(Request.Headers["Authorization"].ToString().Split(" ")[1]));

            //İstek içerisinden aldığın id'de dosya ara ve getir
            var file = _fileService.GetFileById(id);

            if (user is null || file is null)
            {
                return BadRequest("There is an error while getting user or file in FileController");
            }
            //Eğer kullanıcı rolu admin değilse ve user bu dosya içerisinde görmeye hakkı yoksa aşağıdan false gelir buda false yapar
            //Ozaman analiz etme hata dön
            else if ( ((user.Role == "Admin") || (user.Role == "User" && user.Id == file.UserId)) == false)
            {
                return Unauthorized("There you permission to analize that file");
            }

            var DestinationFile = FileHandler.MainPath() + file.ChangeFileNameWithOutExtension;

            var isDeCompressed = FileHandler.ExtractZip(file.FilePath, DestinationFile);

            //zip'den çıkarma işlemini kontrol et
            if (isDeCompressed is false)
            {
                return BadRequest("There is problem with Decompress file");
            }

            var FileList = FileHandler.DirectoryList(DestinationFile);

            if (FileList is null)
            {
                return BadRequest("There is problem with Listing file");
            }

            //Bulunan file'ları 
            file.FoundedFile = FileList;

            //Bulunan file'ları DB üzerinden güncelleniyor
            var Updatedfile = _fileService.UpdateFile(id, file);

            if (Updatedfile is null)
            {
                return BadRequest("There is problem with Updated file");
            }


            //Burada analyze işlemleri geliyor.

            CVE40444 cve40444 = new CVE40444();

            var control = cve40444.Control(file);

            if (control is true)
            {
                file.IsMaliciousCVE40444 = true;
                _fileService.UpdateFile(id,file);
            }


            //Analizi yapılan dosya (Klasör) siliniyor ilerisi için zip'de silnebilir.

            if (Directory.Exists(file.FilePathWithoutExtension))
            {
                Directory.Delete(file.FilePathWithoutExtension, true);
            }

            //Son olarak kullanıcıya gönderiliyor
            return Ok(file);
        }


        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public IActionResult UploadFile(IFormFile file)
        {
            //JWT içerisinden elde ettiğin kullanıcı getir.
            var user = _fileService.GetUserById(UserHandler.FindUserIDFromJWTToken(Request.Headers["Authorization"].ToString().Split(" ")[1]));

            //Mevcutluğunu kontrol et
            if (user is null)
            {
                return BadRequest("There is an error while getting user details in FileController");
            }

            if (FileHandler.ExtensionControl(file.FileName) == false)
            {
                return BadRequest("Not supported file");
            }

            //File upload'dan gelen dosya uzantısını zip olarak değiştir
            var ChangeFileExtension =  System.IO.Path.ChangeExtension(System.IO.Path.GetFileName(file.FileName), "zip");

            //Değişen dosya uzantısı isminin arasındaki boşlukları kaldır.
            ChangeFileExtension = FileHandler.RemoveCharAndMakeLower(ChangeFileExtension, " ");

            //Path için kullanılacak dosya ismi alınır extension kısmı kaldırır. Böylece zip extract edileceği zaman bu isim kullanılır.
            var ChangeFileNameWithOutExtension = System.IO.Path.GetFileNameWithoutExtension(ChangeFileExtension);

            //File upload yapılacak yer ile güncellenene dosya ismini birleştir.
            var FilePath = FileHandler.MainPath() + ChangeFileExtension;


            //Dosya yolunu aç ve dosya içerisindekini yaz.
            using (var localFile = System.IO.File.OpenWrite(FilePath))
            using (var uploadedFile = file.OpenReadStream())
            {
                uploadedFile.CopyTo(localFile);
            }
            
            //Dosya path'ini ver ve içerisi için bir hash hesaplaması yaptır.
            var hash = FileHandler.CalculateHash(FilePath);


            /*
             * 
            //Hesaplanan hash bizde mevcutsa bunun kontrolleri yapılmış burada git bu hash db'de ara çıktıyı dön
            if (_fileService.GetFileByHash(hash) != null)
            {
                return Ok(_fileService.GetFileByHash(hash));
            }
            *
            */

            var tmp = _fileService.Create(new File()
            {
                UserId = user.Id,
                FileName = file.FileName,
                ChangedFileName = ChangeFileExtension,
                ChangeFileNameWithOutExtension = ChangeFileNameWithOutExtension,
                FilePath = FilePath,
                FilePathWithoutExtension = FileHandler.MainPath() + ChangeFileNameWithOutExtension,
                FileHash = hash,
                IsMaliciousCVE40444 = false,
                IsDeleted = false,
                UploadedDate = DateTime.Now
            });

            return Ok(tmp);
        }



    }
 }
