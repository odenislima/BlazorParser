using BlazorInputFile;
using BlazorParser.Client.Services;
using BlazorParser.Domain;
using BlazorParser.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace BlazorParser.Client.ViewModels
{
    public class EventsViewModel : IEventsViewModel
    {
        private static string[] ValidFileExtensions =
        {
            ".csv",
            ".txt"
        };

        private static int MaxFileSizeInMiB = 3;

        private readonly IFileProcessorFactory _fileProcessorFactory;
        private readonly IFileUploadService _fileUploadService;

        public List<EventModel> Events { get; }

        public List<ErrorData> Errors { get; }

        public string ErrorMessage { get; private set; }

        public IFileListEntry SelectedFile { get; private set; }

        private IDisposable _subscription;

        public string SuccessMessage { get; private set; }

        public bool HasFinished { get; private set; }

        public bool IsProcessing { get; private set; }

        public bool ShowErrorMessage { get; private set; }

        public bool ShowSuccessMessage { get; private set; }

        public EventsViewModel(
            IFileProcessorFactory fileProcessorFactory,
            IFileUploadService fileUploadService)
        {
            _fileProcessorFactory = fileProcessorFactory;
            _fileUploadService = fileUploadService;

            Events = new List<EventModel>();
            Errors = new List<ErrorData>();
        }

        public async Task FileSelectedHandler(IFileListEntry[] files)
        {
            Events.Clear();
            Errors.Clear();

            SelectedFile = files.FirstOrDefault();

            if (SelectedFile != null)
            {
                var sizeMiB = ConvertToBytes(SelectedFile.Size);

                if (sizeMiB > MaxFileSizeInMiB)
                {
                    ShowErrorMessage = true;
                    ErrorMessage = $"File is to large. Max size allowed is {MaxFileSizeInMiB} bytes.";
                    return;
                }

                if (!ValidFileExtensions.Any(_ => SelectedFile.Name.EndsWith(_)))
                {
                    ShowErrorMessage = true;
                    ErrorMessage = $"Please only allow files [{ string.Join(",", ValidFileExtensions) }]";
                    return;
                }

                try
                {
                    IsProcessing = true;

                    var resourceFile = await _fileUploadService.UploadAsync(SelectedFile);
                    var fileProcessor = _fileProcessorFactory.Create();
                    SubscribeOnStream(fileProcessor);
                    fileProcessor.Process(resourceFile);

                    return;
                }
                catch (Exception)
                {
                    ShowError("An error occurred while processing the file. Please try later.");
                    ResetStateWhenErroring();
                    return;
                }
            }
        }

        private void ShowError(string message)
        {
            ErrorMessage = message;
            ShowErrorMessage = true;
        }

        public void CloseSuccessMessageHanlder() => ShowSuccessMessage = false;
        public void CloseErrorMessageHanlder() => ShowErrorMessage = false;

        public void Dispose() => ReaseResources();

        private float ConvertToBytes(long size) => (size / 1024f) / 1024f;

        private void OnNewError(ErrorData error) => Errors.Add(error);

        private void OnNewEvent(EventModel eventModel) => Events.Add(eventModel);

        private void OnDataStreamCompleted()
        {
            ReaseResources();
            HasFinished = true;
            IsProcessing = false;
            SuccessMessage = $"The file {SelectedFile.Name} has been processed.";
        }

        private void ReaseResources() => _subscription?.Dispose();

        private void ResetStateWhenErroring()
        {
            HasFinished = false;
            IsProcessing = false;
            SelectedFile = null;
            ErrorMessage = string.Empty;
        }

        private void SubscribeOnStream(IFileProcessor fileProcessor)
        {
            fileProcessor
                    .DataStream
                    .Subscribe(OnNewEvent, OnDataStreamCompleted);

            _subscription = fileProcessor.ErrorsStream.Subscribe(OnNewError);
        }
    }
}
