using System;

namespace BlazorParser.Domain
{
    public class ResourceFile
    {
        public ResourceFile(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentNullException(nameof(location));

            Location = location;
        }

        public string Location { get; }
    }
}
