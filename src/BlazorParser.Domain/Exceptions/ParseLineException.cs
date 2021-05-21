using System;

namespace BlazorParser.Domain.Exceptions
{
    public class ParseLineException : Exception
    {
        public ErrorData ErrorData { get; }

        public ParseLineException(string error, string line) : base(error)
        {
            ErrorData = new ErrorData(error, line);
        }

    }
}
