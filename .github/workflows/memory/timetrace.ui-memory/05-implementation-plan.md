# Plan: Convert Figma Wireframe to WPF/XAML Screens

> **Context**: The detailed analysis backing this plan is stored in the sibling files in this folder (`memory/timetrace.ui-memory/`):
> - `01-existing-codebase-analysis.md` — Full existing WPF codebase documentation
> - `02-figma-design-analysis.md` — Complete Figma wireframe screen-by-screen breakdown
> - `03-library-models-and-repos.md` — timetrace.library entity models and repository interfaces
> - `04-gap-analysis.md` — Detailed gap analysis between current code and Figma design

---

The Figma "TimeTraceWireframe" defines 3 screens — **ProcessesList** (card grid), **AppDetail** (date-filtered carousel + grid gallery), and **Settings** (4 card sections). The existing WPF project has boilerplate Views/ViewModels/Services but they're structurally incomplete and visually misaligned with the Figma. This plan rewrites or extends each layer to match the Figma while preserving all existing infrastructure (tray, lightbox, busy indicator, navigation, DI, ViewLocator, converters, `PresentationFramework.Fluent` theming).

Mock data stays but models are aligned to `timetrace.library` entities so the backend team can remove mocks and inject real repositories with minimal structural change.

**Steps**

## Phase 1 — Models & Data Service (foundation)

1. **Update** `timetrace.ui/Models/ApplicationModel.cs` — Keep existing fields (`Id`, `ProcessName`, `ProcessId`, `Status`, `LastCaptured`, `IconPath`, `CaptureCount`, `IsRunning`, `ExtensionData`). Add a new `ImageCount` computed property. Rename nothing — these fields already align with `timetrace.library.Models.Process` (int `ProcessId`, string `Name`). Add `// TODO: Map from IProcessRepository.GetProcesses()` comments on every mock-populated field.

2. **Update** `timetrace.ui/Models/CapturedImageModel.cs` — Add an `ImageSource` property (type `BitmapImage` or `ImageSource`) for actual image rendering alongside existing `FilePath`. Add `ImageGuid` (Guid) to align with `timetrace.library.Models.Image.ImageGuid`. Add `// TODO: Map from IImageRepository / Image entity` comments.

3. **Update** `timetrace.ui/Services/IApplicationDataService.cs` — Add method `List<CapturedImageModel> GetCapturedImagesForProcess(int processId)` with semantics closer to `IProcessRepository.GetImagesForProcess(processId, page, pageSize)`. Keep existing `GetCapturedImages(int applicationId)` as an alias. Add `// TODO: Replace with IProcessRepository + IImageRepository calls` comment.

4. **Update** `timetrace.ui/Services/ApplicationDataService.cs` — Expand mock data to 6 apps matching Figma (Chrome, VS Code, Slack, Spotify, Excel, Outlook). Generate 4–6 `CapturedImageModel` entries per app with sample `FilePath` values and placeholder `ImageSource` (use a solid-color generated `BitmapImage` or `null` with an Image-missing fallback in XAML). Add `// TODO: Wire to IProcessRepository / IImageRepository` at class level.

5. **Update** `timetrace.ui/Models/FilterCriteria.cs` — Remove `Statuses` field (Figma has no status filter). Keep `FromDate`, `ToDate`. Remove `SelectedDate` (unused in Figma; the calendar is on-demand, not a separate selection). Mark as `// NOTE: Backend IProcessRepository.GetProcessDetailsByDateRange(startDate, endDate, page, pageSize) aligns to FromDate/ToDate`.

## Phase 2 — Navigation Sidebar (layout & visual overhaul)

6. **Rewrite** `timetrace.ui/Views/NavigationSidebar.xaml` — Reorganize into 3 rows: **header** (app title + icon), **nav items** (StackPanel), and **footer** (version text). Use Fluent theme colors (keep system brush tokens, do not hardcode dark colors). Add a styled **active indicator** (bold font weight or accent-colored left border) on the nav item matching the currently active view. Replace emoji icons (📱, ⚙️) with `Geometry` path data icons (monitor-screen and gear shapes) via `Path` elements. The collapse/expand toggle button becomes a circular button positioned at the sidebar edge (absolute-positioned via margin/alignment on the border). Sidebar footer shows "Windows Process Capture v1.0" when expanded, hidden when collapsed.

7. **Update** `timetrace.ui/ViewModels/MainViewModel.cs` — Add `ActiveNavItem` (string, "Applications" or "Settings") property. Set it in `NavigateToApplications()` / `NavigateToSettings()` and in the `Navigated` handler (for detail views, keep "Applications" active). Bind the sidebar active indicator to `ActiveNavItem`.

8. **Update** `timetrace.ui/Views/MainWindow.xaml` — Remove the hardcoded header `TextBlock` for `CurrentViewTitle` from the main content area. Each screen will own its own header internally (as per Figma, where "Currently Running Applications" and "Settings" headers are part of the respective screen content, not a global bar). Keep `ContentControl` bound to `CurrentView`. Keep all DataTemplate mappings. Adjust sidebar collapsed/expanded widths: collapsed `70`, expanded `240`.

## Phase 3 — Applications List Screen (card grid)

9. **Rewrite** `timetrace.ui/Views/ApplicationListView.xaml` — Replace `ListView` with a `ScrollViewer > ItemsControl` using `WrapPanel` as ItemsPanel (3 columns at 1200px, 2 at narrow). Each item becomes a card `Border` (CornerRadius 12, subtle border, hover shadow effect via Trigger) containing: an icon area (`Path` or emoji `TextBlock`), app name (`TextBlock Bold`), screenshot count with a camera icon (`Path` + text), and a chevron-right arrow. Use `InputBindings` + `MouseBinding` (left-click → `SelectApplicationCommand`) on each card. Add own header: "Currently Running Applications" title + "Click on any application to view captured screenshots" subtitle inside the view.

10. **Update** `timetrace.ui/ViewModels/ApplicationListViewModel.cs` — Remove `RefreshApplicationsCommand` from the button (Figma has no refresh button). Keep the command for programmatic use. The `SelectApplicationCommand` should pass the selected `ApplicationModel` to `ApplicationDetailsViewModel` before navigating (call `detailsViewModel.SetApplication(application)` — a new method).

## Phase 4 — Application Details Screen (header + filter + gallery)

11. **Rewrite** `timetrace.ui/Views/ApplicationDetailsView.xaml` — Two rows: **header bar** (row 0, auto) and **content area** (row 1, `*`). 
    - **Header**: back button (← Back), app icon + name, "N of M screenshots" counter, view-mode toggle (Carousel | Grid) as a pair of `RadioButton`-style toggle buttons in a `Border`. 
    - **Content**: two-columns: left filter panel (column 0, width 260) and right gallery area (column 1, `*`). Use `ContentControl` bound to `CurrentGalleryView` that swaps between `PresenterView` (grid) and a new `CarouselView` (carousel) based on the `ViewMode` property.

12. **Update** `timetrace.ui/ViewModels/ApplicationDetailsViewModel.cs` — Add:
    - `SetApplication(ApplicationModel app)` method — sets `Application`, loads images via `IApplicationDataService`, populates `AllImages` and `FilteredImages`.
    - `ViewMode` enum property (Carousel/Grid), default Carousel. Bound from header toggle.
    - `TotalImageCount` / `FilteredImageCount` computed properties.
    - `NavigateBackCommand` (calls `INavigationService.NavigateTo(ApplicationListViewModel)`).
    - Move image loading/filtering logic here (away from `PresenterViewModel`).
    - Wire `FilterViewModel.FilterChanged` to re-filter images.
    - `CurrentIndex` (int) for carousel position, `CurrentImage` (computed from `FilteredImages[CurrentIndex]`).
    - `NextImageCommand`, `PreviousImageCommand`, `GoToImageCommand(int index)`.

13. **Simplify** `timetrace.ui/ViewModels/FilterViewModel.cs` — Remove `SelectedDate`, `ShowActive`, `ShowMinimized`, `ShowClosed`, and `Statuses`. Keep only `FromDate`, `ToDate`. Keep `ApplyFilterCommand`, `ClearFilterCommand`. Add `HasActiveFilter` (bool, computed from `FromDate != null || ToDate != null`). Update `FilterChanged` event args to carry from/to values. Add `FilterSummary` (string, formatted date range) for the active filter badge.

14. **Rewrite** `timetrace.ui/Views/FilterView.xaml` — Match Figma's filter sidebar: 
    - Header with calendar icon + "Date Filter" label.
    - From `DatePicker` with label and clear (×) button.
    - To `DatePicker` with label and clear (×) button.
    - Active filter summary badge (blue background Border, visible when `HasActiveFilter` is true, shows `FilterSummary`).
    - "Clear Filters" button (visible when `HasActiveFilter`).
    - Bottom stats panel: "Total: N screenshots" / "Filtered: M results".

15. **Rewrite** `timetrace.ui/Views/PresenterView.xaml` — Grid gallery mode:
    - Remove ViewLocator (DataContext will be set from parent `ApplicationDetailsView`).
    - `ScrollViewer > ItemsControl` with `UniformGrid Columns="3"`.
    - Each card: `Border` with `Image` element (Source bound to `FilePath` or `ImageSource` with fallback), screenshot name, and formatted timestamp. Clicking a grid card switches to carousel mode at that index.

16. **Create new** `Views/CarouselView.xaml` + `Views/CarouselView.xaml.cs` — Carousel gallery mode:
    - Main area: dark background `Border` with centered `Image` (bound to `CurrentImage.ImageSource` or `CurrentImage.FilePath`) + prev/next overlay buttons.
    - Top center counter pill: "1 / N · filename.png".
    - Bottom-right timestamp badge.
    - Bottom thumbnail strip: horizontal `ListBox` of small `Image` thumbnails, `SelectedIndex` bound to `CurrentIndex`.
    - Range slider at bottom: `Slider` (min 0, max `FilteredImageCount - 1`, value bound to `CurrentIndex`).
    - Keyboard binding: `KeyBinding Key="Left" Command="{Binding PreviousImageCommand}"`, `Key="Right" Command="{Binding NextImageCommand}"`.

17. **Update** `timetrace.ui/ViewModels/PresenterViewModel.cs` — Simplify: remove `applicationId`, `dataService`, `ApplyFilter`, `LoadImages`. This ViewModel becomes a thin wrapper or is removed entirely, with `ApplicationDetailsViewModel` owning the `FilteredImages` collection and carousel state directly. If kept, it just proxies `FilteredImages` from the parent.

## Phase 5 — Settings Screen (4 card sections)

18. **Rewrite** `timetrace.ui/Views/SettingsView.xaml` — Match Figma's 4-card layout:
    - **Card 1 — Capture Settings**: "Automatic Capture" `ToggleSwitch`, "Capture Interval (minutes)" `TextBox` (type number), "Screenshot Quality" `ComboBox` (Low/Medium/High).
    - **Card 2 — Monitoring Settings**: "Monitor All Applications" `ToggleSwitch`, "Include Background Apps" `ToggleSwitch`, "Include System Apps" `ToggleSwitch`.
    - **Card 3 — Storage Settings**: "Storage Location" `TextBox` + Browse `Button`, "Retention Period (days)" `TextBox`, "Compress Screenshots" `ToggleSwitch`.
    - **Card 4 — Notifications**: "Notify on Screenshot Capture" `ToggleSwitch`, "Notify on New Application" `ToggleSwitch`.
    - Footer: "Reset to Defaults" + "Save Settings" buttons wired to commands.
    - Each card is a `Border` with CornerRadius, header `TextBlock` Bold, description `TextBlock` Gray, and grouped controls.
    - Own screen header: gear icon + "Settings" + description text.

19. **Update** `timetrace.ui/ViewModels/SettingsViewModel.cs` — Add all new properties: `AutoCapture` (bool), `CaptureIntervalMinutes` (int, default 5), `ScreenshotQuality` (string, "High"), `MonitorAll` (bool), `IncludeBackgroundApps` (bool), `IncludeSystemApps` (bool), `StoragePath` (string), `RetentionDays` (int, default 30), `CompressScreenshots` (bool), `NotifyOnCapture` (bool), `NotifyOnNewApp` (bool). Add `SaveSettingsCommand` and `ResetToDefaultsCommand` (with `// TODO: Persist via IConfigurationRepository.UpdateConfigurationSettingDetail()`). Add `BrowseStoragePathCommand` (opens `FolderBrowserDialog`).

## Phase 6 — New Converter + ResourceDictionary

20. **Create new** `Converters/NullToVisibilityConverter.cs` — Returns `Visible` when value is non-null, `Collapsed` when null. Needed for active filter badge and conditional UI elements.

21. **Create new** `Converters/ViewModeToVisibilityConverter.cs` — Compares bound `ViewMode` enum against parameter, returns `Visible` if matching, `Collapsed` otherwise. Used to swap Carousel/Grid views.

22. **Create new** `Converters/IntToVisibilityConverter.cs` — Returns `Visible` when int > 0. Used for "no screenshots" empty state.

23. **Update** `timetrace.ui/App.xaml` — Add any new global styles or resources needed (card style, toggle switch style) as a new `ResourceDictionary` (e.g., `Resources/CardStyles.xaml`). Keep existing merged dictionaries untouched.

## Phase 7 — DI & Wiring

24. **Update** `timetrace.ui/App.xaml.cs` — No changes to service registrations (keep `IApplicationDataService` → `ApplicationDataService` with mock data). Add `// TODO: Replace ApplicationDataService with real implementation using timetrace.library repositories` comment. Keep all existing registrations untouched.

25. **Update** `timetrace.ui/Helpers/ViewModelLocator.cs` — No changes needed (all 6 ViewModel types already registered).

26. **Update** `timetrace.ui/Helpers/ViewLocator.cs` — No changes needed unless a new ViewModel is added. Since `CarouselView` shares `ApplicationDetailsViewModel`, no new entry required.

## Phase 8 — Cleanup

27. Remove the `Refresh` button from `ApplicationListView` (not in Figma). Keep `RefreshApplicationsCommand` in the ViewModel for programmatic use.

28. On `NavigationSidebar`, ensure the sidebar header shows app icon image from `Resources/AppIcon/timeTrace.png` when expanded (using `Image` element, fallback to text "TT").

---

## Verification

- Build the solution (`dotnet build`) — zero errors.
- Launch the app: sidebar shows two nav items with active indicator; collapsed/expanded toggle works; footer shows version.
- **Applications screen**: 6 app cards in a responsive grid. Click a card → navigates to detail view.
- **Detail screen**: header shows back button, app name, image count, carousel/grid toggle. Left panel has From/To date pickers with filter summary. Carousel shows full image with navigation. Grid shows card thumbnails. Switching modes preserves index.
- **Settings screen**: 4 card sections with all toggles/inputs. Save/Reset buttons invoke commands (with TODO stubs).
- Exit → lightbox confirmation still works. Minimize to tray still works. Busy indicator overlay infrastructure untouched.
- All existing features (tray, lightbox, busy indicator, ViewLocator, converters) remain functional.

---

## Decisions

- **Carousel + Grid**: Both view modes implemented (user confirmed full Figma fidelity).
- **Mock data kept**: Models aligned with `timetrace.library` entity shapes and annotated with TODO for backend wiring. `IApplicationDataService` stays as the abstraction layer — swap implementation to call real repositories later.
- **Fluent theme preserved**: Don't override with Figma's hardcoded dark hex colors. Use fluent theming which is available in App.xaml Resources section. This is a fluent theme matching windows provided by microsoft for WPF apps, and it will automatically adapt to light/dark mode and provide consistent styling. Reference to microsoft documentation: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/whats-new/net90#:~:text=Application.Resources%3E,Application.Resources%3E
- **Status filter removed**: Figma has no status checkboxes. `FilterViewModel` simplified to From/To dates only.
- **PresenterViewModel simplified**: Carousel/gallery state owned by `ApplicationDetailsViewModel` to avoid split ownership of `CurrentIndex` and filtering across three ViewModels.
