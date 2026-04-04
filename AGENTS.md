# AGENTS.md

## Start here
- Read `docs/architecture/BACKGROUND_PROCESSOR_ARCHITECTURE.md` for the current system map when touching background, platform, or persistence code.
- Check `.github/Memories/*.md` and `.github/Lessons/*.md` before changing `timetrace.library`, `TimeTrace.Platform.*`, or Avalonia UI code; they capture repo-specific pitfalls.

## Big picture
- The solution is split into `timetrace.library` (SQLite + repositories), `TimeTrace.Platform.Abstractions` (interfaces/DTOs), `TimeTrace.Platform.Linux` / `TimeTrace.Platform.Windows` (OS-specific services), `TimeTrace.BackgroundProcessor` (worker host), `timetrace.ui.avalonia` (Linux Avalonia UI), and `timetrace.ui` (legacy WPF UI).
- Background capture flows through `TimeTrace.BackgroundProcessor/Program.cs` → `ScreenshotCaptureWorker` → `CaptureOrchestrationService` → platform services → `timetrace.library` repositories.
- Linux platform registration is auto-detected via `IPlatformServiceRegistration` and `AddPlatformServices()`; explicit `AddLinuxPlatformServices()` / `AddWindowsPlatformServices()` exist for tests and targeted wiring.

## Project conventions that matter
- Target `net9.0`, keep `Nullable` and `ImplicitUsings` enabled, and follow the existing namespace/file layout per project.
- In `timetrace.library`, entity IDs use full names like `ProcessId`, `ImageId`, and `ProcessDetailId`.
- `IProcessRepository` owns both `Process` and `ProcessDetail` operations; there is no separate process-detail repository.
- `ImageRepository.AddImage()` auto-generates `Name` and `ImagePath` from config, so orchestration code should not invent its own final image path.
- `ServiceCollectionExtensions.AddDatabaseContextFactory()` always points to `LocalApplicationData/timetrace.db` and registers `IProcessRepository`, `IConfigurationRepository`, `IImageRepository`, and `IRepositoryBase`.
- In Linux capture services, strategy order matters: `GnomeScreenshotCapture`, `GrimCapture`, then `ScrotCapture` are registered for DI enumeration/fallback.

## Avalonia-specific rules
- Avalonia files use `.axaml` / `.axaml.cs`, not WPF `.xaml`.
- Use `ResourceInclude` for resource dictionaries and `StyleInclude` for styles; mixing them triggers build errors.
- In `App.axaml`, keep `Application.Resources` before `Application.Styles` so `StaticResource` lookups resolve.
- Prefer `x:DataType` and compiled bindings; the project enables `AvaloniaUseCompiledBindingsByDefault`.
- `timetrace.ui.avalonia/App.axaml.cs` wires DI manually and currently uses `MockApplicationDataService`.

## Test and debug workflow
- Build the whole solution with `dotnet build TimeTrace.sln`.
- Run tests with `dotnet test TimeTrace.sln`; test projects use NUnit, Moq, and coverlet.
- For UI checks, run `dotnet run --project timetrace.ui.avalonia/timetrace.ui.avalonia.csproj`.
- For background service checks, run `dotnet run --project TimeTrace.BackgroundProcessor/TimeTrace.BackgroundProcessor.csproj`.
- On Linux, use the testing notes in `docs/testing-guide-linux.md` for display-server, distro, and runtime verification.

## Change discipline
- Verify actual interface signatures before writing mocks or tests; several repo lessons were created after signature mismatches.
- Keep changes localized to the project that owns the behavior, and prefer the existing DI/extensions over adding parallel wiring.
- If you touch new platform behavior, update the architecture doc or a memory/lesson only when the change adds durable guidance.

