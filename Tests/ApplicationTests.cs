using System;
using Xunit;

namespace Tests
{
    public class ApplicationTests
    {
        [Fact]
        public void AlwaysPassTest()
        {
            bool Value = true;
            Assert.True(Value, "Value Should be True");
        }
    }
}
