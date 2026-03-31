# Lesson: Verify Interface Signatures Before Writing Tests

## Metadata
- PatternId: LESSON-001
- PatternVersion: 1
- Status: active
- Supersedes: none
- CreatedAt: 2026-03-31
- LastValidatedAt: 2026-03-31
- ValidationEvidence: Build failures in TimeTrace.BackgroundProcessor.Tests due to interface mismatches

## Task Context
- Triggering task: Implementing Phase 4 (Testing) for Background Processor
- Date/time: 2026-03-31
- Impacted area: TimeTrace.BackgroundProcessor.Tests

## Mistake
- What went wrong: Tests were written assuming interface method signatures without verifying the actual implementations
- Expected behavior: Tests should compile and match the actual service interfaces
- Actual behavior: 59 build errors due to:
  - Wrong method names (`CaptureScreenshotAsync` vs `CaptureActiveWindowAsync`)
  - Wrong constructor parameters for `CaptureOrchestrationService`
  - Non-existent properties (`CapturedAt` on `ScreenshotResult`, `Id` vs `ProcessId`)
  - Missing `IsAvailable` and `IsSupported` properties on interfaces

## Root Cause Analysis
- Primary cause: Assumptions made about interface design without consulting actual source files
- Contributing factors: Complex multi-file implementation with multiple interfaces
- Detection gap: Tests were created in bulk before attempting compilation

## Resolution
- Fix implemented: Viewed actual interface files and rewrote tests to match real signatures
- Why this fix works: Tests now use correct method names, parameters, and property names
- Verification performed: Build succeeded, all 21 tests passed

## Preventive Actions
- Guardrails added: Always `view` interface files before writing tests
- Tests/checks added: Build verification before committing test files
- Process updates: When creating test projects, read service implementations first

## Reuse Guidance
- How to apply this lesson in future tasks:
  1. Before writing any test file, `view` the actual interface being tested
  2. Verify constructor parameters by checking the service implementation
  3. Check property names on DTOs/records (e.g., `ProcessId` vs `Id`)
  4. For mocked interfaces, confirm method signatures match exactly
