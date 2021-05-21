namespace BlazorParser.Domain
{
    public class ErrorData
    {
        public ErrorData(string error, string line)
        {
            Error = error;
            Line = line;
        }

        public ErrorData(string error):this(error, string.Empty)
        {
            Error = error;
        }

        public string Error { get; }
        public string Line { get; }
    }
}
