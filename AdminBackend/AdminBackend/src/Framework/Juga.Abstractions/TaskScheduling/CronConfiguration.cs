namespace Juga.Abstractions.TaskScheduling;

[Serializable]
public class CronConfiguration
{
    public const string EXACT_TIME_WORKS_DAILY = "ExactTimeWorksDaily";
    public const string EXACT_DAY_TIME_WORKS_WEEKLY = "ExactDayTimeWorksWeekly";
    public const string EXACT_DAY_TIME_WORKS_MONTHLY = "ExactDayTimeWorksMonthly";
    public const string MINUTE_INTERVAL = "MinuteInterval";
    public const string HOUR_INTERVAL = "HourInterval";
    public const string DAY_INTERVAL = "DayInterval";

    /// <summary>
    /// Cron tipi. ExactTimeWorksDaily, ExactDayTimeWorksWeekly, ExactDayTimeWorksMonthly, MinuteInterval, HourInterval, DayInterval
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Cronun dakika bilgisi. Field 0 in "{0}/{1} {2}/{3} {4}/{5} {6} {7}"
    /// </summary>
    public int? Minute { get; set; }

    /// <summary>
    /// Cronun dakika aralığı bilgisi. Field 1 in "{0}/{1} {2}/{3} {4}/{5} {6} {7}"
    /// </summary>
    public int? MinuteInterval { get; set; }

    /// <summary>
    /// Cronun saat bilgisi. Field 2 in "{0}/{1} {2}/{3} {4}/{5} {6} {7}"
    /// </summary>
    public int? Hour { get; set; }

    /// <summary>
    /// Cronun saat aralığı bilgisi. Field 3 in "{0}/{1} {2}/{3} {4}/{5} {6} {7}"
    /// </summary>
    public int? HourInterval { get; set; }

    /// <summary>
    /// Cronun bilgisi. Field 4 in "{0}/{1} {2}/{3} {4}/{5} {6} {7}"
    /// </summary>
    public List<int> Days { get; set; }

    /// <summary>
    /// Cronun gün aralığı bilgisi. Field 5 in "{0}/{1} {2}/{3} {4}/{5} {6} {7}"
    /// </summary>
    public int? DayInterval { get; set; }

    /// <summary>
    /// Cronun haftanın günleri bilgisi. Field 7 in "{0}/{1} {2}/{3} {4}/{5} {6} {7}"
    /// </summary>
    public List<CronExpressionDay> Weekdays { get; set; }
}