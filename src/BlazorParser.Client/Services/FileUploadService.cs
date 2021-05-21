using BlazorInputFile;
using BlazorParser.Domain;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace BlazorParser.Client.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;

        public FileUploadService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public async Task<ResourceFile> UploadAsync(IFileListEntry fileEntry)
        {
            var path = Path.Combine(_environment.ContentRootPath, "UploadedFiles", fileEntry.Name);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using var ms = new MemoryStream();
            await fileEntry.Data.CopyToAsync(ms);
            using FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write);
            ms.WriteTo(file);

            return new ResourceFile(path);
        }
    }
}
