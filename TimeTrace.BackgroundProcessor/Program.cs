using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TimeTrace.BackgroundProcessor.Services;
using TimeTrace.BackgroundProcessor.Workers;
using TimeTrace.Platform.Abstractions.Extensions;

var builder = Host.CreateApplicationBuilder(args);

// Platform-specific services (auto-detects Linux/Windows)
builder.Services.AddPlatformServices();

// Existing library DI - uses same DB as UI
// Connection string format: "Data Source={0};Password={1}"
// Path resolves to: LocalApplicationData/timetrace.db
builder.Services.AddDatabaseContextFactory("Data Source={0};Password={1}");

// Capture services
builder.Services.AddSingleton<ICaptureConfigurationService, CaptureConfigurationService>();
builder.Services.AddSingleton<ICaptureOrchestrationService, CaptureOrchestrationService>();

// Background worker
builder.Services.AddHostedService<ScreenshotCaptureWorker>();

// Configure for systemd on Linux
if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    builder.Services.AddSystemd();
}

var host = builder.Build();

Console.WriteLine("TimeTrace Background Processor starting...");
Console.WriteLine($"Platform: {RuntimeInformation.OSDescription}");
Console.WriteLine($"Runtime: {RuntimeInformation.FrameworkDescription}");

await host.RunAsync();
