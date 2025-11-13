using AdminBackend.Application.Services.App;

namespace AdminBackend.Infrastructure.Services.App
{
    public class AppService : IAppService
    {
        public string? RequesterApplicationIdentifier {  get; set; }
    }
}
