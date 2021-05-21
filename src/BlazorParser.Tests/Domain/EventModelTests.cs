using BlazorParser.Domain;
using System;
using Xunit;

namespace BlazorParser.Tests.Domain
{
    public class EventModelTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void It_should_throw_ArgumentException_when_Name_is_empty_or_null(string name)
        {
            Assert.Throws<ArgumentException>(() => new EventModel(name, "test test"));
        }

        [Fact]
        public void It_should_throw_ArgumentException_when_Name_length_is_greather_than_32()
        {
            Assert.Throws<ArgumentException>(() => new EventModel("Test Test Test Test Test Test Test", "test test"));
        }

        [Theory]
        [InlineData("Test Test Test Test Test Test")]
        [InlineData("T")]
        public void It_should_not_throw_ArgumentException_when_Name_length_is_less_than_32(string name)
        {
            var result = Record.Exception(() => new EventModel(name, "test test"));
            Assert.Null(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void It_should_throw_ArgumentException_when_Description_is_empty_or_null(string description)
        {
            Assert.Throws<ArgumentException>(() => new EventModel("test", description));
        }

        [Fact]
        public void It_should_throw_ArgumentException_when_Description_length_is_greather_than_255()
        {
            var description = @"Test Test Test Test Test Test Test Test Test" +
                                "Test Test Test Test Test Test Test Test Test" +
                                "Test Test Test Test Test Test Test Test Test" +
                                "Test Test Test Test Test Test Test Test " +
                                "Test Test Test Test Test Test Test Test Test " +
                                "Test Test Test Test Test Test Test Test Test Test Test Test Test1";

            Assert.Throws<ArgumentException>(() => new EventModel("Test", description));
        }

        [Theory]
        [InlineData("Test")]
        [InlineData("T")]
        public void It_should_not_throw_ArgumentException_when_Description_length_is_less_than_255(string description)
        {
            var result = Record.Exception(() => new EventModel("test", description));
            Assert.Null(result);
        }

        [Fact]
        public void It_should_trim_name_and_description()
        {
            var expecptedValue = "test";
            var result = new EventModel("test ", "test ");
            Assert.Equal(expecptedValue, result.Name);
            Assert.Equal(expecptedValue, result.Description);
        }
    }
}
