using BlazorInputFile;
using BlazorParser.Client.ViewModels;
using BlazorParser.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorParser.Tests.Pages.Stubs
{
    public class EventsViewModelStub : IEventsViewModel
    {
        public List<ErrorData> Errors { get; set; } = new List<ErrorData>();

        public List<EventModel> Events { get; set; } = new List<EventModel>();

        public string ErrorMessage { get; set; }

        public bool IsProcessing { get; set; }

        public IFileListEntry SelectedFile { get; set; }

        public string SuccessMessage { get; set; }

        public bool HasFinished { get; set; }

        public bool ShowErrorMessage { get; set; }

        public bool ShowSuccessMessage { get; set; }

        public void CloseErrorMessageHanlder()
        {
            
        }

        public void CloseSuccessMessageHanlder()
        {
            
        }

        public void Dispose()
        {
            
        }

        public Task FileSelectedHandler(IFileListEntry[] files)
        {
            return Task.Delay(0);
        }
    }
}
