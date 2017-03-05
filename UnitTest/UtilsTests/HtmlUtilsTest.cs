namespace UnitTest
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xunit;
    using SubLib.Utils;

    public class HtmlUtilsTest
    {
        [Fact]
        public void RemoveHtmlTagsTest()
        {
            string textString = "<i>Hello World!</i>";
            string expected = "Hello World!";
            Assert.Equal(expected, HtmlUtils.RemoveHtmlTags(textString));
        }
    }
}
