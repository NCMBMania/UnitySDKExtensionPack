using System;

namespace NCMBExtended
{
    public class TimeZoneConverter
    {
        public static DateTime UtcToLocal(DateTime utcTime)
        {
            TimeZone zone = TimeZone.CurrentTimeZone;
            TimeSpan offset = zone.GetUtcOffset(DateTime.Now);
            return utcTime + offset;
        }

        public static DateTime UtcToLocal(DateTime? nullableUtcTime)
        {
            if (nullableUtcTime == null)
            {
                return new DateTime(0);
            }
            else
            {
                DateTime utctime = (DateTime)nullableUtcTime;

                TimeZone zone = TimeZone.CurrentTimeZone;
                TimeSpan offset = zone.GetUtcOffset(DateTime.Now);
                return utctime + offset;
            }
        }
    }
}
