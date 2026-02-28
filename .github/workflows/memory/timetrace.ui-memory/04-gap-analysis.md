# Gap Analysis: Existing WPF Code vs. Figma Design

This document catalogs every difference between the current WPF boilerplate and the Figma wireframe, organized by screen/area.

---

## 1. Data Layer

| # | Gap | Current State | Figma Target |
|---|-----|--------------|--------------|
| 1.1 | Mock data coverage | 3 apps, 2 images | 6 apps, 25+ images |
| 1.2 | Real data not connected | `ApplicationDataService` returns hard-coded lists | Library repos (`IProcessRepository`, `IImageRepository`) exist but are never registered in UI DI |
| 1.3 | Model fields mismatch | `ApplicationModel` has `Status`, `ProcessId`, `IsRunning`, `LastCaptured`, `ExtensionData` — none of which exist in Figma mock | Figma `Process` has only `id`, `name`, `icon`, `captureCount` |
| 1.4 | No image rendering | `CapturedImageModel` has `FilePath` but never loads an actual image (emoji placeholder only) | Figma displays actual images from URLs |
| 1.5 | Missing image GUID | `CapturedImageModel` lacks `ImageGuid` | Library entity `Image` has `ImageGuid` (Guid) |

## 2. Navigation Sidebar

| # | Gap | Current State | Figma Target |
|---|-----|--------------|--------------|
| 2.1 | Styling | Light theme, transparent buttons, emoji icons (📱, ⚙️) | Dark bg (`zinc-800`), Lucide icons, blue active state |
| 2.2 | Active state indicator | No visual distinction for active nav item | Blue background highlight on active item |
| 2.3 | Collapse toggle position | Regular button inside header grid | Circular button positioned at sidebar edge (mid-height) |
| 2.4 | Footer | No footer | "Windows Process Capture v1.0" text |
| 2.5 | Header | "TimeTrace" text only | "Process Monitor" text |
| 2.6 | Collapsed icon | Uses same emoji | Shows Monitor icon in header |
| 2.7 | Nav item labels | "Applications", "Settings" | "Running Processes", "Settings" |

## 3. Applications List Screen

| # | Gap | Current State | Figma Target |
|---|-----|--------------|--------------|
| 3.1 | Layout | Flat `ListView` with rows | Responsive card grid (1/2/3 columns) |
| 3.2 | Card content | Icon, name, status badge, last captured time, capture count, PID | Icon, name, capture count with camera icon, chevron arrow |
| 3.3 | Header | "Captured Applications" (global header elsewhere) | "Currently Running Applications" + subtitle (inside view) |
| 3.4 | Refresh button | Present | Not in Figma |
| 3.5 | Card interaction | ListView selection + MouseLeftButtonUp | Card click → navigate |
| 3.6 | Status badge | Colored text per status | Not present in Figma card |

## 4. Application Details Screen

| # | Gap | Current State | Figma Target |
|---|-----|--------------|--------------|
| 4.1 | Header bar | None (just "Captured Images" text in PresenterView) | Back button, app icon + name, "N of M screenshots", view mode toggle |
| 4.2 | View modes | Grid only (UniformGrid placeholder) | Carousel (default) + Grid toggle |
| 4.3 | Carousel view | Does not exist | Full image viewer with prev/next, counter pill, thumbnail strip, range slider, keyboard nav |
| 4.4 | Grid cards | Emoji placeholder, name, timestamp, file size, process status | Actual image, name, timestamp |
| 4.5 | Filter sidebar width | 300px | 260px (w-64) |
| 4.6 | Status filter | 3 checkboxes (Active, Minimized, Closed) | Not present in Figma |
| 4.7 | Calendar view | Full `Calendar` control + `SelectedDate` | No standalone calendar — From/To DatePickers with popover calendars |
| 4.8 | Filter summary badge | Does not exist | Blue badge showing active filter dates |
| 4.9 | Stats footer | Does not exist | "Total: N" / "Filtered: M" at bottom of filter panel |
| 4.10 | Empty state | Does not exist | Icon + contextual message |
| 4.11 | Hard-coded app ID | `PresenterViewModel.applicationId = 1` | Dynamic per-app navigation |

## 5. Settings Screen

| # | Gap | Current State | Figma Target |
|---|-----|--------------|--------------|
| 5.1 | Layout | Simple stacked controls (3 items) | 4 card sections with headers/descriptions |
| 5.2 | Capture settings | Only `AutoStart` checkbox | Auto Capture toggle + Capture Interval (number) + Screenshot Quality (dropdown) |
| 5.3 | Monitoring settings | Does not exist | Monitor All / Include Background / Include System Apps toggles |
| 5.4 | Storage settings | Only `ScreenshotsPath` textbox | Storage Location + Browse button + Retention Period + Compress toggle |
| 5.5 | Notification settings | Only `NotifyOnNewCapture` checkbox | Notify on Capture + Notify on New App toggles |
| 5.6 | Toggle controls | `CheckBox` | `ToggleSwitch` (Fluent theme) or `CheckBox` styled appropriately |
| 5.7 | Save/Reset commands | Buttons exist but have NO Command bindings | Buttons should be wired to `SaveSettingsCommand` / `ResetToDefaultsCommand` |
| 5.8 | Browse button | Does not exist | Should open `FolderBrowserDialog` |
| 5.9 | Screen header | "Settings" text (in global header) | Settings icon + "Settings" + description (inside view) |

## 6. Global / Cross-Cutting

| # | Gap | Current State | Figma Target |
|---|-----|--------------|--------------|
| 6.1 | Global header | `TextBlock` bound to `CurrentViewTitle` in MainWindow | Each screen owns its own header (no global header bar) |
| 6.2 | Sidebar widths | Collapsed 90px, expanded 250px | Collapsed ~64–70px, expanded ~240–256px |
| 6.3 | PresenterViewModel ownership | Owns images, filtering, applicationId | Should be simplified; ApplicationDetailsViewModel should own image state |

---

## Summary of Required Changes

### Files to CREATE (new)
- `Views/CarouselView.xaml` + `.xaml.cs`
- `Converters/NullToVisibilityConverter.cs`
- `Converters/ViewModeToVisibilityConverter.cs`
- `Converters/IntToVisibilityConverter.cs`
- `Resources/CardStyles.xaml` (optional, for shared card styling)

### Files to REWRITE (major overhaul)
- `Views/NavigationSidebar.xaml`
- `Views/ApplicationListView.xaml`
- `Views/ApplicationDetailsView.xaml`
- `Views/FilterView.xaml`
- `Views/PresenterView.xaml`
- `Views/SettingsView.xaml`

### Files to UPDATE (structural changes)
- `ViewModels/MainViewModel.cs` — add `ActiveNavItem`
- `ViewModels/ApplicationDetailsViewModel.cs` — add `SetApplication`, `ViewMode`, carousel state, `NavigateBackCommand`
- `ViewModels/FilterViewModel.cs` — simplify to From/To only, add `HasActiveFilter`, `FilterSummary`
- `ViewModels/PresenterViewModel.cs` — simplify (remove data ownership)
- `ViewModels/SettingsViewModel.cs` — add all new properties and commands
- `Models/ApplicationModel.cs` — add `ImageCount`
- `Models/CapturedImageModel.cs` — add `ImageSource`, `ImageGuid`
- `Models/FilterCriteria.cs` — simplify to From/To only
- `Services/IApplicationDataService.cs` — add `GetCapturedImagesForProcess`
- `Services/ApplicationDataService.cs` — expand mock data to 6 apps
- `Views/MainWindow.xaml` — remove global header, adjust widths

### Files NOT to touch
- `App.xaml` / `App.xaml.cs` (keep Fluent theme, DI, tray init)
- `Controls/BusyIndicator/*`
- `Controls/Lightbox/*`
- `Services/Tray/*`
- `Services/NavigationService.cs` / `INavigationService.cs`
- `Helpers/ViewLocator.cs` / `ViewModelLocator.cs`
- `Converters/BooleanToGridLengthConverter.cs`
- `Converters/BooleanToStringConverter.cs`
- `Converters/BooleanToVisibilityConverter.cs`
- `Converters/StatusToBrushConverter.cs`
- `Views/MainWindow.xaml.cs` (lightbox exit confirmation)
- `Resources/TrayIconResources.xaml`
