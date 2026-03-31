# Lesson: Avalonia StaticResource Must Be Defined Before Usage

## Metadata
- PatternId: AVLN-RES-001
- PatternVersion: 1
- Status: active
- Supersedes: none
- CreatedAt: 2026-03-31
- LastValidatedAt: 2026-03-31
- ValidationEvidence: Fixed runtime KeyNotFoundException in TimeTrace Avalonia UI

## Task Context
- Triggering task: UI refresh for TimeTrace Avalonia application
- Date/time: 2026-03-31
- Impacted area: App.axaml resource and style initialization

## Mistake
- What went wrong: Runtime `KeyNotFoundException: Static resource 'SidebarTextBrush' not found`
- Expected behavior: App should start normally with styles applying correctly
- Actual behavior: App crashed during initialization when parsing `Application.Styles`

## Root Cause Analysis
- Primary cause: In Avalonia's App.axaml, `Application.Styles` was defined BEFORE `Application.Resources`. When XAML parser encountered `{StaticResource SidebarTextBrush}` in the styles, the resource hadn't been loaded yet.
- Contributing factors: The brushes were defined in Colors.axaml loaded via `ResourceInclude` in `MergedDictionaries`, which hadn't been processed yet.
- Detection gap: Build succeeded but runtime failed - this is a XAML parsing order issue, not a compile-time error.

## Resolution
- Fix implemented: Moved `Application.Resources` section ABOVE `Application.Styles` in App.axaml
- Why this fix works: Avalonia processes App.axaml sections in document order. By placing Resources first, all `StaticResource` references in Styles can resolve correctly.
- Verification performed: App now starts without exceptions

## Preventive Actions
- Guardrails added: Added comment in App.axaml: "IMPORTANT: Resources MUST come before Styles so StaticResource references resolve"
- Tests/checks added: None (this is a runtime initialization issue)
- Process updates: Always verify Avalonia apps run, not just build, when refactoring App.axaml

## Reuse Guidance
- How to apply this lesson in future tasks:
  1. In Avalonia App.axaml, always place `Application.Resources` BEFORE `Application.Styles`
  2. If styles reference resources from merged dictionaries, those dictionaries must be loaded first
  3. For resources that must be available before styles parse, define them inline in App.axaml Resources or ensure proper ordering
  4. Build success does NOT guarantee runtime success for XAML resource resolution issues
