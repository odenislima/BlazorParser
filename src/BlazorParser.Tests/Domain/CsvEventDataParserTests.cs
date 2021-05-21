using BlazorParser.Domain;
using BlazorParser.Domain.Exceptions;
using System;
using Xunit;

namespace BlazorParser.Tests.Domain
{
    public class CsvEventDataParserTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void It_should_throw_exception_when_line_length_is_less_then_2(string line)
        {
            Assert.Throws<ParseLineException>(() =>
            {
                var sut = new CsvEventDataParser();
                sut.Parse(line);
            });
        }

        [Theory]
        [InlineData("Test Test Test ")]
        [InlineData("Test,Test,Test")]
        [InlineData("TestTestTest")]
        public void It_should_throw_exception_when_line_delimitator_is_not_semicolon(string line)
        {
            Assert.Throws<ParseLineException>(() =>
            {
                var sut = new CsvEventDataParser();
                sut.Parse(line);
            });
        }

        [Fact]
        public void It_should_parse_name_and_description_when_line_does_not_contain_start_and_end_dates()
        {
            var expectedName = "Event-Name-1";
            var expectedDescription = "Event-Description-1";
            var line = $"{expectedName}; {expectedDescription};";
            var sut = new CsvEventDataParser();

            var actualResult = sut.Parse(line);

            Assert.Equal(expectedName, actualResult.Name);
            Assert.Equal(expectedDescription, actualResult.Description);
            Assert.Null(actualResult.Start);
            Assert.Null(actualResult.End);
        }

        [Fact]
        public void It_should_parse_name_description_and_start_date_when_line_does_not_contain_end_date()
        {
            var startDataStr = "2015-08-09T08:38:49-07:00";
            var expectedName = "Event-Name-1";
            var expectedDescription = "Event-Description-1";
            var expectedStartDate = DateTimeOffset.Parse(startDataStr);
            var line = $"{expectedName};{expectedDescription};{startDataStr}";
            var sut = new CsvEventDataParser();

            var actualResult = sut.Parse(line);

            Assert.Equal(expectedName, actualResult.Name);
            Assert.Equal(expectedDescription, actualResult.Description);
            Assert.Equal(expectedStartDate, actualResult.Start);
            Assert.Null(actualResult.End);
        }

        [Fact]
        public void It_should_parse_all_properties_when_line_contais_all_values()
        {
            var startDataStr = "2015-08-09T08:38:49-07:00";
            var endDataStr = "2007-03-01T13:00:00Z";
            var expectedName = "Event-Name-1";
            var expectedDescription = "Event-Description-1";
            var expectedStartDate = DateTimeOffset.Parse(startDataStr);
            var expectedEndDate = DateTimeOffset.Parse(endDataStr);
            var line = $"{expectedName};{expectedDescription};{startDataStr};{endDataStr}";

            var sut = new CsvEventDataParser();

            var actualResult = sut.Parse(line);

            Assert.Equal(expectedName, actualResult.Name);
            Assert.Equal(expectedDescription, actualResult.Description);
            Assert.Equal(expectedStartDate, actualResult.Start);
            Assert.Equal(expectedEndDate, actualResult.End);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("aa/bb/cc")]
        [InlineData("123456")]
        [InlineData("aa/11/bb")]
        public void It_should_parse_dates_when_value_cannot_be_conveter(string value)
        {   
            var expectedName = "Event-Name-1";
            var expectedDescription = "Event-Description-1";            
            var line = $"{expectedName};{expectedDescription};{value};{value}";
            var sut = new CsvEventDataParser();

            var actualResult = sut.Parse(line);

            Assert.Null(actualResult.Start);
            Assert.Null(actualResult.End);
        }

    }
}
