namespace AdminBackend.Application.Settings;

public class StillProcessingJobWorkerSettings
{
    public const string SectionName = "StillProcessingJobWorker";

    public double MaxProcessIntervalInMins { get; set; } = 30.0;
    public double CheckIntervalInMins { get; set; } = 5.0;
}