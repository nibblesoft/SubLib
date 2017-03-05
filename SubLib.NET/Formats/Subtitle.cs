namespace SubLib.Formats
{
    using SubLib.Core;
    using SubLib.Models;
    using SubLib.Utils;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Subtitle base class.
    /// </summary>
    public abstract class Subtitle
    {
        protected int _errorCount;
        protected List<Paragraph> _paragraphs;

        /// <summary>
        /// Subtitle base constructor.
        /// </summary>
        /// <param name="file">File to be used to load subtitle.</param>
        public Subtitle(string file)
        {
            File = file;
            _paragraphs = new List<Paragraph>();
            IsFrameBased = false;
            Load(file);
        }

        /// <summary>
        /// Load subtitle using the given file.
        /// </summary>
        /// <param name="file">Subtitle file</param>
        public virtual void Load(string file)
        {
            string[] lines;
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sr = new StreamReader(fs, Encoding.UTF8))
            {
                lines = sr.ReadToEnd().SplitToLines();
            }
            if (lines?.Length > 0)
            {
                Load(lines);
            }
        }

        /// <summary>
        /// Load subtitle using the given string array.
        /// </summary>
        /// <param name="lines">Subtitle lines.</param>
        public abstract void Load(string[] lines);

        /// <summary>
        /// Remove formatting from specific subtitle format.
        /// </summary>
        protected virtual void RemoveTextFormatting()
        {
            foreach (var p in Paragraphs)
            {
                p.Text = HtmlUtils.RemoveHtmlTags(p.Text);
            }
        }

        /// <summary>
        /// Save subtitle to a file.
        /// </summary>
        /// <param name="file">File to save subtitle</param>
        /// <param name="encoding">Encoding to be used when saving the subtitle.</param>
        public abstract void Save(string file, Encoding encoding);

        /// <summary>
        /// Renumber subtitle.
        /// </summary>
        /// <param name="from">Number to start renumber from.</param>
        public void Renumber(int from)
        {
            from = Math.Max(1, from);
            foreach (var paragraph in Paragraphs)
            {
                paragraph.Number = from++;
            }
        }

        /// <summary>
        /// Removies any paragraph which contains only empty lines.
        /// </summary>
        /// <returns>Total number of paragraph removed.</returns>
        public int RemoveEmptyLines()
        {
            int count = _paragraphs.Count;
            if (count > 0)
            {
                for (int i = count - 1; i >= 0; i--)
                {
                    Paragraph p = Paragraphs[i];
                    // TODO: if text is empty and removev
                    _paragraphs.RemoveAt(i);
                }
            }
            return count - _paragraphs.Count;
        }

        /// <summary>
        /// Format name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Subtitle extension.
        /// </summary>
        public abstract string Extension { get; }

        /// <summary>
        /// File name.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Format extension.
        /// </summary>

        /// <summary>
        /// Errors count that occour while parsing.
        /// </summary>
        public int Errors { get => _errorCount; }

        /// <summary>
        /// Gets list of paragraphs.
        /// </summary>
        public IList<Paragraph> Paragraphs { get => _paragraphs; }

        /// <summary>
        /// Return true if subtitle format uses frames instead of times
        /// </summary>
        public virtual bool IsFrameBased { get; set; }
    }

}