# Lesson: Namespace Conflicts with System Types Require Type Aliases

## Metadata
- PatternId: LESSON-002
- PatternVersion: 1
- Status: active
- Supersedes: none
- CreatedAt: 2026-03-31
- LastValidatedAt: 2026-03-31
- ValidationEvidence: Build failure in TimeTrace.Platform.Linux due to Process namespace conflict

## Task Context
- Triggering task: Implementing Linux platform services (Phase 2)
- Date/time: 2026-03-31
- Impacted area: TimeTrace.Platform.Linux

## Mistake
- What went wrong: Using `System.Diagnostics.Process` in a namespace containing `TimeTrace.Platform.Linux.Process`
- Expected behavior: Code should compile without ambiguous type references
- Actual behavior: CS0118 error - 'Process' is a namespace but used as a type

## Root Cause Analysis
- Primary cause: Project structure created `Process/` folder for process-related services, creating namespace conflict
- Contributing factors: Common pattern of grouping related services in subdirectories
- Detection gap: Conflict not apparent until adding code that uses `System.Diagnostics.Process`

## Resolution
- Fix implemented: Used type alias at file level:
  ```csharp
  using SysProcess = System.Diagnostics.Process;
  using SysProcessStartInfo = System.Diagnostics.ProcessStartInfo;
  ```
- Why this fix works: Aliases disambiguate the system types from the namespace
- Verification performed: Build succeeded after applying aliases

## Preventive Actions
- Guardrails added: Document type alias pattern in architecture doc
- Tests/checks added: None (compile-time error catches this)
- Process updates: When creating namespace structures, check for conflicts with `System.*` types

## Reuse Guidance
- How to apply this lesson in future tasks:
  1. Common conflicting names: `Process`, `File`, `Path`, `Environment`, `Task`
  2. When creating subdirectories/namespaces, consider potential conflicts
  3. Use descriptive namespace names (e.g., `ProcessInfo` instead of `Process`)
  4. If conflict exists, use type aliases consistently across all files in that namespace
