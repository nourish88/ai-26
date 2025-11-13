using AdminBackend.Application.Services.FileStorage;
using AdminBackend.Application.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace AdminBackend.Infrastructure.Services.FileStorage;

public static class FileStorageExtensions
{
    public static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FileStorageSettings>(configuration.GetSection(FileStorageSettings.SectionName));
        
        services.AddMinio(configure =>
        {
            configure
                .WithEndpoint(configuration.GetValue<string>("Minio:Host"))
                .WithCredentials(configuration.GetValue<string>("Minio:AccessKey"),
                    configuration.GetValue<string>("Minio:SecretKey"))
                .WithSSL(configuration.GetValue<bool>("Minio:Secure"));
        });
        
        services.AddSingleton<IFileStorage, BlobStorage>();

        return services;
    }
}