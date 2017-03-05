namespace SubLib.Models
{
    using SubLib.Utils;
    using System;

    public class Paragraph
    {
        public Paragraph() : this(new TimeCode(), new TimeCode(), string.Empty)
        {
        }

        public Paragraph(double startMilliSeconds, double endMilliSeconds, string text)
            : this(new TimeCode(startMilliSeconds), new TimeCode(endMilliSeconds), text)
        {

        }

        public Paragraph(TimeCode startTime, TimeCode endTime, string text)
        {
            ID = Guid.NewGuid().ToString();
            StartTime = startTime;
            EndTime = endTime;
            Text = text;
        }

        public string ID { get; }

        public int Number { get; set; }

        public int NumberOfLines
        {
            get
            {
                if (Text.Length == 0)
                {
                    return 0;
                }
                return StringUtils.CountTagInText(Text, '\n') + 1;
            }
        }

        public TimeCode StartTime { get; set; }

        public TimeCode EndTime { get; set; }

        public string Text { get; set; }
    }
}
