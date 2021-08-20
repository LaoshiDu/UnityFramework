using System;

public class MyTime
{
    private static DateTime _dateStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
    /// <summary>
    /// ��ȡ��ǰʱ���
    /// </summary>
    /// <returns></returns>
    public static long GetTimeStamp()
    {
        return Convert.ToInt64((DateTime.Now - _dateStart).TotalSeconds);
    }
    /// <summary>
    /// ��ȡdtʱ���
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static long GetTimeStamp(DateTime dt)
    {
        return Convert.ToInt64((dt - _dateStart).TotalSeconds);
    }

    private static TimeSpan _timtSpan = default;
    /// <summary>
    /// ��ȡָ����ʾ��ʽ��ʱ���ȱ��
    /// "{0:00}:{1:00}" ��/�� �ۼӵ�������
    /// "{0:00}:{1:00}:{2:00}" ʱ/��/�� �ۼӵ�Сʱ��
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static string GetTimeSpanWithFormat(DateTime startTime, DateTime endTime, string format = "{0:00}:{1:00}")
    {
        _timtSpan = endTime - startTime;
        if (format.Equals("{0:00}:{1:00}"))
        {
            return string.Format("{0:00}:{1:00}", _timtSpan.Minutes + _timtSpan.Days * 24 * 60, _timtSpan.Seconds);
        }
        if (format.Equals("{0:00}:{1:00}:{2:00}"))
        {
            return string.Format("{0:00}:{1:00}:{2:00}", _timtSpan.Hours + _timtSpan.Days * 24, _timtSpan.Minutes, _timtSpan.Seconds);
        }
        return string.Format("{0:00}:{1:00}", _timtSpan.Minutes, _timtSpan.Seconds);
    }

    /// <summary>
    /// ��ȡָ����ʾ��ʽ��ʱ���ȱ��
    /// </summary>
    /// <param name="span"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static string GetTimeSpanWithFormat(TimeSpan span, string format = "{0:00}:{1:00}")
    {
        if (format.Equals("{0:00}:{1:00}"))
        {
            return string.Format("{0:00}:{1:00}", span.Minutes + span.Days * 24 * 60 + span.Hours * 60, span.Seconds);
        }
        if (format.Equals("{0:00}:{1:00}:{2:00}"))
        {
            return string.Format("{0:00}:{1:00}:{2:00}", span.Hours + span.Days * 24, span.Minutes, span.Seconds);
        }
        return string.Format("{0:00}:{1:00}", span.Minutes + span.Days * 24 * 60 + span.Hours * 60, span.Seconds);
    }

    private static DateTime _dateStartUTC = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    /// <summary>
    /// ��ȡ��ǰUTCʱ���
    /// </summary>
    /// <returns></returns>
    public static long GetUTCTimeStamp()
    {
        return Convert.ToInt64((DateTime.UtcNow - _dateStartUTC).TotalSeconds);
    }

    /// <summary>
    /// ��ȡUTCʱ���
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static long GetUTCTimeStamp(DateTime dt)
    {
        return Convert.ToInt64((dt - _dateStartUTC).TotalSeconds);
    }

    /// <summary>
    /// UTCʱ���ת��������
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <returns></returns>
    public static DateTime GetUTCDateTime(long timeStamp)
    {
        long ticks = timeStamp * 10000000L;
        TimeSpan value = new TimeSpan(ticks);
        return _dateStartUTC.Add(value);
    }
}