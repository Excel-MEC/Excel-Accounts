using System;
using Xunit;

namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            bool Value = true;
            Assert.True(Value, "Value Should be True");
        }
    }
}
