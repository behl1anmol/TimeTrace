# Memory: Platform Abstractions Architecture Pattern

## Metadata
- PatternId: MEMORY-002
- PatternVersion: 1
- Status: active
- Supersedes: none
- CreatedAt: 2026-03-31
- LastValidatedAt: 2026-03-31
- ValidationEvidence: Implemented and tested cross-platform architecture for Background Processor

## Source Context
- Triggering task: Designing cross-platform background processor for Linux and Windows
- Scope/system: TimeTrace.Platform.* projects
- Date/time: 2026-03-31

## Memory
- Key fact or decision: Cross-platform architecture uses abstractions project + platform-specific implementations
- Why it matters: Enables swap-in/swap-out of platform implementations without changing consumers

## Architecture Pattern

### Project Structure
```
TimeTrace.Platform.Abstractions/    # Interfaces + DTOs (netstandard2.0 or net9.0)
├── Screenshot/
│   ├── IScreenshotCaptureService.cs
│   ├── IActiveWindowService.cs
│   ├── ScreenshotResult.cs
│   └── ActiveWindowInfo.cs
├── Process/
│   └── IProcessInfoService.cs
├── IPlatformServiceRegistration.cs
└── Extensions/
    └── PlatformServiceCollectionExtensions.cs

TimeTrace.Platform.Linux/           # Linux implementation (net9.0)
├── Screenshot/
│   ├── LinuxActiveWindowService.cs
│   ├── LinuxScreenshotCaptureService.cs
│   └── Capture/
│       ├── ICaptureStrategy.cs
│       ├── GnomeScreenshotCapture.cs
│       ├── ScrotCapture.cs
│       └── GrimCapture.cs
└── Extensions/
    └── LinuxPlatformServiceCollectionExtensions.cs

TimeTrace.Platform.Windows/         # Windows implementation (net9.0-windows)
├── Screenshot/
│   ├── WindowsActiveWindowService.cs
│   └── WindowsScreenshotCaptureService.cs
└── Extensions/
    └── WindowsPlatformServiceCollectionExtensions.cs
```

### Platform Auto-Detection Pattern
```csharp
public interface IPlatformServiceRegistration
{
    bool IsCurrentPlatform { get; }
    void RegisterServices(IServiceCollection services);
}

// Usage in consumer
services.AddPlatformServices(); // Auto-detects via IPlatformServiceRegistration
```

### Linux Screenshot Capture Strategy
- Priority-based fallback chain
- GnomeScreenshot (100) → Grim (90) → Scrot (80)
- Each strategy checks tool availability via `which` command

## Applicability
- When to reuse: Any cross-platform .NET application needing OS-specific implementations
- Preconditions/limitations: Requires .NET 9+ for Windows TFM support

## Actionable Guidance
- Recommended future action: Follow this pattern for any new platform-specific features
- Related files/services/components:
  - `docs/architecture/BACKGROUND_PROCESSOR_ARCHITECTURE.md`
  - All `TimeTrace.Platform.*` projects
