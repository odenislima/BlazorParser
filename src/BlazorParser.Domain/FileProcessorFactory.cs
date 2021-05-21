using BlazorParser.Domain.Interfaces;
using System.IO.Abstractions;

namespace BlazorParser.Domain
{
    public class FileProcessorFactory : IFileProcessorFactory
    {
        private readonly IEventDataParser _eventDataParse;
        private readonly IFileSystem _fileSystem;

        public FileProcessorFactory(
            IEventDataParser eventDataParse,
            IFileSystem fileSystem)
        {
            _eventDataParse = eventDataParse;
            _fileSystem = fileSystem;
        }

        public IFileProcessor Create() => new FileProcessor(_eventDataParse, _fileSystem);
    }
}