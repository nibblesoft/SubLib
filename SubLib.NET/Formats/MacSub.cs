namespace SubLib.Formats
{
    using SubLib.Models;
    using SubLib.NET.Utils;
    using SubLib.Utils;
    using System;
    using System.Text;

    public class MacSub : Subtitle
    {
        private enum Expecting
        {
            StartFrame,
            Text,
            EndFrame
        }

        public MacSub(string file) : base(file)
        {
        }

        public override string Name => "MacSub";

        public override string Extension => ".txt";

        public override bool IsFrameBased => true;

        public override void Save(string file, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public string ToText()
        {
            // Startframe
            // Text
            // Endframe.
            const string writeFormat = "/{0}{3}{1}{3}/{2}{3}";
            var sb = new StringBuilder();
            foreach (var p in Paragraphs)
            {
                sb.AppendFormat(writeFormat, TimeUtils.MillisecondsToFrames(p.StartTime.TotalMilliseconds), HtmlUtils.RemoveHtmlTags(p.Text, true),
                    TimeUtils.MillisecondsToFrames(p.EndTime.TotalMilliseconds), Environment.NewLine);
            }
            return sb.ToString();
        }

        public override void Load(string[] lines)
        {
            var expecting = Expecting.StartFrame;
            Paragraphs.Clear();
            _errorCount = 0;
            char[] trimChar = { '/' };
            var p = new Paragraph();
            for (int i = 0, lineNumber = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                string nextLine = null;
                if (i + 1 < lines.Length)
                {
                    nextLine = lines[i + 1].Trim();
                }
                try
                {
                    switch (expecting)
                    {
                        case Expecting.StartFrame:
                            if (ContainsOnlyNumber(line))
                            {
                                double ms = TimeUtils.FramesToMilliseconds(int.Parse(line.TrimStart(trimChar)));
                                p.StartTime = new TimeCode(ms);
                                expecting = Expecting.Text;
                            }
                            else
                            {
                                _errorCount++;
                            }
                            break;

                        case Expecting.Text:
                            line = HtmlUtils.RemoveHtmlTags(line, true);
                            p.Text += string.IsNullOrEmpty(p.Text) ? line : Environment.NewLine + line;
                            // Next reading is going to be endframe if next line starts with (/) delimeter which indicates frame start.
                            if ((nextLine == null) || (nextLine.Length > 0 && nextLine[0] == '/'))
                            {
                                expecting = Expecting.EndFrame;
                                p.Number = lineNumber++;
                            }
                            break;

                        case Expecting.EndFrame:
                            if (ContainsOnlyNumber(line))
                            {
                                double ms = TimeUtils.FramesToMilliseconds(int.Parse(line.TrimStart(trimChar)));
                                p.EndTime = new TimeCode(ms);
                                Paragraphs.Add(p);
                                // Prepare for next reading.
                                p = new Paragraph();
                                expecting = Expecting.StartFrame;
                            }
                            else
                            {
                                _errorCount++;
                            }
                            break;
                    }
                }
                catch
                {
                    _errorCount++;
                }
            }
        }

        private static bool ContainsOnlyNumber(string input)
        {
            int len = input.Length;
            // 10 = length of int.MaxValue (2147483647); +1 if starts with '/'
            if (len == 0 || len > 11 || input[0] != '/')
            {
                return false;
            }
            int halfLen = len / 2;
            for (int i = 1; i <= halfLen; i++) // /10.0 (Do not parse double)
            {
                if (!(CharUtils.IsDigit(input[i]) && CharUtils.IsDigit(input[len - i])))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
