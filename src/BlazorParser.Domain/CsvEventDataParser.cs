using BlazorParser.Domain.Exceptions;
using BlazorParser.Domain.Interfaces;
using System;

namespace BlazorParser.Domain
{
    public class CsvEventDataParser : IEventDataParser
    {
        private static readonly char Delimitator = ';';

        public EventModel Parse(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                throw new ParseLineException("Invalid line format.", line);

            var columns = line.Split(new char[] { Delimitator });

            EnsureLineIsValid(line, columns);

            if (columns.Length == 2)
                return new EventModel(columns[0], columns[1]);

            if (columns.Length == 3)
                return new EventModel(columns[0], columns[1], ParseDateTime(columns[2]), null);

            return new EventModel(columns[0], columns[1], ParseDateTime(columns[2]), ParseDateTime(columns[3]));
        }

        private void EnsureLineIsValid(string line, string[] columns)
        {
            if (columns.Length < 2)
            {
                throw new ParseLineException("Invalid line format.", line);
            }
        }

        private DateTimeOffset? ParseDateTime(string value)
        {
            try
            {
                return DateTimeOffset.Parse(value?.Trim());
            }
            catch (Exception) { }

            return null;
        }
    }
}