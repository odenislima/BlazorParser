using BlazorInputFile;
using BlazorParser.Domain;
using System.Threading.Tasks;

namespace BlazorParser.Client.Services
{
    public interface IFileUploadService
    {
        Task<ResourceFile> UploadAsync(IFileListEntry file);
    }
}