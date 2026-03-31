# Memory: Avalonia UI Patterns and Conventions

## Metadata
- PatternId: MEMORY-004
- PatternVersion: 1
- Status: active
- Supersedes: none
- CreatedAt: 2026-03-31
- LastValidatedAt: 2026-03-31
- ValidationEvidence: Implemented timetrace.ui.avalonia project, verified with dotnet build and runtime testing

## Source Context
- Triggering task: Implementing Avalonia-based Linux UI for TimeTrace
- Scope/system: timetrace.ui.avalonia
- Date/time: 2026-03-31

## Memory
- Key fact or decision: Avalonia 11.x UI framework patterns differ from WPF in key areas
- Why it matters: Prevents common build errors (AVLN2000) and runtime crashes

## Key Facts

### File Extensions
- Avalonia uses `.axaml` extension instead of WPF's `.xaml`
- Code-behind files use `.axaml.cs`

### Resource/Style Includes (Critical - AVLN2000 Error Prevention)
- **ResourceInclude**: Use for ResourceDictionary files (colors, brushes, static resources)
  ```xml
  <ResourceInclude Source="/Styles/Colors.axaml"/>
  ```
- **StyleInclude**: Use for IStyle files (control templates, style definitions)
  ```xml
  <StyleInclude Source="/Styles/Controls.axaml"/>
  ```
- **Mixing these causes AVLN2000 build errors**

### Animations (Runtime Crash Prevention)
- Avalonia uses CSS-like `Transitions` for simple animations
- Complex Storyboard-style animations require registered animators
- RenderTransform animations via KeyFrames crash unless animator is registered
- Safe approach: Use `Transitions` property or simple opacity/scale changes
  ```xml
  <Border.Transitions>
      <Transitions>
          <DoubleTransition Property="Opacity" Duration="0:0:0.2"/>
      </Transitions>
  </Border.Transitions>
  ```

### Style Selectors
- Uses CSS-like selector syntax instead of TargetType
  ```xml
  <Style Selector="Button.primary">
      <Setter Property="Background" Value="{DynamicResource AccentBrush}"/>
  </Style>
  ```

### MVVM with CommunityToolkit.Mvvm
- Works identically to WPF usage
- `[ObservableProperty]`, `[RelayCommand]` source generators work correctly
- Async commands: `[RelayCommand] private async Task RefreshAsync()` generates `RefreshCommand`

### Compiled Bindings
- Enable in project: `<AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>`
- Requires `x:DataType` on views for type-safe binding
  ```xml
  <UserControl x:DataType="vm:ApplicationListViewModel">
  ```

### Theme Switching
- Use `Application.RequestedThemeVariant = ThemeVariant.Dark | Light`
- Access via `Application.Current.RequestedThemeVariant`

## Applicability
- When to reuse: Any Avalonia UI development in this repository
- Preconditions/limitations: Avalonia 11.x specific; may differ in future versions

## Actionable Guidance
- Always use ResourceInclude for resource dictionaries, StyleInclude for styles
- Avoid complex RenderTransform animations; prefer Transitions
- Set x:DataType on all views for compiled binding support
- Test runtime behavior, not just build success

## Related Files
- `timetrace.ui.avalonia/App.axaml` - Application resources and theme setup
- `timetrace.ui.avalonia/Styles/Colors.axaml` - Color palette (ResourceDictionary)
- `timetrace.ui.avalonia/Styles/Controls.axaml` - Control styles (IStyle)
