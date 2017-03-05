namespace SubLib.Formats
{
    using System;
    using System.Text;

    public class WebVTT : Subtitle
    {
        public override string Name => "WebVTT";

        public override string Extension => ".vtt";

        public WebVTT(string file) : base(file)
        {
        }

        public override void Load(string[] lines)
        {
        }

        public override void Save(string file, Encoding encoding)
        {

        }

        public string ToText()
        {
            throw new NotImplementedException();
        }
    }
}
