# Lesson: Use Manual File Creation for Repository Self-Learning System

## Metadata
- PatternId: LESSON-003
- PatternVersion: 1
- Status: active
- Supersedes: none
- CreatedAt: 2026-03-31
- LastValidatedAt: 2026-03-31
- ValidationEvidence: store_memory tool failed with "repository may not exist" error; manual file creation to .github/Memories/ succeeded

## Task Context
- Triggering task: Implementing Avalonia UI for TimeTrace Linux, attempting to record Avalonia styling patterns
- Date/time: 2026-03-31
- Impacted area: Self-learning system (.github/Memories, .github/Lessons)

## Mistake
- What went wrong: Used the built-in `store_memory` tool to record Avalonia UI patterns
- Expected behavior: Memory would be stored and available for future reference
- Actual behavior: Tool returned error: "Unable to store memory. The repository may not exist or you may not have write access to it."

## Root Cause Analysis
- Primary cause: The `store_memory` tool is a built-in agent capability designed for a separate storage mechanism (internal agent memory or GitHub API), NOT for writing to the repository's `.github/Memories/` folder
- Contributing factors: 
  - Tool name (`store_memory`) suggests it would work with the repository's memory system
  - No clear documentation distinguishing between tool-based storage and repository-based storage
- Detection gap: Only discovered at runtime when tool failed; no pre-execution validation

## Resolution
- Fix implemented: Created memory file manually using the `create` tool, following the established template in `.github/Memories/`
- Why this fix works: Direct file creation bypasses the tool abstraction and writes directly to the repository filesystem where the self-learning system expects artifacts
- Verification performed: Successfully created `.github/Memories/004-avalonia-ui-patterns.md` and verified file exists

## Preventive Actions
- Guardrails added: This lesson documents the correct approach
- Tests/checks added: None (process lesson, not code)
- Process updates: 
  1. Always use `create` tool for `.github/Memories/` and `.github/Lessons/`
  2. Never rely on `store_memory` tool for this repository's self-learning system
  3. Follow the template structure from existing memory/lesson files

## Reuse Guidance
- How to apply this lesson in future tasks:
  1. When recording lessons or memories for this repository, ALWAYS use manual file creation:
     ```
     create tool → .github/Memories/NNN-descriptive-name.md
     create tool → .github/Lessons/NNN-descriptive-name.md
     ```
  2. Before creating, check existing files to determine next number (e.g., if 003 exists, use 004)
  3. Follow the exact template structure from `dotnet-delivery-architect` agent instructions
  4. Include all required metadata: PatternId, PatternVersion, Status, Supersedes, CreatedAt, LastValidatedAt, ValidationEvidence
  5. The `store_memory` built-in tool is for a DIFFERENT system and will fail silently or with generic errors

## Related Patterns
- MEMORY-004: Avalonia UI Patterns (created using this corrected approach)
