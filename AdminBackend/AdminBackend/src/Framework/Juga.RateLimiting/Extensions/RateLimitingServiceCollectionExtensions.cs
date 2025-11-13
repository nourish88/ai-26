using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Juga.RateLimiting.Extensions;

public static class RateLimitingServiceCollectionExtensions
{
    public static IServiceCollection TryAddRateLimitingServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        if (!IsRateLimitingEnabled(configuration)) return services;

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            if (!TryGetRateLimitSettings(configuration, out var settings)) return;
            ConfigureFixedWindowLimiters(options, configuration, settings);
            ConfigureSlidingWindowLimiters(options, configuration, settings);
            ConfigureTokenBucketLimiter(options, configuration, settings);
            ConfigureConcurrencyLimiter(options, settings);
        });

        return services;
    }

    private static bool IsRateLimitingEnabled(IConfiguration configuration)
    {
        return configuration.GetValue<bool>("Juga:RateLimiting:Enable");
    }

    private static bool TryGetRateLimitSettings(IConfiguration configuration, out RateLimitSettings settings)
    {
        settings = new RateLimitSettings
        {
            PermitLimit = configuration.GetValue<int?>("Juga:RateLimiting:PermitLimit"),
            QueueLimit = configuration.GetValue<int?>("Juga:RateLimiting:QueueLimit"),
            Window = configuration.GetValue<int?>("Juga:RateLimiting:Window"),
            SegmentPerWindow = configuration.GetValue<int?>("Juga:RateLimiting:SegmentPerWindow"),
            TokenLimit = configuration.GetValue<int?>("Juga:RateLimiting:TokenLimit"),
            TokensPerPeriod = configuration.GetValue<int?>("Juga:RateLimiting:TokensPerPeriod"),
            ReplenishmentPeriod = configuration.GetValue<int?>("Juga:RateLimiting:ReplenishmentPeriod")
        };

        return settings is { PermitLimit: not null, QueueLimit: not null, Window: not null };
    }

    private static void ConfigureFixedWindowLimiters(RateLimiterOptions options, IConfiguration configuration,
        RateLimitSettings settings)
    {
        options.AddPolicy("fixed-by-user",
            context => GenerateFixedWindowLimiter(configuration, context.User.Identity?.Name, settings));
        options.AddPolicy("fixed-by-ip",
            context => GenerateFixedWindowLimiter(configuration, context.Connection.RemoteIpAddress?.ToString(),
                settings));
        options.AddPolicy("fixed-by-x-forwarded-ip",
            context => GenerateFixedWindowLimiter(configuration, context.Request.Headers["X-Forwarded-For"].ToString(),
                settings));

        options.AddFixedWindowLimiter("fixed-all",
            limiterOptions => { ConfigureFixedWindowLimiterOptions(limiterOptions, configuration, settings); });
    }

    private static void ConfigureSlidingWindowLimiters(RateLimiterOptions options, IConfiguration configuration,
        RateLimitSettings settings)
    {
        if (settings.SegmentPerWindow.HasValue)
        {
            options.AddPolicy("sliding-by-user",
                context => GenerateSlidingWindowLimiter(configuration, context.User.Identity?.Name, settings));
            options.AddPolicy("sliding-by-ip",
                context => GenerateSlidingWindowLimiter(configuration, context.Connection.RemoteIpAddress?.ToString(),
                    settings));
            options.AddPolicy("sliding-by-x-forwarded-ip",
                context => GenerateSlidingWindowLimiter(configuration,
                    context.Request.Headers["X-Forwarded-For"].ToString(), settings));

            options.AddSlidingWindowLimiter("sliding-all",
                limiterOptions => { ConfigureSlidingWindowLimiterOptions(limiterOptions, configuration, settings); });
        }
    }

    private static void ConfigureTokenBucketLimiter(RateLimiterOptions options, IConfiguration configuration,
        RateLimitSettings settings)
    {
        if (settings is { TokenLimit: not null, TokensPerPeriod: not null, ReplenishmentPeriod: not null })
            options.AddTokenBucketLimiter("token",
                limiterOptions => { ConfigureTokenBucketLimiterOptions(limiterOptions, configuration, settings); });
    }

    private static void ConfigureConcurrencyLimiter(RateLimiterOptions options, RateLimitSettings settings)
    {
        options.AddConcurrencyLimiter("concurrency",
            limiterOptions => { ConfigureConcurrencyLimiterOptions(limiterOptions, settings); });
    }

    private static RateLimitPartition<string?> GenerateFixedWindowLimiter(IConfiguration configuration,
        string? partitionKey, RateLimitSettings settings)
    {
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey,
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = settings.PermitLimit.Value,
                Window = GetTimeSpan(configuration, settings.Window.Value),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = settings.QueueLimit.Value
            }
        );
    }

    private static RateLimitPartition<string?> GenerateSlidingWindowLimiter(IConfiguration configuration,
        string? partitionKey, RateLimitSettings settings)
    {
        return RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey,
            _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = settings.PermitLimit.Value,
                Window = GetTimeSpan(configuration, settings.Window.Value),
                SegmentsPerWindow = settings.SegmentPerWindow.Value,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = settings.QueueLimit.Value
            });
    }

    private static void ConfigureFixedWindowLimiterOptions(FixedWindowRateLimiterOptions options,
        IConfiguration configuration, RateLimitSettings settings)
    {
        if (settings.PermitLimit != null) options.PermitLimit = settings.PermitLimit.Value;
        if (settings.Window != null) options.Window = GetTimeSpan(configuration, settings.Window.Value);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        if (settings.QueueLimit != null) options.QueueLimit = settings.QueueLimit.Value;
    }

    private static void ConfigureSlidingWindowLimiterOptions(SlidingWindowRateLimiterOptions options,
        IConfiguration configuration, RateLimitSettings settings)
    {
        if (settings.PermitLimit != null) options.PermitLimit = settings.PermitLimit.Value;
        if (settings.Window != null) options.Window = GetTimeSpan(configuration, settings.Window.Value);
        if (settings.SegmentPerWindow != null) options.SegmentsPerWindow = settings.SegmentPerWindow.Value;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        if (settings.QueueLimit != null) options.QueueLimit = settings.QueueLimit.Value;
    }

    private static void ConfigureTokenBucketLimiterOptions(TokenBucketRateLimiterOptions options,
        IConfiguration configuration, RateLimitSettings settings)
    {
        if (settings.TokenLimit != null) options.TokenLimit = settings.TokenLimit.Value;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        if (settings.QueueLimit != null) options.QueueLimit = settings.QueueLimit.Value;
        if (settings.ReplenishmentPeriod != null)
            options.ReplenishmentPeriod = GetTimeSpan(configuration, settings.ReplenishmentPeriod.Value);
        if (settings.TokensPerPeriod != null) options.TokensPerPeriod = settings.TokensPerPeriod.Value;
        options.AutoReplenishment = true;
    }

    private static void ConfigureConcurrencyLimiterOptions(ConcurrencyLimiterOptions options,
        RateLimitSettings settings)
    {
        options.PermitLimit = settings.PermitLimit.Value;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = settings.QueueLimit.Value;
    }

    private static TimeSpan GetTimeSpan(IConfiguration configuration, int value)
    {
        var timePeriod = configuration.GetValue<string?>("Juga:RateLimiting:TimePeriod") ?? "Day";
        return timePeriod switch
        {
            "Second" => TimeSpan.FromSeconds(value),
            "Minute" => TimeSpan.FromMinutes(value),
            _ => TimeSpan.FromDays(value)
        };
    }
}

public class RateLimitSettings
{
    public int? PermitLimit { get; set; }
    public int? QueueLimit { get; set; }
    public int? Window { get; set; }
    public int? SegmentPerWindow { get; set; }
    public int? TokenLimit { get; set; }
    public int? TokensPerPeriod { get; set; }
    public int? ReplenishmentPeriod { get; set; }
}