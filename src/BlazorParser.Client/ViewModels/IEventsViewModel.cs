using BlazorInputFile;
using BlazorParser.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorParser.Client.ViewModels
{
    public interface IEventsViewModel : IDisposable
    {
        List<ErrorData> Errors { get; }
        List<EventModel> Events { get; }        
        string ErrorMessage { get; }
        bool IsProcessing { get; }
        IFileListEntry SelectedFile { get; }
        string SuccessMessage { get; }
        bool HasFinished { get; }

        bool ShowErrorMessage { get; }
        void CloseErrorMessageHanlder();

        bool ShowSuccessMessage { get; }
        void CloseSuccessMessageHanlder();

        Task FileSelectedHandler(IFileListEntry[] files);
    }
}