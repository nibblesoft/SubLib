namespace SubLib.NET.Utils
{
    using SubLib.Models;
    using SubLib.NET.Models;
    using System;
    public static class TimeUtils
    {
        public static double MillisecondsToFrames(double milliseconds) => Math.Round(milliseconds / (TimeCode.BaseUnit / Configs.TimeConfigs.FrameRate));

        public static double FramesToMilliseconds(double frames) => Math.Round(frames * (TimeCode.BaseUnit / Configs.TimeConfigs.FrameRate));
    }
}
