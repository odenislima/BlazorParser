using System;

namespace BlazorParser.Domain.Interfaces
{
    public interface IFileProcessor
    {
        IObservable<ErrorData> ErrorsStream { get; }
        IObservable<EventModel> DataStream { get; }

        void Process(ResourceFile resourceFile);
    }
}