using BlazorParser.Domain;
using BlazorParser.Domain.Exceptions;
using BlazorParser.Domain.Interfaces;
using Microsoft.Reactive.Testing;
using Moq;
using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reactive;
using System.Text;
using Xunit;

namespace BlazorParser.Tests.Domain
{
    public class FileProcessorTests
    {
        [Fact]
        public void It_should_push_error_data_and_complete_stream_data_when_resource_file_is_null()
        {
            var fileSystemMock = new MockFileSystem();
            var eventDataParserMock = new Mock<IEventDataParser>();
            var eventModel = new EventModel("test", "test", null, null);
            eventDataParserMock.Setup(_ => _.Parse(It.IsAny<string>()))
                .Returns(eventModel);
            var testSchedule = new TestScheduler();
            var dataStreamObservable = testSchedule.CreateObserver<EventModel>();
            var errorStreamObservable = testSchedule.CreateObserver<ErrorData>();
            var sut = new FileProcessor(eventDataParserMock.Object, fileSystemMock);

            fileSystemMock.AddFile(@"c:\test\", new MockFileData("Test"));

            using var errorSub = sut.ErrorsStream.Subscribe(errorStreamObservable.OnNext);
            using var dataSub = sut.DataStream.Subscribe(dataStreamObservable);


            sut.Process(null);

            Assert.NotNull(dataStreamObservable.Messages);
            var value = dataStreamObservable.Messages[0].Value;
            Assert.Equal(NotificationKind.OnCompleted, value.Kind);
            Assert.NotEmpty(errorStreamObservable.Messages);
        }

        [Fact]
        public void It_should_push_error_when_parser_throws_exceptions()
        {
            var expectedFirstError = "error1";
            var expectedSecondError = "error2";
            var fileSystemMock = new MockFileSystem();
            var eventDataParserMock = new Mock<IEventDataParser>();
            var testSchedule = new TestScheduler();
            var dataStreamObservable = testSchedule.CreateObserver<EventModel>();
            var errorStreamObservable = testSchedule.CreateObserver<ErrorData>();
            var sut = new FileProcessor(eventDataParserMock.Object, fileSystemMock);

            eventDataParserMock.SetupSequence(_ => _.Parse(It.IsAny<string>()))
               .Throws(new ParseLineException(expectedFirstError, expectedFirstError))
               .Throws(new Exception(expectedSecondError));

            var path = @"c:\test\";
            fileSystemMock.AddFile(path, new MockFileData(""));
            var file = new MockFile(fileSystemMock);
            file.AppendAllLines(path, new[] { "line 1", "line 2", "line 3" });

            using var errorSub = sut.ErrorsStream.Subscribe(errorStreamObservable.OnNext);
            using var dataSub = sut.DataStream.Subscribe(dataStreamObservable.OnNext);


            sut.Process(new ResourceFile("test"));

            Assert.Equal(2, errorStreamObservable.Messages.Count);
            var error1 = errorStreamObservable.Messages[0].Value.Value;
            var error2 = errorStreamObservable.Messages[1].Value.Value;
            Assert.Equal(expectedFirstError, error1.Error);
            Assert.Equal(expectedSecondError, error2.Error);
        }

        [Fact]
        public void It_should_not_stop_parsing_when_parser_throws_exceptions()
        {
            var expectedExceptionError = "parse error";
            var fileSystemMock = new MockFileSystem();
            var eventDataParserMock = new Mock<IEventDataParser>();
            var expectedEventModel = new EventModel("test", "test", null, null);
            eventDataParserMock.SetupSequence(_ => _.Parse(It.IsAny<string>()))
                .Throws(new ParseLineException(expectedExceptionError, expectedExceptionError))
                .Throws(new Exception(expectedExceptionError))
                .Returns(expectedEventModel);

            var testSchedule = new TestScheduler();
            var dataStreamObservable = testSchedule.CreateObserver<EventModel>();
            var errorStreamObservable = testSchedule.CreateObserver<ErrorData>();
            var sut = new FileProcessor(eventDataParserMock.Object, fileSystemMock);
            var path = @"c:\test\";
            fileSystemMock.AddFile(path, new MockFileData(""));
            var file = new MockFile(fileSystemMock);
            file.AppendAllLines(path, new[] { "line 1", "line 2", "line 3" });

            using var errorSub = sut.ErrorsStream.Subscribe(errorStreamObservable.OnNext);
            using var dataSub = sut.DataStream.Subscribe(dataStreamObservable.OnNext);


            sut.Process(new ResourceFile("test"));

            Assert.NotEmpty(dataStreamObservable.Messages);
            var first = dataStreamObservable.Messages[0].Value.Value;
            Assert.Equal(expectedEventModel.Name, first.Name);
            Assert.Equal(expectedEventModel.Description, first.Description);
            Assert.Equal(2, errorStreamObservable.Messages.Count);
        }

        [Fact]
        public void It_should_push_on_completed_when_an_error_occurrs_while_reading_the_resource_file()
        {
            var fileSystemMock = new Mock<IFileSystem>();
            var eventDataParserMock = new Mock<IEventDataParser>();
            var testSchedule = new TestScheduler();
            var dataStreamObservable = testSchedule.CreateObserver<EventModel>();
            var errorStreamObservable = testSchedule.CreateObserver<ErrorData>();
            var sut = new FileProcessor(eventDataParserMock.Object, fileSystemMock.Object);

            var fileMock = new Mock<IFile>();
            fileMock.Setup(_=> _.ReadLines(It.IsAny<string>(), It.IsAny<Encoding>()))
            .Throws(new Exception("error"));
            fileSystemMock.SetupGet(_ => _.File).Returns(fileMock.Object);
            
            using var errorSub = sut.ErrorsStream.Subscribe(errorStreamObservable.OnNext);
            using var dataSub = sut.DataStream.Subscribe(dataStreamObservable);

            sut.Process(new ResourceFile("test"));

            Assert.Equal(1, errorStreamObservable.Messages.Count);
            Assert.Equal(1, dataStreamObservable.Messages.Count);
            var first = dataStreamObservable.Messages[0].Value;
            Assert.Equal(NotificationKind.OnCompleted, first.Kind);            
        }

        [Fact]
        public void It_should_publish_on_completed_there_are_not_more_line_to_parse()
        {
            var fileSystemMock = new MockFileSystem();
            var eventDataParserMock = new Mock<IEventDataParser>();
            var expectedEventModel = new EventModel("test", "test", null, null);
            var expectedEventModel2 = new EventModel("test2", "test2", null, null);
            eventDataParserMock.SetupSequence(_ => _.Parse(It.IsAny<string>()))
                .Returns(expectedEventModel)
                .Returns(expectedEventModel2);

            var testSchedule = new TestScheduler();
            var dataStreamObservable = testSchedule.CreateObserver<EventModel>();
            var errorStreamObservable = testSchedule.CreateObserver<ErrorData>();
            var sut = new FileProcessor(eventDataParserMock.Object, fileSystemMock);
            var path = @"c:\test\";
            fileSystemMock.AddFile(path, new MockFileData(""));
            var file = new MockFile(fileSystemMock);
            file.AppendAllLines(path, new[] { "line 1", "line 2" });

            using var errorSub = sut.ErrorsStream.Subscribe(errorStreamObservable.OnNext);
            using var dataSub = sut.DataStream.Subscribe(dataStreamObservable);

            sut.Process(new ResourceFile("test"));

            Assert.Equal(3, dataStreamObservable.Messages.Count);
            var first = dataStreamObservable.Messages[0].Value.Value;
            var second = dataStreamObservable.Messages[1].Value.Value;
            var last = dataStreamObservable.Messages[2].Value;

            Assert.Equal(expectedEventModel.Name, first.Name);
            Assert.Equal(expectedEventModel.Description, first.Description);
            Assert.Equal(expectedEventModel2.Name, second.Name);
            Assert.Equal(expectedEventModel2.Description, second.Description);
            Assert.Equal(NotificationKind.OnCompleted, last.Kind);

            Assert.Empty(errorStreamObservable.Messages);
        }
    }
}