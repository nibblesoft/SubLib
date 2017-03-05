namespace SubLib.Core
{
    using System;
    using System.Text;

    public static class StringExtensions
    {
        public static bool StartsWith(this string s, char c) => s.Length > 0 && s[0] == c;

        public static bool StartsWith(this StringBuilder sb, char c) => sb.Length > 0 && sb[0] == c;

        public static bool EndsWith(this string s, char c) => s.Length > 0 && s[s.Length - 1] == c;

        public static bool EndsWith(this StringBuilder sb, char c) => sb.Length > 0 && sb[sb.Length - 1] == c;

        public static bool Contains(this string source, char value) => source.IndexOf(value) >= 0;

        public static bool Contains(this string source, char[] value) => source.IndexOfAny(value) >= 0;

        public static bool Contains(this string source, string value, StringComparison comparisonType) => source.IndexOf(value, comparisonType) >= 0;

        public static string[] SplitToLines(this string source) => source.Replace("\r\r\n", "\n").Replace("\r\n", "\n").Replace('\r', '\n').Replace('\u2028', '\n').Split('\n');

    }
}
