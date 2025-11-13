using System.ComponentModel;

namespace Juga.Abstractions.TaskScheduling
{
    [Serializable]
    public enum CronExpressionDay
    {
        [Description("Pazar")]
        Pazar = 0,

        [Description("Pazartesi")]
        Pazartesi = 1,

        [Description("Salı")]
        Sali = 2,

        [Description("Çarşamba")]
        Carsamba = 3,

        [Description("Perşembe")]
        Persembe = 4,

        [Description("Cuma")]
        Cuma = 5,

        [Description("Cumartesi")]
        Cumartesi = 6
    }
}