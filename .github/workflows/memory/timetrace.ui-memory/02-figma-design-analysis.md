# Figma Design Analysis — TimeTraceWireframe

**Figma URL**: `https://www.figma.com/make/8zeDHjwl0gm22bD1fnoN3d/TimeTraceWireframe`
**File Key**: `8zeDHjwl0gm22bD1fnoN3d`

The Figma Make file is a React + Tailwind + Radix UI prototype defining 3 screens for a Windows Process Capture / Monitor application.

---

## Routing Structure (routes.ts)

```
/ → Root (layout shell)
  index → ProcessesList
  /app/:appId → AppDetail
  /settings → Settings
```

---

## Screen 1: Root Layout (Root.tsx)

**Description**: App shell with collapsible dark sidebar + main content area.

### Sidebar
- Dark background (`bg-zinc-800`, `text-white`)
- Collapsed width: `w-16` (64px). Expanded width: `w-64` (256px)
- **Header**: "Process Monitor" text (hidden when collapsed), Monitor icon when collapsed
- **Nav Items** (2 items):
  - "Running Processes" with Monitor icon
  - "Settings" with Settings icon
  - Active state: `bg-blue-600 text-white` (blue highlight)
  - Inactive state: `text-zinc-300 hover:bg-zinc-700`
- **Footer**: "Windows Process Capture v1.0" (hidden when collapsed)
- **Collapse Toggle**: Circular button at sidebar edge (mid-height), shows ChevronLeft/ChevronRight

### Main Content
- `flex-1 overflow-hidden`
- Renders `<Outlet />` (child routes)
- Background: `bg-zinc-100` (light gray)

---

## Screen 2: ProcessesList (ProcessesList.tsx)

**Description**: Card grid of tracked applications.

### Layout
- Full-height scrollable area with `bg-zinc-100`
- Padding: `p-8`

### Header
- Title: "Applications" (`h2`, `mb-2`)
- Subtitle: "Click on any application to view captured screenshots" (`text-zinc-600`)

### Card Grid
- `grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-4`
- Responsive: 1 column mobile, 2 columns medium, 3 columns large

### Each Card
- White background, `border-zinc-200`, hover shadow
- **Card Header**: Row with app icon (emoji, 4xl), app name (lg font), ChevronRight arrow
- **Card Content**: Camera icon + "{count} screenshots captured" text
- Click → navigates to `/app/{process.id}`

### Mock Data (6 processes)
| id | name | icon | captureCount |
|----|------|------|-------------|
| chrome | Google Chrome | 🌐 | 24 |
| vscode | Visual Studio Code | 💻 | 18 |
| slack | Slack | 💬 | 12 |
| spotify | Spotify | 🎵 | 8 |
| excel | Microsoft Excel | 📊 | 15 |
| outlook | Microsoft Outlook | 📧 | 20 |

---

## Screen 3: AppDetail (AppDetail.tsx)

**Description**: Detail view for a single application with date-filtered screenshot gallery in carousel or grid mode.

### Header Bar
- **Back button**: ghost variant, "← Back", navigates to `/`
- **App info**: emoji icon (2xl) + app name (h2) + "N of M screenshots" counter
- **View mode toggle**: Two buttons in a `bg-zinc-100 rounded-lg` container
  - "Carousel" with GalleryHorizontal icon (default)
  - "Grid" with LayoutGrid icon
  - Active mode: `bg-white text-zinc-900 shadow-sm`

### Left Sidebar — Filter Panel (w-64, 256px)
- `border-r border-zinc-200 bg-zinc-50`

#### Date Filter Section
- Header: Calendar icon + "Date Filter" label
- **From DatePicker**: Label "FROM", input with CalendarIcon, formatted date or placeholder "Pick start date…", X button to clear
- **To DatePicker**: Label "TO", same pattern, placeholder "Pick end date…"
- Both have popover `Calendar` (month picker with day grid)
- Cross-validation: From can't be after To, To can't be before From

#### Active Filter Summary
- Blue background badge (`bg-blue-50 border-blue-100 text-blue-800`)
- Shows "From: **date**" and/or "To: **date**"
- Only visible when a filter is active

#### Clear Filters Button
- `variant="outline" size="sm" w-full`
- Only visible when a filter is active

#### Stats Footer
- Bottom panel with `border-t border-zinc-200 bg-white`
- "Total: N screenshots" / "Filtered: M results"

### Right Area — Gallery

#### Carousel View (default)
- **Main image area**: `bg-zinc-900` dark, centered `<img>` with object-contain
  - Prev/Next buttons: circular, `bg-black/50`, positioned absolute left/right center
  - **Counter pill** (top center): "1 / N · filename.png" on `bg-black/60` rounded-full
  - **Timestamp badge** (bottom right): `bg-black/60` rounded-full, formatted date
  - **Navigation hint** (bottom left): "Scroll or use ← → keys to navigate"
- **Thumbnail strip** (bottom): horizontal scrollable row of 96x64px thumbnails
  - Active thumbnail: `border-blue-400 ring-2 ring-blue-400/40 opacity-100`
  - Inactive: `border-transparent opacity-50 hover:opacity-80`
  - Mouse wheel on strip → horizontal scroll
- **Range slider**: `input[range]` with accent-blue-400, shows "1" to "N" labels

#### Keyboard / Mouse Navigation
- Arrow keys Left/Right → prev/next image
- Mouse wheel on main image → prev/next
- Mouse wheel on thumbnail strip → horizontal scroll
- Click thumbnail → jump to that index

#### Grid View
- Header: "Screenshot Gallery" + "Captured images with timestamps"
- `grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5`
- Each card: aspect-video image (hover scale 1.05), name, timestamp
- Click card → switches to carousel at that index

#### Empty State
- Image icon (large), "No screenshots found"
- Contextual message: filter active → "Try adjusting or clearing the date filter" / else → "No captures available for this application"

---

## Screen 4: Settings (Settings.tsx)

**Description**: Configuration page with 4 card sections.

### Header
- Settings icon (w-6 h-6) + "Settings" (h2)
- "Configure application capture and monitoring preferences" subtitle

### Card Layout
- `max-w-4xl space-y-6` — stacked vertically, max width constrained

### Card 1 — Capture Settings
- Title: "Capture Settings"
- Description: "Configure how and when screenshots are captured"
- **Automatic Capture**: Switch toggle, defaultChecked, label + description
- **Capture Interval (minutes)**: Number input, defaultValue "5", max-w-xs
- **Screenshot Quality**: Select dropdown (Low (Fast) / Medium (Balanced) / High (Best Quality)), default "high"

### Card 2 — Monitoring Settings
- Title: "Monitoring Settings"
- Description: "Choose which applications to monitor"
- **Monitor All Applications**: Switch, defaultChecked
- **Include Background Apps**: Switch, unchecked
- **Include System Apps**: Switch, unchecked

### Card 3 — Storage Settings
- Title: "Storage Settings"
- Description: "Manage screenshot storage and retention"
- **Storage Location**: Text input + "Browse" button (flex row)
- **Retention Period (days)**: Number input, default 30 + description text
- **Compress Screenshots**: Switch, defaultChecked

### Card 4 — Notifications
- Title: "Notifications"
- Description: "Configure notification preferences"
- **Notify on Screenshot Capture**: Switch, unchecked
- **Notify on New Application**: Switch, defaultChecked

### Footer Buttons
- "Reset to Defaults" (variant="outline") + "Save Settings" (primary, with Save icon)

---

## Mock Data Types

### Process
```typescript
interface Process {
  id: string;        // e.g. "chrome"
  name: string;      // e.g. "Google Chrome"
  icon: string;      // emoji e.g. "🌐"
  captureCount: number;
}
```

### Screenshot
```typescript
interface Screenshot {
  id: string;
  appId: string;     // FK to Process.id
  name: string;      // filename e.g. "Dashboard_View.png"
  date: Date;
  imageUrl: string;  // Unsplash placeholder URL
}
```

### Screenshot Distribution
- Chrome: 20 screenshots (c1–c20), dates Feb 26–28 2026
- VS Code: 6 screenshots (v1–v6), dates Feb 26–28
- Slack: 3 screenshots (s1–s3), dates Feb 26–28
- Spotify: 0 screenshots in mock data (captureCount says 8)
- Excel: 3 screenshots (e1–e3), dates Feb 27–28
- Outlook: 0 screenshots in mock data (captureCount says 20)

---

## Design Token Summary (from Figma TSX)

### Colors (Tailwind zinc palette — for reference only, use WPF Fluent theme tokens)
- Sidebar bg: `zinc-800` (#27272a)
- Active nav: `blue-600` (#2563eb)
- Main bg: `zinc-100` (#f4f4f5)
- Card bg: white
- Card border: `zinc-200` (#e4e4e7)
- Filter sidebar bg: `zinc-50` (#fafafa)
- Carousel bg: `zinc-900` (#18181b)
- Filter badge: `blue-50` bg, `blue-100` border, `blue-800` text
- Text primary: `zinc-900`
- Text secondary: `zinc-500` / `zinc-600`
- Text muted: `zinc-400`

### Spacing
- Page padding: `p-8` (32px)
- Card gap: `gap-4` (16px) for process list, `gap-5` (20px) for grid gallery
- Margin between sections: `space-y-6` (24px)

### Typography
- h2: default large heading
- Controls: `text-sm` (14px)
- Descriptions: `text-sm text-zinc-500`
- Labels: `text-xs font-medium uppercase tracking-wide`

### Border Radius
- Cards: implicit (Radix Card component, typically 8–12px)
- Buttons: `rounded-lg` (8px), `rounded-md` (6px)
- Counter pill / badges: `rounded-full`
- Thumbnails: `rounded` (4px)
