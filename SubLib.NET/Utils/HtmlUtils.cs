namespace SubLib.Utils
{
    using System;

    public static class HtmlUtils
    {
        public static string RemoveHtmlTags(string text, bool alsoSsaTags = false)
        {
            if (text == null || text.Length < 3)
            {
                return text;
            }
            if (alsoSsaTags)
            {
                text = StringUtils.RemoveSsaTags(text);
            }
            int idx = text.IndexOf('<');
            while (idx >= 0)
            {
                int endIdx = text.IndexOf('>', idx + 1);
                if (endIdx < idx)
                {
                    break;
                }
                text = text.Remove(idx, endIdx - idx + 1);
                idx = text.IndexOf('>', idx + 1);
            }
            return text;
        }

        public static string RemoveEmptyTags(string input)
        {
            throw new NotImplementedException("RemoveEmptyTags");
        }

        public static string ValidataTags(string input)
        {
            throw new NotImplementedException("VaalidateTags");
        }
    }
}
