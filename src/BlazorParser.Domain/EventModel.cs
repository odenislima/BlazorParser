using System;

namespace BlazorParser.Domain
{
    public class EventModel
    {
        public EventModel(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("EventName cannot be emtpy.");
            }

            if (name.Length > 32)
            {
                throw new ArgumentException("EventName is too long..");
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be emtpy.");
            }

            if (description.Length > 255)
            {
                throw new ArgumentException("description is too long..");
            }

            Name = name.Trim();
            Description = description.Trim();
        }

        public EventModel(string name, string description, DateTimeOffset? start, DateTimeOffset? end)
            : this(name, description)
        {
            Start = start;
            End = end;
        }

        public string Name { get; }
        public string Description { get; }
        public DateTimeOffset? Start { get; }
        public DateTimeOffset? End { get; }
    }
}