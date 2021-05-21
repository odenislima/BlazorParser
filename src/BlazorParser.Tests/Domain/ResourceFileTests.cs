using BlazorParser.Domain;
using System;
using Xunit;

namespace BlazorParser.Tests.Domain
{
    public class ResourceFileTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void It_should_throw_ArgumentNullException_when_Location_is_empty_or_null(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new ResourceFile(value));
        }
    }
}
