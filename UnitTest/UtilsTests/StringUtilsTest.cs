namespace UnitTest
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using SubLib.Utils;
    using Xunit;
    public class StringUtilsTest
    {
        [Fact]
        public void CountTagInTextTest1()
        {
            Assert.Equal(1, StringUtils.CountTagInText("foobar\r\nfoobar", '\n'));
        }

        [Fact]
        public void CountTagInTextTest2()
        {
            Assert.Equal(0, StringUtils.CountTagInText("foobar\r", '\n'));
        }

        [Fact]
        public void RemoveSsaTagsTest1()
        {
            Assert.Equal("Os mortos estão vivos", StringUtils.RemoveSsaTags("{\an5}Os mortos estão vivos"));
        }

    }
}
