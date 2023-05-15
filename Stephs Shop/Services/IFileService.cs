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
			// if(File.Exists())
			var filepath = Path.Combine("~/uploads/", FileContent.filename);
			if (File.Exists(filepath))
            {
                _logger.LogDebug("File already exists");
                return;
            }
           
            File.WriteAllBytes(filepath , FileContent.filebytes);
           
        }
    }
}
