﻿namespace RapidTransit.Core.Extensions
{
    using System;
    using System.Text;


    public static class TimeSpanExtensions
    {
        static readonly TimeSpan _day = TimeSpan.FromDays(1);
        static readonly TimeSpan _hour = TimeSpan.FromHours(1);
        static readonly TimeSpan _month = TimeSpan.FromDays(30);
        static readonly TimeSpan _year = TimeSpan.FromDays(365);


        public static string ToFriendlyString(this TimeSpan ts)
        {
            if (ts.Equals(_month))
                return "1M";
            if (ts.Equals(_year))
                return "1y";
            if (ts.Equals(_day))
                return "1d";
            if (ts.Equals(_hour))
                return "1h";

            var sb = new StringBuilder();

            int years = ts.Days / 365;
            int months = (ts.Days % 365) / 30;
            int weeks = ((ts.Days % 365) % 30) / 7;
            int days = (((ts.Days % 365) % 30) % 7);

            if (years > 0)
                sb.Append(years).Append("y");

            if (months > 0)
                sb.Append(months).Append("M");

            if (weeks > 0)
                sb.Append(weeks).Append("w");

            if (days > 0)
                sb.Append(days).Append("d");

            if (ts.Hours > 0)
                sb.Append(ts.Hours).Append("h");
            if (ts.Minutes > 0)
                sb.Append(ts.Minutes).Append("m");
            if (ts.Seconds > 0)
                sb.Append(ts.Seconds).Append("s");
            if (ts.Milliseconds > 0)
                sb.Append(ts.Milliseconds).Append("ms");

            if (ts.Ticks == 0)
                sb.Append("-0-");
            else if (sb.Length == 0)
                sb.Append(ts.Ticks * 100).Append("ns");

            return sb.ToString();
        }
    }
}