namespace AdminBackend.Application.Services.DataManager;

public interface IDataManagerService
{
    Task SendJobRequest(JobRequest request, CancellationToken cancellationToken);
}