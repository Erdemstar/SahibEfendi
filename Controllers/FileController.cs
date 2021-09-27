using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SahibEfendi.Analyzer.FileAnalyzer;
using SahibEfendi.Handler.File;
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
        public IActionResult GetAllFile()
        {

            var files = _fileService.GetAllFile();
            if (files is null)
            {
                return BadRequest("There is an error while getting all files");
            }
            return Ok(files);
        }

        [Route("{id:length(24)}")]
        [HttpGet]
        public IActionResult GetFileById(string id)
        {
            var file = _fileService.GetFileById(id);
            if (file is null)
            {
                return BadRequest("There is an error while getting file");
            }
            return Ok(file);
        }

        [Route("{id:length(24)}")]
        [HttpDelete]
        public IActionResult DeleteFile(string id)
        {
            var file = _fileService.DeleteFile(id);
            if (file is null)
            {
                return BadRequest("There is an error while getting file");
            }
            return Ok(file);

        }


        [Route("{id:length(24)}")]
        [HttpPut]
        public IActionResult UpdateFile(string id, File file)
        {

            var ufile = _fileService.UpdateFile(id, file);
            if (ufile is null)
            {
                return BadRequest("There is an error while updating file");
            }
            return Ok(ufile);
        }

        [Route("{id:length(24)}")]
        [HttpGet]
        public IActionResult FileAnalize(string id)
        {
            //GelenID'nin DB içerisinde varlığını kontrol et varsa ata
            var file = _fileService.GetFileById(id);

            if (file is null)
            {
                return NotFound("There is no file id " + id);
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
        public IActionResult UploadFile(IFormFile file)
        {
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

            //Hesaplanan hash bizde mevcutsa bunun kontrolleri yapılmış burada git bu hash db'de ara çıktıyı dön
            if (_fileService.GetFileByHash(hash) != null)
            {
                return Ok(_fileService.GetFileByHash(hash));
            }

            var tmp = _fileService.Create(new File()
            {
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
