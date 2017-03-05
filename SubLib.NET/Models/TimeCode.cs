using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubLib.Models
{
    public class TimeCode
    {
        public const double BaseUnit = 1000.00;
        protected double _totalMilliseconds;

        public TimeSpan TimeSpan
        {
            get
            {
                return TimeSpan.FromMilliseconds(_totalMilliseconds);
            }
        }

        /// <summary>
        /// Default constructor to initialize time code.
        /// </summary>
        public TimeCode(double milliseconds = 0d)
        {
            _totalMilliseconds = milliseconds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        /// <param name="milliseconds"></param>
        public TimeCode(int hours, int minutes, int seconds, int milliseconds)
        {
            _totalMilliseconds = hours * 60 * 60 * BaseUnit + minutes * 60 * BaseUnit + seconds * BaseUnit + milliseconds;
        }

        public int Hours
        {
            get => (int)(_totalMilliseconds - (_totalMilliseconds % (BaseUnit * Math.Pow(60, 3))));
            set => _totalMilliseconds = value;
        }

        public int Minutes { get; set; }

        public int Seconds { get; set; }

        public int Milliseconds { get; set; }

        public double TotalMilliseconds
        {
            get => _totalMilliseconds;
            set => _totalMilliseconds = value;
        }

        public double TotalSeconds
        {
            get => _totalMilliseconds / BaseUnit;
            set => _totalMilliseconds = value * BaseUnit;
        }

        public override string ToString() => ToShortString();

        public string ToShortString(bool localize = false)
        {
            var ts = TimeSpan;
            string decimalSeparator = localize ? CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator : ",";
            string s;
            if (ts.Minutes == 0 && ts.Hours == 0 && ts.Days == 0)
                s = string.Format("{0:0}{1}{2:000}", ts.Seconds, decimalSeparator, ts.Milliseconds);
            else if (ts.Hours == 0 && ts.Days == 0)
                s = string.Format("{0:0}:{1:00}{2}{3:000}", ts.Minutes, ts.Seconds, decimalSeparator, ts.Milliseconds);
            else
                s = string.Format("{0:0}:{1:00}:{2:00}{3}{4:000}", ts.Hours + ts.Days * 24, ts.Minutes, ts.Seconds, decimalSeparator, ts.Milliseconds);

            if (TotalMilliseconds >= 0)
                return s;
            return "-" + s.Replace("-", string.Empty);
        }
    }
}
