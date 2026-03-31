# Memory: Avalonia Theme-Aware Styling with Light/Dark Mode

## Pattern: Using ThemeDictionaries for Theme-Aware Resources

### Problem
In Avalonia, you need brushes that automatically change colors when switching between Light and Dark themes. Using static brushes doesn't respond to theme changes.

### Solution
Use `ResourceDictionary.ThemeDictionaries` in App.axaml to define theme-specific brushes:

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.ThemeDictionaries>
            <!-- Light Theme -->
            <ResourceDictionary x:Key="Light">
                <SolidColorBrush x:Key="ContentBackgroundBrush" Color="#F4F4F5"/>
                <SolidColorBrush x:Key="CardBackgroundBrush" Color="#FFFFFF"/>
                <SolidColorBrush x:Key="TextPrimaryBrush" Color="#18181B"/>
            </ResourceDictionary>
            
            <!-- Dark Theme -->
            <ResourceDictionary x:Key="Dark">
                <SolidColorBrush x:Key="ContentBackgroundBrush" Color="#09090B"/>
                <SolidColorBrush x:Key="CardBackgroundBrush" Color="#18181B"/>
                <SolidColorBrush x:Key="TextPrimaryBrush" Color="#FAFAFA"/>
            </ResourceDictionary>
        </ResourceDictionary.ThemeDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

### Usage in Views
Reference with `{DynamicResource}`:
```xml
<Border Background="{DynamicResource CardBackgroundBrush}">
    <TextBlock Foreground="{DynamicResource TextPrimaryBrush}"/>
</Border>
```

### Enabling System Theme Detection
Set `RequestedThemeVariant="Default"` in App.axaml:
```xml
<Application RequestedThemeVariant="Default">
```
This makes the app follow the system's light/dark preference automatically.

### Static Resources for Non-Theme Elements
For elements that don't change with theme (like always-dark sidebar), use `StaticResource`:
```xml
<Border Background="{StaticResource SidebarBackgroundBrush}"/>
```

## Key Differences
| Scenario | Resource Type |
|----------|---------------|
| Theme-aware content | `{DynamicResource}` |
| Fixed colors (sidebar always dark) | `{StaticResource}` |
| Status colors (always same) | `{StaticResource}` |

## Related Files
- `App.axaml` - ThemeDictionaries definition
- `Styles/Colors.axaml` - Static color definitions
- `Services/ThemeService.cs` - Theme switching logic
