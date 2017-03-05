namespace SubLib.NET.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class Configs
    {
        static Configs()
        {
            // todo: init timeConfigs;
            TimeConfigs = new TimeConfigs() { FrameRate = 25 };
        }

        public static TimeConfigs TimeConfigs { get; }
    }
}
