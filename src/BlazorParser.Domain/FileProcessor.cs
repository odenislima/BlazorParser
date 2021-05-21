using BlazorParser.Domain.Exceptions;
using System.IO;
using System;
using System.Text;
using System.Collections.Generic;
using System.Reactive.Subjects;
using BlazorParser.Domain.Interfaces;
using System.IO.Abstractions;

namespace BlazorParser.Domain
{
    public class FileProcessor : IFileProcessor
    {
        private readonly IEventDataParser _eventDataParser;
        private readonly IFileSystem _fileSystem;
        private static readonly Encoding Encoding = Encoding.UTF8;

        private readonly Subject<ErrorData> _errorStream = new Subject<ErrorData>();
        private readonly Subject<EventModel> _dataStream = new Subject<EventModel>();

        public FileProcessor(IEventDataParser eventDataParser, IFileSystem fileSystem)
        {
            _eventDataParser = eventDataParser;
            _fileSystem = fileSystem;
        }

        public IObservable<ErrorData> ErrorsStream => _errorStream;
        public IObservable<EventModel> DataStream => _dataStream;

        public void Process(ResourceFile resourceFile)
        {
            if (resourceFile == null)
            {
                _errorStream.OnNext(new ErrorData("Invalid resource file", string.Empty));
                _dataStream.OnCompleted();
                return;
            }

            try
            {
                foreach (var line in ReadLines(resourceFile))
                {
                    try
                    {
                        var eventModel = _eventDataParser.Parse(line);

                        _dataStream.OnNext(eventModel);
                    }
                    catch (ParseLineException ex)
                    {
                        _errorStream.OnNext(ex.ErrorData);
                    }
                    catch (Exception ex)
                    {
                        _errorStream.OnNext(new ErrorData(ex.Message, line));
                    }
                }
            }
            catch (Exception ex)
            {
                _errorStream.OnNext(new ErrorData($"The following error [{ex.Message}] has occurred while read the resource."));
            }

            _dataStream.OnCompleted();
        }

        private IEnumerable<string> ReadLines(ResourceFile resourceFile)
        {
            foreach (var line in _fileSystem.File.ReadLines(resourceFile.Location, Encoding))
            {
                yield return line;
            }
        }
    }
}