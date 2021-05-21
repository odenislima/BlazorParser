namespace BlazorParser.Domain.Interfaces
{
    public interface IEventDataParser
    {
        EventModel Parse(string line);
    }
}