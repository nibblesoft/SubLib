namespace SubLib.Formats
{
    using Enums;
    using SubLib.Core;
    using SubLib.Models;
    using SubLib.Utils;
    using System;
    using System.Text;
    using System.Text.RegularExpressions;

    public class Subrip : Subtitle
    {
        private static readonly Regex RegexTimeCodes = new Regex(@"^-?\d+:-?\d+:-?\d+[:,]-?\d+\s*-->\s*-?\d+:-?\d+:-?\d+[:,]-?\d+$", RegexOptions.Compiled);

        public override string Name => "Subrip";

        public override string Extension => ".srt";

        public Subrip(string file) : base(file)
        {
        }

        public override void Load(string[] lines)
        {
            var expectingLine = ExpectingLineSubrip.Number;
            var paragraph = new Paragraph();
            Paragraph preParagraph = null;

            int countLines = lines.Length;
            for (int i = 0; i < countLines; i++)
            {
                string line = lines[i];

                string lineNext = null;
                if (i + 1 < countLines)
                {
                    lineNext = lines[i + 1];
                }

                switch (expectingLine)
                {
                    case ExpectingLineSubrip.Number:
                        line = line.Trim();
                        if (int.TryParse(line, out int lineNumber))
                        {
                            paragraph.Number = lineNumber;
                            expectingLine = ExpectingLineSubrip.TimeCodes;
                        }
                        else
                        {
                            _errorCount++;
                            // If next line contains time code then parse it.
                            if (lineNext?.Length > 0 && RegexTimeCodes.IsMatch(lineNext)) // TODO: Filter by regex minimum match length
                            {
                                expectingLine = ExpectingLineSubrip.TimeCodes;
                            }
                            else
                            {
                                // If fails matching time code, keep looking for paragraph number.
                                expectingLine = ExpectingLineSubrip.Number;
                            }
                        }
                        break;

                    case ExpectingLineSubrip.TimeCodes:
                        //line = line.Trim();
                        //Match matchTimestamp = null;

                        //// Only try to match time-code if line has high probability of matching!
                        //if (line.Length > 20)
                        //{
                        //    matchTimestamp = RegexTimeCodes.Match(line);
                        //}
                        //if (matchTimestamp?.Success == true)
                        //{
                        //    // 00:01:11,680 --> 00:01:13,159
                        //    string timeStamp = matchTimestamp.Value.Replace(" --> ", ":");
                        //    timeStamp = timeStamp.Replace(" --> ", ":");
                        //}
                        if (TryReadTimeCodesLine(line, paragraph))
                        {
                            paragraph.Text = string.Empty;
                            expectingLine = ExpectingLineSubrip.Text;
                        }
                        else if (!string.IsNullOrWhiteSpace(line))
                        {
                            _errorCount++;
                            expectingLine = ExpectingLineSubrip.Number; // lets go to next paragraph
                        }
                        break;

                    case ExpectingLineSubrip.Text:
                        if (!string.IsNullOrWhiteSpace(line) || IsText(lineNext))
                        {
                            //if (_isWsrt && !string.IsNullOrEmpty(line))
                            //{
                            //    for (int k = 30; k < 40; k++)
                            //    {
                            //        line = line.Replace("<" + k + ">", "<i>");
                            //        line = line.Replace("</" + k + ">", "</i>");
                            //    }
                            //}

                            if (paragraph.Text.Length > 0)
                                paragraph.Text += Environment.NewLine;
                            paragraph.Text += StringUtils.RemoveBadChars(line).TrimEnd().Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
                        }
                        else if (string.IsNullOrEmpty(line) && string.IsNullOrEmpty(paragraph.Text))
                        {
                            paragraph.Text = string.Empty;
                            if (!string.IsNullOrEmpty(lineNext) && (StringUtils.IsInteger(lineNext) || RegexTimeCodes.IsMatch(lineNext)))
                            {
                                Paragraphs.Add(paragraph);
                                preParagraph = paragraph; ;
                                paragraph = new Paragraph();
                                expectingLine = ExpectingLineSubrip.Number;
                            }
                        }
                        else
                        {
                            Paragraphs.Add(paragraph);
                            preParagraph = paragraph;
                            paragraph = new Paragraph();
                            expectingLine = ExpectingLineSubrip.Number;
                        }
                        break;
                }
            }

        }

        private static bool IsText(string text)
        {
            return !(string.IsNullOrWhiteSpace(text) || StringUtils.IsInteger(text) || RegexTimeCodes.IsMatch(text));
        }

        private static bool TryReadTimeCodesLine(string timeStamp, Paragraph paragraph)
        {
            timeStamp = timeStamp.Replace('،', ',');
            timeStamp = timeStamp.Replace('', ',');
            timeStamp = timeStamp.Replace('¡', ',');

            const string defaultSeparator = " --> ";
            // Fix some badly formatted separator sequences - anything can happen if you manually edit ;)
            timeStamp = timeStamp.Replace(" -> ", defaultSeparator); // I've seen this
            timeStamp = timeStamp.Replace(" - > ", defaultSeparator);
            timeStamp = timeStamp.Replace(" ->> ", defaultSeparator);
            timeStamp = timeStamp.Replace(" -- > ", defaultSeparator);
            timeStamp = timeStamp.Replace(" - -> ", defaultSeparator);
            timeStamp = timeStamp.Replace(" -->> ", defaultSeparator);
            timeStamp = timeStamp.Replace(" ---> ", defaultSeparator);

            // Removed stuff after timecodes - like subtitle position
            //  - example of position info: 00:02:26,407 --> 00:02:31,356  X1:100 X2:100 Y1:100 Y2:100
            if (timeStamp.Length > 30 && timeStamp[29] == ' ')
                timeStamp = timeStamp.Substring(0, 29);

            // removes all extra spaces
            timeStamp = timeStamp.Replace(" ", string.Empty).Replace("-->", defaultSeparator).Trim();

            // Fix a few more cases of wrong time codes, seen this: 00.00.02,000 --> 00.00.04,000
            timeStamp = timeStamp.Replace('.', ':');
            if (timeStamp.Length >= 29 && (timeStamp[8] == ':' || timeStamp[8] == ';'))
                timeStamp = timeStamp.Substring(0, 8) + ',' + timeStamp.Substring(8 + 1);
            if (timeStamp.Length >= 29 && timeStamp.Length <= 30 && (timeStamp[25] == ':' || timeStamp[25] == ';'))
                timeStamp = timeStamp.Substring(0, 25) + ',' + timeStamp.Substring(25 + 1);

            if (RegexTimeCodes.IsMatch(timeStamp))
            {
                string[] parts = timeStamp.Replace("-->", ":").Replace(" ", string.Empty).Split(':', ',');
                try
                {
                    int startHours = int.Parse(parts[0]);
                    int startMinutes = int.Parse(parts[1]);
                    int startSeconds = int.Parse(parts[2]);
                    int startMilliseconds = int.Parse(parts[3]);
                    int endHours = int.Parse(parts[4]);
                    int endMinutes = int.Parse(parts[5]);
                    int endSeconds = int.Parse(parts[6]);
                    int endMilliseconds = int.Parse(parts[7]);

                    //if (_isMsFrames && (parts[3].Length != 2 || startMilliseconds > 30 || parts[7].Length != 2 || endMilliseconds > 30))
                    //{
                    //    _isMsFrames = false;
                    //}

                    paragraph.StartTime = new TimeCode(startHours, startMinutes, startSeconds, startMilliseconds);
                    if (parts[0].StartsWith('-') && paragraph.StartTime.TotalMilliseconds > 0)
                        paragraph.StartTime.TotalMilliseconds = paragraph.StartTime.TotalMilliseconds * -1;

                    paragraph.EndTime = new TimeCode(endHours, endMinutes, endSeconds, endMilliseconds);
                    if (parts[4].StartsWith('-') && paragraph.EndTime.TotalMilliseconds > 0)
                        paragraph.EndTime.TotalMilliseconds = paragraph.EndTime.TotalMilliseconds * -1;

                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public void Save(string file) => Save(file, Encoding.UTF8);

        public override void Save(string file, Encoding encoding)
        {
        }
    }
}
