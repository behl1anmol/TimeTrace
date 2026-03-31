# Lesson 005: Avalonia Does Not Support "CurrentColor"

## The Mistake

When using SVG Path elements in Avalonia, I used `Fill="CurrentColor"` expecting it to inherit the parent's foreground color:

```xml
<!-- WRONG - This causes a runtime crash -->
<Path Data="M12 4v16m-8-8h16" Fill="CurrentColor" />
```

## The Error

```
System.FormatException: Invalid brush string: 'CurrentColor'.
   at Avalonia.Media.Brush.Parse(String s)
```

## Why It Happened

`currentColor` is a **CSS/SVG concept** that means "use the current text color" - it's a relative color value that inherits from the parent element's `color` property.

**Avalonia XAML is not CSS/SVG** - it uses a different brush system. Valid brush strings in Avalonia are:
- Hex colors: `#FF0000`, `#AARRGGBB`
- Named colors: `Red`, `Blue`, `Transparent`
- Resource references: `{StaticResource MyBrush}`, `{DynamicResource MyBrush}`
- NOT: `CurrentColor`, `currentColor`, `inherit`

## The Fix

Replace `CurrentColor` with an explicit brush reference:

```xml
<!-- CORRECT - Use a StaticResource brush -->
<Path Data="M12 4v16m-8-8h16" 
      Fill="{StaticResource SidebarTextBrush}" />

<!-- CORRECT - Use a DynamicResource for theme-aware colors -->
<Path Data="M12 4v16m-8-8h16" 
      Fill="{DynamicResource SystemControlForegroundBaseHighBrush}" />

<!-- CORRECT - Use TemplateBinding inside control templates -->
<Path Data="M12 4v16m-8-8h16" 
      Fill="{TemplateBinding Foreground}" />

<!-- CORRECT - Use a direct color value -->
<Path Data="M12 4v16m-8-8h16" 
      Fill="#9CA3AF" />
```

## When Converting SVG to Avalonia Path

When converting web SVG icons to Avalonia XAML:

1. **Extract the `d` attribute** → becomes `Path.Data`
2. **Replace `fill="currentColor"`** → use `{StaticResource YourColorBrush}`
3. **Replace `stroke="currentColor"`** → use `Stroke="{StaticResource YourColorBrush}"`
4. **Wrap in Viewbox** for scaling: `<Viewbox Width="20" Height="20"><Path .../></Viewbox>`

## Prevention

- When copying SVG icons from web resources (Heroicons, Feather, etc.), always check for `currentColor` and replace it
- Define your icon colors as resources in `Colors.axaml` for consistency
- Use a search to find any `CurrentColor` before running: `grep -r "CurrentColor" *.axaml`

## Related Patterns

For icons that should match button foreground color dynamically (e.g., changing on hover), define a style that sets the Path fill based on button state:

```xml
<Style Selector="Button:pointerover Path">
    <Setter Property="Fill" Value="{StaticResource AccentBrush}"/>
</Style>
```
