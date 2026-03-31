# Memory: TimeTrace Library Repository Patterns

## Metadata
- PatternId: MEMORY-001
- PatternVersion: 1
- Status: active
- Supersedes: none
- CreatedAt: 2026-03-31
- LastValidatedAt: 2026-03-31
- ValidationEvidence: Reviewed IProcessRepository.cs, IImageRepository.cs, IConfigurationRepository.cs

## Source Context
- Triggering task: Implementing Background Processor integration with timetrace.library
- Scope/system: timetrace.library/Repositories
- Date/time: 2026-03-31

## Memory
- Key fact or decision: Repository design patterns used in timetrace.library
- Why it matters: Understanding these patterns is critical for correct integration

## Key Facts

### Property Naming Convention
- Entity IDs use full name: `ProcessId`, `ImageId`, `ProcessDetailId` (NOT just `Id`)
- Example: `process.ProcessId`, `image.ImageId`, `processDetail.ProcessDetailId`

### IProcessRepository Contains ProcessDetail Methods
- No separate `IProcessDetailRepository` exists
- `IProcessRepository` handles both `Process` and `ProcessDetail` entities
- Methods: `AddProcessDetail()`, `GetProcessDetail()`, `GetProcessDetails()`, etc.

### Repository Method Return Types
- `AddProcess()` returns `Process` (synchronous)
- `AddImage()` returns `Image` (synchronous, not Task<Image>)
- `GetProcessByName()` returns `Process?`

### Configuration Repository Pattern
- `FetchConfigurationSettingKeyValueByIndex(string index)` returns `Dictionary<string, string?>`
- NOT `GetConfigurationSettingDetailByKeyAsync()` (this method doesn't exist)
- Configuration is organized by "Index" (e.g., "ScreenshotCapture", "Paths")

### Image Path Auto-Generation
- `ImageRepository.AddImage()` auto-generates `Name` and `ImagePath`
- Reads path from config: Index="Paths", Key="ImagePath"

## Applicability
- When to reuse: Any integration with timetrace.library repositories
- Preconditions/limitations: Applies to current timetrace.library design

## Actionable Guidance
- Recommended future action: Always review repository interfaces before writing integration code
- Related files/services/components:
  - `timetrace.library/Repositories/IProcessRepository.cs`
  - `timetrace.library/Repositories/IImageRepository.cs`
  - `timetrace.library/Repositories/IConfigurationRepository.cs`
  - `timetrace.library/Utils/ConfigSettingConstants.cs`
