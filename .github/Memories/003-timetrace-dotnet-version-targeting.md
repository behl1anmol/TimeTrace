# Memory: TimeTrace .NET Version Targeting

## Metadata
- PatternId: MEMORY-003
- PatternVersion: 1
- Status: active
- Supersedes: none
- CreatedAt: 2026-03-31
- LastValidatedAt: 2026-03-31
- ValidationEvidence: Build failures when targeting net8.0, success with net9.0

## Source Context
- Triggering task: Creating new projects for Background Processor
- Scope/system: TimeTrace solution
- Date/time: 2026-03-31

## Memory
- Key fact or decision: TimeTrace targets .NET 9, all new projects must use `net9.0`
- Why it matters: Project reference compatibility requires matching TFMs

## Key Facts

### Target Framework Configuration
```xml
<PropertyGroup>
  <TargetFramework>net9.0</TargetFramework>
  <LangVersion>latest</LangVersion>
  <ImplicitUsings>enable</ImplicitUsings>
  <Nullable>enable</Nullable>
</PropertyGroup>
```

### Windows-Specific Projects
```xml
<TargetFramework>net9.0-windows</TargetFramework>
```

### Package Version Strategy
- Use `9.*` pattern for forward compatibility: `<PackageReference Include="..." Version="9.*" />`
- NOT `9.0.*` which is too restrictive

### Existing Project TFMs
- `timetrace.library` → `net9.0`
- `timetrace.ui` → `net9.0-windows`
- `timetrace.library.tests` → `net9.0`

## Applicability
- When to reuse: Creating any new project in TimeTrace solution
- Preconditions/limitations: May need updates when .NET 10 releases

## Actionable Guidance
- Recommended future action: Check existing projects' TFMs before creating new ones
- Related files/services/components:
  - `timetrace.library/timetrace.library.csproj`
  - `TimeTrace.sln`
