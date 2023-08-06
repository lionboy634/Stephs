using Microsoft.Extensions.Logging;
using Stephs_Shop.Models;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Stephs_Shop.Services
{
    public interface IFileService
    {
        void uploadFile(Binary filebytes);
    }


    public class FileService : IFileService
    {
        private ILogger<IFileService> _logger;
        public FileService(ILogger<FileService> logger)
        {
            _logger = logger;
        }
        public void uploadFile(Binary FileContent)
        {
            try
            {
				var filepath = Path.Combine(Directory.GetCurrentDirectory(), @"Uploads", FileContent.filename);
                if(!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

				File.WriteAllBytes(filepath, FileContent.filebytes);
			}
            catch(System.Exception ex)
            {
                _logger.LogError($"Upload file With Errors: {ex.Message}");
            }
        }
    }
}
