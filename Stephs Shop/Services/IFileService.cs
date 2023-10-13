using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stephs_Shop.Models;
using Stephs_Shop.Repositories;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Stephs_Shop.Services
{
    public interface IFileService
    {
        Task uploadFile(Binary filebytes, int productId);
    }


    public class FileService : IFileService
    {
        private ILogger<IFileService> _logger;
        private readonly HttpClient _client;
        private readonly IProductRepository _productRepository;
        public FileService(ILogger<FileService> logger, IProductRepository productRepository)
        {
            _logger = logger;
            _client = new HttpClient();
            _productRepository = productRepository;
        }
        public async Task uploadFile(Binary FileContent, int productId)
        {
            try
            {
				/*var filepath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Uploads", FileContent.filename);
                var fi = new FileInfo(filepath);
                var identity = System.Security.Principal.WindowsIdentity.GetCurrent();*/

                //jsonContent, FormUlrcONTNETN, MultipartFormDataContnet, ByteArrayContent
				
				//_client.BaseAddress = new System.Uri("https://www.filestackapi.com");
                using(var memStream = new MemoryStream(FileContent.filebytes))
                {
                    using(var byteStream = new StreamContent(memStream))
                    {/*
						var content = new MultipartFormDataContent();
						var binaryContent = new ByteArrayContent(FileContent.filebytes);
						binaryContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
						content.Add(binaryContent, "content", FileContent.filename);

						MultipartFormDataContent multiContent = new MultipartFormDataContent
						{
							{ byteStream, "file", FileContent.filename }
						};

						var response = await _client.PostAsync("api/store/S3?key=Ajy3fii4eQNWmejyhoia4z", content);
						response.EnsureSuccessStatusCode();
                        
                        var content_result = await response.Content.ReadAsStringAsync();
                        var stream_result = await response.Content.ReadAsStreamAsync();
						var result = JsonConvert.DeserializeObject(content_result);*/
						string apiKey = "Ajy3fii4eQNWmejyhoia4z";
						ByteArrayContent binaryContent = new ByteArrayContent(FileContent.filebytes);

						// Create the URL
						string apiUrl = $"https://www.filestackapi.com/api/store/S3?key={apiKey}";

						// Send the POST request with the binary content
						HttpResponseMessage response = await _client.PostAsync(apiUrl, binaryContent);
                        if (response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadAsStringAsync();
                            var uploadResponse = JsonConvert.DeserializeObject<FileStackResponse>(result);

                            await _productRepository.UpdateUploadedImage(productId, uploadResponse);
                        }


					}
                }
    //            var content = new
    //            {
    //                data = FileContent.filebytes,
    //                filename = FileContent.filename
    //            };
    //            StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(content));

				//var response = await _client.PostAsync("api/store/S3?key=Ajy3fii4eQNWmejyhoia4z", multiContent);
    //            response.EnsureSuccessStatusCode();
    //            var content_result = await response.Content.ReadAsStringAsync();
    //            var result = JsonConvert.DeserializeObject(content_result);
				//Directory.CreateDirectory(Path.GetDirectoryName(filepath));
				//File.WriteAllBytes(Path.GetDirectoryName(filepath), FileContent.filebytes);
			}
            catch(System.Exception ex)
            {
                _logger.LogError($"Upload file With Errors: {ex.Message}");
                throw;
            }
        }
    }
}
