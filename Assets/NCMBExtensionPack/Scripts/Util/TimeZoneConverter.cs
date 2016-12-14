using System;

public static class TimeZoneConverter
{
    public static void UtcToLocal(this DateTime self)
    {
        TimeZone zone = TimeZone.CurrentTimeZone;
        TimeSpan offset = zone.GetUtcOffset(DateTime.Now);
        self += offset;
    }
}
