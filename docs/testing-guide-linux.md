# TimeTrace Avalonia Linux UI - Testing Guide

## Overview

This document provides testing procedures for the TimeTrace Avalonia Linux UI (`timetrace.ui.avalonia`).

**Version**: Pre-release Alpha  
**Target Platforms**: Linux x64, Linux ARM64

---

## Prerequisites

### System Requirements
- .NET 9.0 Runtime
- X11 or Wayland display server
- One of: Ubuntu 22.04+, Fedora 38+, Arch Linux, or compatible distro

### Build Requirements
- .NET 9.0 SDK
- Git

---

## Build Instructions

```bash
# Clone repository
git clone https://github.com/user/TimeTrace.git
cd TimeTrace

# Build the Avalonia project
dotnet build timetrace.ui.avalonia/timetrace.ui.avalonia.csproj -c Release

# Run the application
dotnet run --project timetrace.ui.avalonia/timetrace.ui.avalonia.csproj
```

---

## Manual Test Checklist

### 1. Application Launch
| Test | Expected | Status |
|------|----------|--------|
| App launches without errors | Window appears with sidebar | ☐ |
| Dark theme applied by default | Dark background colors | ☐ |
| Window resizes correctly | Content adjusts, no clipping | ☐ |
| Window minimizes/maximizes | Standard window behavior | ☐ |

### 2. Navigation
| Test | Expected | Status |
|------|----------|--------|
| Click "Applications" nav button | Shows application list view | ☐ |
| Click "Settings" nav button | Shows settings view | ☐ |
| Sidebar collapse button works | Sidebar width changes 240px ↔ 70px | ☐ |
| Escape key returns from details | Returns to application list | ☐ |

### 3. Application List View
| Test | Expected | Status |
|------|----------|--------|
| Mock applications displayed | 6 application cards visible | ☐ |
| Cards show correct info | Name, icon, status, screenshot count | ☐ |
| Hover effect on cards | Visual feedback on hover | ☐ |
| Click card opens details | Details view for that app | ☐ |
| Refresh button works | No crash, data reloads | ☐ |
| F5 triggers refresh | Same as refresh button | ☐ |

### 4. Application Details View
| Test | Expected | Status |
|------|----------|--------|
| Header shows app info | Name and back button visible | ☐ |
| Filter panel visible | Date pickers present | ☐ |
| Carousel view default | Image carousel with navigation | ☐ |
| Grid view toggle | Switches to grid layout | ☐ |
| Left/Right arrows work | Navigate between images | ☐ |
| Keyboard ←/→ navigation | Same as arrow buttons | ☐ |
| Thumbnail strip scrolls | Horizontal scroll for many images | ☐ |
| Image counter updates | Shows "X / Y" correctly | ☐ |

### 5. Filter Functionality
| Test | Expected | Status |
|------|----------|--------|
| From date picker opens | Calendar popup appears | ☐ |
| To date picker opens | Calendar popup appears | ☐ |
| Date selection filters images | Image list updates | ☐ |
| Clear filter works | All images shown again | ☐ |

### 6. Settings View
| Test | Expected | Status |
|------|----------|--------|
| Settings cards displayed | Multiple setting sections | ☐ |
| Toggle switches work | Visual state changes | ☐ |
| Theme toggle (dark/light) | App theme changes immediately | ☐ |
| Slider for interval | Value changes | ☐ |
| Quality dropdown | Options selectable | ☐ |
| Save button present | No crash on click | ☐ |
| Reset button works | Values return to defaults | ☐ |

### 7. Theme Switching
| Test | Expected | Status |
|------|----------|--------|
| Switch to light theme | All colors update | ☐ |
| Switch back to dark theme | Returns to dark colors | ☐ |
| Theme persists in session | Stays after navigation | ☐ |

### 8. Display Server Compatibility
| Test | Expected | Status |
|------|----------|--------|
| X11: App launches | Full functionality | ☐ |
| X11: Window decorations | Correct appearance | ☐ |
| Wayland: App launches | Full functionality | ☐ |
| Wayland: Window decorations | Correct appearance | ☐ |
| XWayland fallback | Works if native fails | ☐ |

---

## Distribution Testing Matrix

| Distro | Version | X11 | Wayland | Notes |
|--------|---------|-----|---------|-------|
| Ubuntu | 22.04 LTS | ☐ | ☐ | |
| Ubuntu | 24.04 LTS | ☐ | ☐ | |
| Fedora | 39 | ☐ | ☐ | |
| Fedora | 40 | ☐ | ☐ | |
| Arch Linux | Rolling | ☐ | ☐ | |
| Linux Mint | 21.x | ☐ | ☐ | |
| Debian | 12 | ☐ | ☐ | |
| openSUSE | Tumbleweed | ☐ | ☐ | |

---

## Known Issues

1. **Tray Icon**: Placeholder implementation - requires libayatana-appindicator for full functionality
2. **Image Loading**: Currently shows placeholder; requires actual screenshot files to test real loading

---

## Performance Benchmarks

Run these tests to establish baseline performance:

```bash
# Memory usage at startup
ps aux | grep timetrace

# CPU usage during idle
top -p $(pgrep -f timetrace.ui.avalonia)

# Startup time
time dotnet run --project timetrace.ui.avalonia/timetrace.ui.avalonia.csproj
```

---

## Reporting Issues

When reporting issues, include:
1. Linux distribution and version
2. Display server (X11/Wayland)
3. Desktop environment (GNOME, KDE, XFCE, etc.)
4. .NET version (`dotnet --version`)
5. Steps to reproduce
6. Screenshots if applicable
7. Console output/errors
