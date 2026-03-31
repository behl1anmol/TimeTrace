# TimeTrace Background Processor Architecture

> **Status:** Draft for Review  
> **Target:** .NET 9  
> **Primary Focus:** Linux Implementation (Windows to follow)  
> **Date:** March 2026

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Current State Analysis](#2-current-state-analysis)
3. [Proposed Architecture](#3-proposed-architecture)
4. [Project Structure](#4-project-structure)
5. [Interface Design](#5-interface-design)
6. [Linux Implementation Details](#6-linux-implementation-details)
7. [Configuration Integration](#7-configuration-integration)
8. [Dependency Injection Strategy](#8-dependency-injection-strategy)
9. [Testing Strategy](#9-testing-strategy)
10. [Implementation Plan](#10-implementation-plan)
11. [Open Questions & TODOs](#11-open-questions--todos)

---

## 1. Executive Summary

### Problem Statement

TimeTrace needs a **cross-platform background processor** that:
- Captures screenshots of the currently active application window
- Saves screenshots to a user-accessible temp folder
- Persists metadata to SQLite via the existing `timetrace.library`
- Runs automatically on user login as a system background service
- Supports both X11 and Wayland display servers on Linux

### Proposed Solution

A **layered abstraction architecture** using:

| Layer | Project | Purpose |
|-------|---------|---------|
| **Host** | `TimeTrace.BackgroundProcessor` | Worker Service host, orchestration |
| **Abstractions** | `TimeTrace.Platform.Abstractions` | Platform-agnostic interfaces |
| **Linux Impl** | `TimeTrace.Platform.Linux` | Linux-specific capture (X11/Wayland) |
| **Windows Impl** | `TimeTrace.Platform.Windows` | Windows-specific capture (future) |
| **Persistence** | `timetrace.library` (existing) | SQLite repositories |

### Key Design Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| **Background process model** | .NET Worker Service + systemd user service | Desktop session access, auto-restart, logging, no root needed |
| **Abstraction location** | Separate `Platform.Abstractions` project | Clean dependencies, follows MS patterns, testable |
| **Screenshot capture (Linux)** | Process-based with fallback chain | Tool availability varies; `gnome-screenshot` → `scrot` (X11) → `grim` (Wayland) |
| **X11 + Wayland support** | Runtime detection with fallback | Use `xdotool` for X11, portal/wlrctl for Wayland |
| **Temp folder** | `~/.local/share/timetrace/captures/` | User-writable, no sudo, follows XDG spec |
| **Configuration** | Existing `ConfigurationRepository` | Reuse proven infrastructure |

---

## 2. Current State Analysis

### Existing `timetrace.library` Structure

```
timetrace.library/
├── Models/
│   ├── Process.cs              # Application being tracked (Name unique, max 50 chars)
│   ├── ProcessDetail.cs        # Session/window instance (Description required, max 255 chars)
│   ├── Image.cs                # Screenshot metadata (auto-generates Name, ImagePath, ImageGuid)
│   ├── ConfigurationSetting.cs # Config groups (Index-based)
│   └── ConfigurationSettingDetail.cs # Key-value config
├── Repositories/
│   ├── ProcessRepository.cs    # CRUD for Process/ProcessDetail + GetProcessByName()
│   ├── ImageRepository.cs      # CRUD for Image (auto-generates paths via config)
│   └── ConfigurationRepository.cs # Config management
├── Context/
│   └── DatabaseContext.cs      # EF Core DbContext (SQLite)
├── Extensions/
│   └── ServiceCollectionExtensions.cs  # AddDatabaseContextFactory()
└── Utils/
    └── ConfigSettingConstants.cs       # Existing config keys
```

### ⚠️ IMPORTANT: Existing Integration Points

The library already has conventions we **MUST** follow:

| Component | Existing Convention | Impact on Background Processor |
|-----------|--------------------|---------------------------------|
| **Image path generation** | `ImageRepository.AddImage()` auto-generates `Name` and `ImagePath` using `ConfigurationRepository` | We capture to temp, then let `AddImage()` generate final path |
| **Config keys** | `ConfigSettingConstants.FilePathConfigSettingIndex = "Paths"`, `ImagePathConfigSettingKey = "ImagePath"` | Add new keys to same constants file |
| **DI setup** | `AddDatabaseContextFactory(connectionString)` | Use this directly in `Program.cs` |
| **Process lookup** | `GetProcessByName(string name)` | Reuse existing processes instead of duplicating |
| **DB location** | `LocalApplicationData/timetrace.db` | Share same DB across UI and background processor |

### Entity Relationships

```
┌─────────────────┐     ┌─────────────────────┐     ┌─────────────┐
│    Process      │────<│   ProcessDetail     │────<│   Image     │
│ (e.g., Firefox) │  1:n│ (window/session)    │  1:n│ (screenshot)│
├─────────────────┤     ├─────────────────────┤     ├─────────────┤
│ ProcessId (PK)  │     │ ProcessDetailId(PK) │     │ ImageId(PK) │
│ Name            │     │ Description         │     │ Name        │
│ DateTimeStamp   │     │ ProcessId (FK)      │     │ ImagePath   │
└─────────────────┘     │ DateTimeStamp       │     │ ImageGuid   │
                        └─────────────────────┘     │ ProcessDetailId(FK)│
                                                    │ DateTimeStamp│
                                                    └─────────────┘
```

### Configuration System

The existing `ConfigurationRepository` uses a hierarchical key-value structure:

```
ConfigurationSetting (Index: "ScreenshotCapture")
├── ConfigurationSettingDetail (Key: "IntervalSeconds", Value: "30")
├── ConfigurationSettingDetail (Key: "TempFolderPath", Value: "~/.local/share/timetrace/captures")
└── ConfigurationSettingDetail (Key: "Enabled", Value: "true")
```

---

## 3. Proposed Architecture

### High-Level Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        TimeTrace.BackgroundProcessor                        │
│                          (Worker Service Host)                              │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │                    ScreenshotCaptureWorker                           │   │
│  │                    (BackgroundService)                               │   │
│  └─────────────────────────────┬───────────────────────────────────────┘   │
│                                │ uses                                       │
│  ┌─────────────────────────────▼───────────────────────────────────────┐   │
│  │                  CaptureOrchestrationService                         │   │
│  │           (coordinates capture → temp save → DB persist)            │   │
│  └───────┬─────────────────────┬─────────────────────────┬─────────────┘   │
│          │                     │                         │                  │
└──────────┼─────────────────────┼─────────────────────────┼──────────────────┘
           │ uses                │ uses                    │ uses
           ▼                     ▼                         ▼
┌─────────────────────┐ ┌─────────────────────┐ ┌─────────────────────────────┐
│TimeTrace.Platform   │ │TimeTrace.Platform   │ │  timetrace.library          │
│.Abstractions        │ │.Linux               │ │  (Existing)                 │
├─────────────────────┤ ├─────────────────────┤ ├─────────────────────────────┤
│IScreenshotCapture   │ │LinuxScreenshot      │ │ ProcessRepository           │
│  Service            │◄│  CaptureService     │ │ ImageRepository             │
│IActiveWindowService │ │LinuxActiveWindow    │ │ ConfigurationRepository     │
│IPlatformService     │ │  Service            │ │ DatabaseContext             │
│  Registration       │ │LinuxPlatformService │ └─────────────────────────────┘
└─────────────────────┘ │  Registration       │
                        └─────────────────────┘
                                 │
                                 │ uses (process-based)
                                 ▼
                        ┌─────────────────────┐
                        │  External Tools     │
                        ├─────────────────────┤
                        │ gnome-screenshot    │
                        │ scrot (X11)         │
                        │ grim (Wayland)      │
                        │ xdotool (X11)       │
                        │ wlrctl (Wayland)    │
                        └─────────────────────┘
```

### Dependency Flow

```
TimeTrace.BackgroundProcessor
    ├── references → TimeTrace.Platform.Abstractions
    ├── references → TimeTrace.Platform.Linux (conditional: Linux build)
    ├── references → TimeTrace.Platform.Windows (conditional: Windows build)
    └── references → timetrace.library

TimeTrace.Platform.Linux
    └── references → TimeTrace.Platform.Abstractions

TimeTrace.Platform.Windows
    └── references → TimeTrace.Platform.Abstractions

TimeTrace.Platform.Abstractions
    └── references → (none, pure interfaces)
```

---

## 4. Project Structure

### New Projects to Create

```
TimeTrace/
├── TimeTrace.sln                              # ✅ Update to include new projects
│
├── timetrace.library/                         # ✅ EXISTING - No changes needed
├── timetrace.library.tests/                   # ✅ EXISTING
├── timetrace.ui/                              # ✅ EXISTING
│
├── TimeTrace.Platform.Abstractions/           # 🆕 CREATE
│   ├── TimeTrace.Platform.Abstractions.csproj
│   ├── Screenshot/
│   │   ├── IScreenshotCaptureService.cs
│   │   ├── IActiveWindowService.cs
│   │   ├── ScreenshotResult.cs
│   │   └── ActiveWindowInfo.cs
│   ├── Process/
│   │   └── IProcessInfoService.cs
│   └── Extensions/
│       └── PlatformServiceCollectionExtensions.cs
│
├── TimeTrace.Platform.Linux/                  # 🆕 CREATE
│   ├── TimeTrace.Platform.Linux.csproj
│   ├── Screenshot/
│   │   ├── LinuxScreenshotCaptureService.cs
│   │   ├── LinuxActiveWindowService.cs
│   │   └── Capture/
│   │       ├── ICaptureStrategy.cs
│   │       ├── GnomeScreenshotCapture.cs
│   │       ├── ScrotCapture.cs
│   │       └── GrimCapture.cs
│   ├── Process/
│   │   └── LinuxProcessInfoService.cs
│   └── Extensions/
│       └── LinuxPlatformServiceCollectionExtensions.cs
│
├── TimeTrace.Platform.Windows/                # 🆕 CREATE (stub for now)
│   ├── TimeTrace.Platform.Windows.csproj
│   └── Extensions/
│       └── WindowsPlatformServiceCollectionExtensions.cs
│
├── TimeTrace.BackgroundProcessor/             # 🆕 CREATE
│   ├── TimeTrace.BackgroundProcessor.csproj
│   ├── Program.cs
│   ├── Workers/
│   │   └── ScreenshotCaptureWorker.cs
│   ├── Services/
│   │   ├── ICaptureOrchestrationService.cs
│   │   └── CaptureOrchestrationService.cs
│   ├── Configuration/
│   │   └── CaptureSettings.cs
│   ├── appsettings.json
│   └── appsettings.Linux.json
│
├── TimeTrace.BackgroundProcessor.Tests/       # 🆕 CREATE
│   ├── TimeTrace.BackgroundProcessor.Tests.csproj
│   ├── Workers/
│   │   └── ScreenshotCaptureWorkerTests.cs
│   └── Services/
│       └── CaptureOrchestrationServiceTests.cs
│
└── docs/
    └── architecture/
        └── BACKGROUND_PROCESSOR_ARCHITECTURE.md  # This file
```

### Project File Definitions

#### TimeTrace.Platform.Abstractions.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <RootNamespace>TimeTrace.Platform.Abstractions</RootNamespace>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="8.0.*" />
  </ItemGroup>
</Project>
```

#### TimeTrace.Platform.Linux.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <RuntimeIdentifiers>linux-x64;linux-arm64</RuntimeIdentifiers>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <RootNamespace>TimeTrace.Platform.Linux</RootNamespace>
  </PropertyGroup>
  
  <ItemGroup>
    <ProjectReference Include="..\TimeTrace.Platform.Abstractions\TimeTrace.Platform.Abstractions.csproj" />
  </ItemGroup>
  
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.*" />
  </ItemGroup>
</Project>
```

#### TimeTrace.BackgroundProcessor.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk.Worker">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <RuntimeIdentifiers>linux-x64;linux-arm64;win-x64</RuntimeIdentifiers>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <RootNamespace>TimeTrace.BackgroundProcessor</RootNamespace>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="8.0.*" />
    <PackageReference Include="Microsoft.Extensions.Hosting.Systemd" Version="8.0.*" />
  </ItemGroup>
  
  <ItemGroup>
    <ProjectReference Include="..\timetrace.library\timetrace.library.csproj" />
    <ProjectReference Include="..\TimeTrace.Platform.Abstractions\TimeTrace.Platform.Abstractions.csproj" />
  </ItemGroup>
  
  <!-- Conditional platform references -->
  <ItemGroup Condition="$([MSBuild]::IsOSPlatform('Linux'))">
    <ProjectReference Include="..\TimeTrace.Platform.Linux\TimeTrace.Platform.Linux.csproj" />
  </ItemGroup>
  
  <ItemGroup Condition="$([MSBuild]::IsOSPlatform('Windows'))">
    <ProjectReference Include="..\TimeTrace.Platform.Windows\TimeTrace.Platform.Windows.csproj" />
  </ItemGroup>
</Project>
```

---

## 5. Interface Design

### Core Interfaces

#### IScreenshotCaptureService

```csharp
namespace TimeTrace.Platform.Abstractions.Screenshot;

/// <summary>
/// Platform-agnostic screenshot capture service.
/// Implementations handle OS-specific capture mechanisms.
/// </summary>
public interface IScreenshotCaptureService
{
    /// <summary>
    /// Captures a screenshot of the currently active window.
    /// </summary>
    /// <param name="outputPath">Path where screenshot should be saved.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing success status, file path, and window metadata.</returns>
    Task<ScreenshotResult> CaptureActiveWindowAsync(
        string outputPath, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Captures a screenshot of the entire screen/display.
    /// </summary>
    Task<ScreenshotResult> CaptureFullScreenAsync(
        string outputPath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Indicates whether this capture service is available on the current system.
    /// </summary>
    bool IsAvailable { get; }
}
```

#### IActiveWindowService

```csharp
namespace TimeTrace.Platform.Abstractions.Screenshot;

/// <summary>
/// Service for detecting the currently active/focused window.
/// </summary>
public interface IActiveWindowService
{
    /// <summary>
    /// Gets information about the currently active window.
    /// </summary>
    /// <returns>Active window info, or null if no window is focused.</returns>
    Task<ActiveWindowInfo?> GetActiveWindowAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Indicates whether active window detection is supported on this system.
    /// </summary>
    bool IsSupported { get; }
}
```

#### Data Transfer Objects

```csharp
namespace TimeTrace.Platform.Abstractions.Screenshot;

/// <summary>
/// Information about the currently active window.
/// </summary>
public sealed record ActiveWindowInfo
{
    /// <summary>Process name (e.g., "firefox", "code").</summary>
    public required string ProcessName { get; init; }
    
    /// <summary>Window title (e.g., "GitHub - Mozilla Firefox").</summary>
    public required string WindowTitle { get; init; }
    
    /// <summary>OS process identifier.</summary>
    public required int ProcessId { get; init; }
    
    /// <summary>Executable path if available.</summary>
    public string? ExecutablePath { get; init; }
    
    /// <summary>Timestamp when window info was captured.</summary>
    public DateTime CapturedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Result of a screenshot capture operation.
/// </summary>
public sealed record ScreenshotResult
{
    public required bool Success { get; init; }
    
    /// <summary>Path to saved screenshot file (only if Success is true).</summary>
    public string? FilePath { get; init; }
    
    /// <summary>Error message (only if Success is false).</summary>
    public string? ErrorMessage { get; init; }
    
    /// <summary>Active window info at time of capture.</summary>
    public ActiveWindowInfo? WindowInfo { get; init; }
    
    /// <summary>Duration of the capture operation.</summary>
    public TimeSpan Duration { get; init; }

    public static ScreenshotResult Succeeded(string filePath, ActiveWindowInfo? windowInfo, TimeSpan duration) =>
        new() { Success = true, FilePath = filePath, WindowInfo = windowInfo, Duration = duration };
    
    public static ScreenshotResult Failed(string error) =>
        new() { Success = false, ErrorMessage = error };
}
```

#### IPlatformServiceRegistration

```csharp
namespace TimeTrace.Platform.Abstractions;

/// <summary>
/// Marker interface for platform-specific service registration.
/// Each platform project implements this to register its services.
/// </summary>
public interface IPlatformServiceRegistration
{
    /// <summary>
    /// Registers platform-specific services with the DI container.
    /// </summary>
    void RegisterServices(IServiceCollection services);
    
    /// <summary>
    /// Returns true if this platform registration applies to the current OS.
    /// </summary>
    bool IsCurrentPlatform { get; }
}
```

---

## 6. Linux Implementation Details

### Screenshot Capture Strategy

The Linux implementation uses a **Chain of Responsibility** pattern to try multiple capture tools:

```csharp
namespace TimeTrace.Platform.Linux.Screenshot;

public class LinuxScreenshotCaptureService : IScreenshotCaptureService
{
    private readonly IEnumerable<ICaptureStrategy> _captureStrategies;
    private readonly IActiveWindowService _activeWindowService;
    private readonly ILogger<LinuxScreenshotCaptureService> _logger;
    
    public LinuxScreenshotCaptureService(
        IEnumerable<ICaptureStrategy> captureStrategies,
        IActiveWindowService activeWindowService,
        ILogger<LinuxScreenshotCaptureService> logger)
    {
        // Order by priority (highest first)
        _captureStrategies = captureStrategies.OrderByDescending(s => s.Priority);
        _activeWindowService = activeWindowService;
        _logger = logger;
    }
    
    public bool IsAvailable => _captureStrategies.Any(s => s.IsAvailable);
    
    public async Task<ScreenshotResult> CaptureActiveWindowAsync(
        string outputPath, 
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var windowInfo = await _activeWindowService.GetActiveWindowAsync(cancellationToken);
        
        foreach (var strategy in _captureStrategies.Where(s => s.IsAvailable))
        {
            try
            {
                var result = await strategy.CaptureActiveWindowAsync(outputPath, cancellationToken);
                if (result.Success)
                {
                    stopwatch.Stop();
                    return result with { WindowInfo = windowInfo, Duration = stopwatch.Elapsed };
                }
                _logger.LogDebug("Strategy {Strategy} failed: {Error}", 
                    strategy.GetType().Name, result.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Strategy {Strategy} threw exception", strategy.GetType().Name);
            }
        }
        
        return ScreenshotResult.Failed("All capture strategies failed");
    }
}
```

### Capture Strategy Interface

```csharp
namespace TimeTrace.Platform.Linux.Screenshot.Capture;

public interface ICaptureStrategy
{
    /// <summary>Tool/method name for logging.</summary>
    string Name { get; }
    
    /// <summary>Priority (higher = tried first).</summary>
    int Priority { get; }
    
    /// <summary>Whether this tool is available on the system.</summary>
    bool IsAvailable { get; }
    
    /// <summary>Captures the active window screenshot.</summary>
    Task<ScreenshotResult> CaptureActiveWindowAsync(string outputPath, CancellationToken ct);
    
    /// <summary>Captures full screen screenshot.</summary>
    Task<ScreenshotResult> CaptureFullScreenAsync(string outputPath, CancellationToken ct);
}
```

### Tool-Specific Implementations

| Strategy | Priority | X11 | Wayland | Notes |
|----------|----------|-----|---------|-------|
| `GnomeScreenshotCapture` | 100 | ✅ | ✅ | Most compatible, uses GNOME's portal |
| `ScrotCapture` | 80 | ✅ | ❌ | X11-only, lightweight |
| `GrimCapture` | 90 | ❌ | ✅ | Wayland-native (wlroots) |

### Active Window Detection

```csharp
namespace TimeTrace.Platform.Linux.Screenshot;

public class LinuxActiveWindowService : IActiveWindowService
{
    private readonly ILogger<LinuxActiveWindowService> _logger;
    
    public bool IsSupported => IsX11Available() || IsWaylandAvailable();
    
    public async Task<ActiveWindowInfo?> GetActiveWindowAsync(CancellationToken cancellationToken = default)
    {
        // Try X11 first (xdotool)
        if (IsX11Available())
        {
            return await GetActiveWindowX11Async(cancellationToken);
        }
        
        // Fallback to Wayland methods
        if (IsWaylandAvailable())
        {
            return await GetActiveWindowWaylandAsync(cancellationToken);
        }
        
        return null;
    }
    
    private async Task<ActiveWindowInfo?> GetActiveWindowX11Async(CancellationToken ct)
    {
        // xdotool getactivewindow getwindowname
        // xdotool getactivewindow getwindowpid
        var windowId = await RunCommandAsync("xdotool", "getactivewindow", ct);
        if (string.IsNullOrEmpty(windowId)) return null;
        
        var windowTitle = await RunCommandAsync("xdotool", $"getwindowname {windowId}", ct);
        var pidStr = await RunCommandAsync("xdotool", $"getwindowpid {windowId}", ct);
        
        if (!int.TryParse(pidStr, out var pid)) return null;
        
        var processName = GetProcessName(pid);
        
        return new ActiveWindowInfo
        {
            ProcessName = processName ?? "unknown",
            WindowTitle = windowTitle ?? "",
            ProcessId = pid,
            ExecutablePath = GetExecutablePath(pid)
        };
    }
    
    // ... Wayland implementation using wlrctl or D-Bus portal
}
```

### systemd User Service

```ini
# ~/.config/systemd/user/timetrace-capture.service
[Unit]
Description=TimeTrace Screenshot Capture Service
After=graphical-session.target

[Service]
Type=notify
ExecStart=%h/.local/bin/timetrace-capture
Restart=on-failure
RestartSec=5
Environment=DOTNET_ENVIRONMENT=Production

[Install]
WantedBy=default.target
```

Enable with:
```bash
systemctl --user enable timetrace-capture.service
systemctl --user start timetrace-capture.service
journalctl --user -u timetrace-capture.service -f  # View logs
```

---

## 7. Configuration Integration

### Using Existing ConfigurationRepository

The background processor will read configuration from the existing `ConfigurationRepository`.

**Existing constants (in `timetrace.library/Utils/ConfigSettingConstants.cs`):**
```csharp
public static class ConfigSettingConstants
{
    public const string FilePathConfigSettingIndex = "Paths";
    public const string ImagePathConfigSettingKey = "ImagePath";
}
```

**Proposed additions to `ConfigSettingConstants.cs`:**
```csharp
public static class ConfigSettingConstants
{
    // Existing
    public const string FilePathConfigSettingIndex = "Paths";
    public const string ImagePathConfigSettingKey = "ImagePath";
    
    // TODO: New constants for background processor - review and finalize names
    public const string CaptureConfigSettingIndex = "ScreenshotCapture";
    public const string CaptureIntervalSecondsKey = "IntervalSeconds";
    public const string CaptureTempFolderPathKey = "TempFolderPath";
    public const string CaptureEnabledKey = "Enabled";
    public const string CaptureMaxTempFolderSizeMBKey = "MaxTempFolderSizeMB";
}
```

### CaptureSettings DTO

```csharp
namespace TimeTrace.BackgroundProcessor.Configuration;

public sealed class CaptureSettings
{
    /// <summary>Interval between captures in seconds. Default: 30</summary>
    public int IntervalSeconds { get; set; } = 30;
    
    /// <summary>Path to temp folder for screenshots. Default: ~/.local/share/timetrace/captures</summary>
    public string TempFolderPath { get; set; } = GetDefaultTempPath();
    
    /// <summary>Whether capture is enabled. Default: true</summary>
    public bool Enabled { get; set; } = true;
    
    /// <summary>Maximum temp folder size in MB before cleanup. Default: 500</summary>
    public int MaxTempFolderSizeMB { get; set; } = 500;
    
    private static string GetDefaultTempPath()
    {
        // XDG Base Directory spec: ~/.local/share/timetrace/captures
        var xdgDataHome = Environment.GetEnvironmentVariable("XDG_DATA_HOME")
            ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share");
        return Path.Combine(xdgDataHome, "timetrace", "captures");
    }
}
```

### Configuration Service

```csharp
namespace TimeTrace.BackgroundProcessor.Services;

public interface ICaptureConfigurationService
{
    CaptureSettings GetCurrentSettings();
    void UpdateSettings(Action<CaptureSettings> updateAction);
}

public class CaptureConfigurationService : ICaptureConfigurationService
{
    private readonly IConfigurationRepository _configRepo;
    
    public CaptureSettings GetCurrentSettings()
    {
        var settings = new CaptureSettings();
        
        var configValues = _configRepo.FetchConfigurationSettingKeyValueByIndex(
            ConfigSettingConstants.CaptureConfigSettingIndex);
        
        if (configValues.TryGetValue(ConfigSettingConstants.CaptureIntervalSecondsKey, out var interval)
            && int.TryParse(interval, out var intervalValue))
        {
            settings.IntervalSeconds = intervalValue;
        }
        
        if (configValues.TryGetValue(ConfigSettingConstants.CaptureTempFolderPathKey, out var tempPath)
            && !string.IsNullOrEmpty(tempPath))
        {
            settings.TempFolderPath = ExpandPath(tempPath);
        }
        
        // ... other settings
        
        return settings;
    }
    
    private static string ExpandPath(string path) =>
        path.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
}
```

### Capture Orchestration Flow

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                      CaptureOrchestrationService                            │
│                                                                             │
│  1. Get active window info (IActiveWindowService)                           │
│       └─► ActiveWindowInfo { ProcessName, WindowTitle, ProcessId }          │
│                                                                             │
│  2. Capture screenshot to TEMP folder (IScreenshotCaptureService)           │
│       └─► /home/user/.local/share/timetrace/captures/{guid}.png             │
│                                                                             │
│  3. Find or create Process (IProcessRepository.GetProcessByName)            │
│       └─► If exists: reuse existing Process                                 │
│       └─► If new: Create Process { Name = ProcessName }                     │
│                                                                             │
│  4. Create ProcessDetail (IProcessRepository.AddProcessDetail)              │
│       └─► ProcessDetail { Description = WindowTitle, ProcessId = ... }      │
│                                                                             │
│  5. Create Image (IImageRepository.AddImage)                                │
│       └─► ⚠️ AddImage() AUTO-GENERATES Name and ImagePath from config!      │
│       └─► We need to MOVE file from temp to generated ImagePath             │
│                                                                             │
│  6. Move screenshot from temp to final ImagePath                            │
│       └─► File.Move(tempPath, image.ImagePath)                              │
│                                                                             │
│  7. Cleanup temp folder if over size limit                                  │
└─────────────────────────────────────────────────────────────────────────────┘
```

**⚠️ IMPORTANT:** The `ImageRepository.AddImage()` method automatically generates `Name` and `ImagePath` using the configuration repository (`Paths.ImagePath`). Our orchestration must:
1. Capture to temp folder first
2. Call `AddImage()` to get the generated path
3. Move the file from temp to the generated `ImagePath`

---

## 8. Dependency Injection Strategy

### Platform Auto-Detection

```csharp
namespace TimeTrace.Platform.Abstractions.Extensions;

public static class PlatformServiceCollectionExtensions
{
    /// <summary>
    /// Registers platform-specific services based on the current runtime platform.
    /// </summary>
    public static IServiceCollection AddPlatformServices(this IServiceCollection services)
    {
        var registrations = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IPlatformServiceRegistration).IsAssignableFrom(t) 
                        && !t.IsInterface && !t.IsAbstract)
            .Select(t => (IPlatformServiceRegistration)Activator.CreateInstance(t)!)
            .ToList();
        
        var currentPlatform = registrations.FirstOrDefault(r => r.IsCurrentPlatform);
        
        if (currentPlatform is null)
        {
            throw new PlatformNotSupportedException(
                $"No platform implementation found for {RuntimeInformation.OSDescription}. " +
                $"Ensure TimeTrace.Platform.Linux or TimeTrace.Platform.Windows is referenced.");
        }
        
        currentPlatform.RegisterServices(services);
        return services;
    }
}
```

### Linux Platform Registration

```csharp
namespace TimeTrace.Platform.Linux.Extensions;

public sealed class LinuxPlatformServiceRegistration : IPlatformServiceRegistration
{
    public bool IsCurrentPlatform => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    
    public void RegisterServices(IServiceCollection services)
    {
        // Active window detection
        services.AddSingleton<IActiveWindowService, LinuxActiveWindowService>();
        
        // Screenshot capture with chain-of-responsibility
        services.AddSingleton<IScreenshotCaptureService, LinuxScreenshotCaptureService>();
        
        // Individual capture strategies
        services.AddSingleton<ICaptureStrategy, GnomeScreenshotCapture>();
        services.AddSingleton<ICaptureStrategy, ScrotCapture>();
        services.AddSingleton<ICaptureStrategy, GrimCapture>();
    }
}
```

### Background Processor Program.cs

```csharp
using TimeTrace.Platform.Abstractions.Extensions;
// Uses existing DI extension from timetrace.library

var builder = Host.CreateApplicationBuilder(args);

// Platform-specific services (auto-detects Linux/Windows)
builder.Services.AddPlatformServices();

// ✅ Existing library DI - uses same DB as UI
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
    builder.UseSystemd();
}

var host = builder.Build();
await host.RunAsync();
```

**Note:** The `AddDatabaseContextFactory` extension already handles:
- DB path (`LocalApplicationData/timetrace.db`)
- Password retrieval
- Registering `IProcessRepository`, `IImageRepository`, `IConfigurationRepository`

---

## 9. Testing Strategy

### Test Layers

| Test Type | What to Test | Mocking Approach |
|-----------|--------------|------------------|
| **Unit (Orchestration)** | `CaptureOrchestrationService` logic | Mock `IScreenshotCaptureService`, `IActiveWindowService`, repositories |
| **Unit (Worker)** | `ScreenshotCaptureWorker` timing, error handling | Mock `ICaptureOrchestrationService` |
| **Integration (Linux)** | Actual screenshot tools | Docker + Xvfb (headless X11) |
| **Integration (Config)** | Configuration loading | In-memory SQLite |

### Unit Test Example

```csharp
[TestFixture]
public class CaptureOrchestrationServiceTests
{
    private Mock<IScreenshotCaptureService> _screenshotServiceMock;
    private Mock<IActiveWindowService> _windowServiceMock;
    private Mock<IProcessRepository> _processRepoMock;
    private Mock<IImageRepository> _imageRepoMock;
    private CaptureOrchestrationService _sut;
    
    [Test]
    public async Task CaptureAndPersistAsync_WhenWindowActive_CreatesProcessAndImage()
    {
        // Arrange
        var windowInfo = new ActiveWindowInfo
        {
            ProcessName = "firefox",
            WindowTitle = "GitHub - Mozilla Firefox",
            ProcessId = 12345
        };
        
        _windowServiceMock
            .Setup(x => x.GetActiveWindowAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(windowInfo);
            
        _screenshotServiceMock
            .Setup(x => x.CaptureActiveWindowAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ScreenshotResult.Succeeded("/tmp/screenshot.png", windowInfo, TimeSpan.FromMilliseconds(50)));
        
        // Act
        var result = await _sut.CaptureAndPersistAsync(CancellationToken.None);
        
        // Assert
        Assert.That(result.Success, Is.True);
        _processRepoMock.Verify(x => x.AddProcess(It.Is<Process>(p => p.Name == "firefox")), Times.Once);
        _imageRepoMock.Verify(x => x.AddImage(It.IsAny<Image>()), Times.Once);
    }
}
```

### Docker + Xvfb Integration Test

```dockerfile
# Dockerfile.test-linux
FROM mcr.microsoft.com/dotnet/sdk:8.0

RUN apt-get update && apt-get install -y \
    xvfb xdotool scrot gnome-screenshot x11-apps \
    && rm -rf /var/lib/apt/lists/*

ENV DISPLAY=:99

COPY . /app
WORKDIR /app

CMD ["bash", "-c", "Xvfb :99 -screen 0 1024x768x24 & sleep 2 && dotnet test --filter Category=LinuxIntegration"]
```

---

## 10. Implementation Plan

### Implementation Status

**Last Updated:** 2026-03-31

| Phase | Status | Notes |
|-------|--------|-------|
| Phase 0: Library Modifications | ✅ Complete | Config constants added |
| Phase 1: Foundation | ✅ Complete | All interfaces and projects created |
| Phase 2: Linux Implementation | ✅ Complete | X11 + Wayland support |
| Phase 3: Orchestration & Persistence | ✅ Complete | Worker, services wired up |
| Phase 4: Testing & Validation | ✅ Complete | 21 unit tests passing |
| Phase 5: Deployment & Polish | ✅ Complete | systemd service + Windows stub |

### Phase 0: Library Modifications (Prerequisite)

| # | Task | File | Change |
|---|------|------|--------|
| 0.1 | Add capture config constants | `timetrace.library/Utils/ConfigSettingConstants.cs` | Add `CaptureConfigSettingIndex`, `CaptureIntervalSecondsKey`, etc. |
| 0.2 | Verify DI extension works cross-platform | `ServiceCollectionExtensions.cs` | Test `LocalApplicationData` path on Linux |

### Phase 1: Foundation (Abstractions + Skeleton)

| # | Task | Dependencies |
|---|------|--------------|
| 1.1 | Create `TimeTrace.Platform.Abstractions` project | None |
| 1.2 | Define core interfaces (`IScreenshotCaptureService`, `IActiveWindowService`, DTOs) | 1.1 |
| 1.3 | Create `TimeTrace.Platform.Linux` project skeleton | 1.2 |
| 1.4 | Create `TimeTrace.BackgroundProcessor` Worker Service skeleton | 1.2 |
| 1.5 | Update `TimeTrace.sln` with new projects | 1.1-1.4 |

### Phase 2: Linux Implementation

| # | Task | Dependencies |
|---|------|--------------|
| 2.1 | Implement `LinuxActiveWindowService` (X11 via xdotool) | 1.3 |
| 2.2 | Implement `ICaptureStrategy` interface and base class | 1.3 |
| 2.3 | Implement `ScrotCapture` strategy (X11) | 2.2 |
| 2.4 | Implement `GnomeScreenshotCapture` strategy | 2.2 |
| 2.5 | Implement `GrimCapture` strategy (Wayland) | 2.2 |
| 2.6 | Implement `LinuxScreenshotCaptureService` with fallback chain | 2.1, 2.3-2.5 |
| 2.7 | Implement `LinuxPlatformServiceRegistration` | 2.6 |

### Phase 3: Orchestration & Persistence

| # | Task | Dependencies |
|---|------|--------------|
| 3.1 | Implement `CaptureConfigurationService` (reads from `ConfigurationRepository`) | 1.4 |
| 3.2 | Implement `CaptureOrchestrationService` (coordinates capture → persist) | 2.6, 3.1 |
| 3.3 | Implement `ScreenshotCaptureWorker` (BackgroundService) | 3.2 |
| 3.4 | Wire up DI in `Program.cs` | 3.3 |

### Phase 4: Testing & Validation

| # | Task | Dependencies |
|---|------|--------------|
| 4.1 | Create `TimeTrace.BackgroundProcessor.Tests` project | 3.4 |
| 4.2 | Write unit tests for `CaptureOrchestrationService` | 4.1 |
| 4.3 | Write unit tests for `ScreenshotCaptureWorker` | 4.1 |
| 4.4 | Create Docker + Xvfb integration test setup | 4.1 |
| 4.5 | Validate on actual Linux desktop (X11 and Wayland) | 4.4 |

### Phase 5: Deployment & Polish

| # | Task | Dependencies |
|---|------|--------------|
| 5.1 | Create systemd user service file | 4.5 |
| 5.2 | Create installation script | 5.1 |
| 5.3 | Document deployment steps | 5.2 |
| 5.4 | Stub `TimeTrace.Platform.Windows` (no-op implementation) | 1.2 |

---

## 11. Open Questions & TODOs

### Configuration Keys (TODO)

```csharp
// TODO: Add to timetrace.library/Utils/ConfigSettingConstants.cs
public const string CaptureConfigSettingIndex = "ScreenshotCapture";
public const string CaptureIntervalSecondsKey = "IntervalSeconds";
public const string CaptureTempFolderPathKey = "TempFolderPath";  
public const string CaptureEnabledKey = "Enabled";
public const string CaptureMaxTempFolderSizeMBKey = "MaxTempFolderSizeMB";
```

### DB Path Consideration

The existing `ServiceCollectionExtensions.GetDbPath()` uses:
```csharp
Environment.SpecialFolder.LocalApplicationData
```

On Linux, this maps to `~/.local/share/` which aligns with XDG spec. ✅

### Image Path Generation

`ImageRepository.AddImage()` reads the final image path from:
```
ConfigurationSetting Index: "Paths"
ConfigurationSettingDetail Key: "ImagePath"
```

**Question:** Should the background processor use the same `Paths.ImagePath` as the UI, or should it have its own config?

**Recommendation:** Use the same config so all images go to the same location regardless of source.

### Open Questions

| # | Question | Impact | Default Assumption |
|---|----------|--------|-------------------|
| 1 | Should we capture on active window *change* or on a fixed interval? | Worker design | Fixed interval (configurable) |
| 2 | Should temp images be auto-cleaned after DB persistence? | Storage management | Yes, configurable retention |
| 3 | What happens if the same process/window is captured repeatedly? | DB design | Create new `ProcessDetail` per session, reuse `Process` by name |
| 4 | Should we support multiple monitors? | Screenshot capture | Capture active monitor only (initial scope) |
| 5 | Wayland: Use portal API or compositor-specific tools? | Linux impl | Try `gnome-screenshot` first (uses portal), fallback to `grim` |

### Deferred Items (Windows)

- `TimeTrace.Platform.Windows` implementation
- Windows service registration (vs systemd)
- Windows screenshot capture (Win32 API or `Windows.Graphics.Capture`)

---

## Appendix: Quick Reference

### Temp Folder Location

```bash
# Linux (XDG spec)
~/.local/share/timetrace/captures/

# Expands to:
/home/{username}/.local/share/timetrace/captures/
```

### External Tool Dependencies (Linux)

```bash
# X11 support
sudo apt install xdotool scrot

# GNOME (X11 + Wayland)
sudo apt install gnome-screenshot

# Wayland (wlroots-based)
sudo apt install grim wlrctl
```

### systemd Commands

```bash
# Enable and start
systemctl --user enable timetrace-capture.service
systemctl --user start timetrace-capture.service

# View status and logs
systemctl --user status timetrace-capture.service
journalctl --user -u timetrace-capture.service -f

# Restart after update
systemctl --user restart timetrace-capture.service
```

---

**Next Step:** Review this architecture and provide feedback. Once approved, I can proceed with creating the project structure and implementing Phase 1.
