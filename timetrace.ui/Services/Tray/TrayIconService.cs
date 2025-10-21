using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace timetrace.ui.Services.Tray;

/// <summary>
/// Implementation of the system tray icon service using Win32 API.
/// This class provides functionality for displaying and managing a system tray icon
/// without requiring Windows Forms references.
/// </summary>
public class TrayIconService : ITrayIconService
{
    #region Constants

    // Windows message constants
    private const int WM_TRAYICON = 0x800;      // Custom message for tray notifications
    private const int WM_LBUTTONUP = 0x0202;    // Left mouse button up
    private const int WM_RBUTTONUP = 0x0205;    // Right mouse button up

    // Shell_NotifyIcon dwMessage values
    private const int NIM_ADD = 0x0;            // Add icon to system tray
    private const int NIM_MODIFY = 0x1;         // Modify icon in system tray
    private const int NIM_DELETE = 0x2;         // Delete icon from system tray

    // NotifyIconData uFlags values
    private const int NIF_MESSAGE = 0x1;        // Message value is valid
    private const int NIF_ICON = 0x2;           // Icon handle is valid
    private const int NIF_TIP = 0x4;            // Tooltip string is valid
    private const int NIF_INFO = 0x10;          // Notification info is valid

    #endregion

    #region Private Fields

    private NotifyIconData _iconData;
    private Window? _mainWindow;
    private HwndSource? _hwndSource;
    private bool _isInitialized = false;
    private bool _isDisposed = false;

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the tray icon, hooks into the message loop, and sets up event handlers.
    /// </summary>
    public void Initialize()
    {
        if (_isInitialized || _isDisposed)
            return;

        _mainWindow = Application.Current.MainWindow;
        if (_mainWindow == null)
            throw new InvalidOperationException("Main window not found. Ensure the service is initialized after the main window is created.");

        // Create window handle for receiving messages
        var helper = new WindowInteropHelper(_mainWindow);

        // Setup notify icon data structure
        _iconData = new NotifyIconData
        {
            cbSize = Marshal.SizeOf(typeof(NotifyIconData)),
            hwnd = helper.Handle,
            uID = 1,
            uFlags = NIF_MESSAGE | NIF_ICON | NIF_TIP,
            uCallbackMessage = WM_TRAYICON,
            hIcon = GetIconHandle(),
            szTip = "TimeTrace Application",
            uVersion = 4
        };

        // Add icon to system tray
        var result = Shell_NotifyIcon(NIM_ADD, ref _iconData);
        if (!result)
        {
            var error = Marshal.GetLastWin32Error();
            throw new InvalidOperationException($"Failed to add icon to system tray. Error Code:{error}");
        }

        // Set up message hook for tray icon interaction
        _hwndSource = HwndSource.FromHwnd(_iconData.hwnd);
        _hwndSource.AddHook(WndProc);

        // Handle window state changes to minimize to tray
        _mainWindow.StateChanged += MainWindow_StateChanged;
        _mainWindow.Closing += MainWindow_StateChanged;

        _isInitialized = true;
    }

    /// <summary>
    /// Shows a notification balloon in the system tray.
    /// </summary>
    /// <param name="title">Title of the notification.</param>
    /// <param name="message">Content of the notification.</param>
    public void ShowNotification(string title, string message)
    {
        if (!_isInitialized || _isDisposed)
            return;

        _iconData.uFlags = NIF_INFO;
        _iconData.szInfoTitle = title;
        _iconData.szInfo = message;
        _iconData.dwInfoFlags = 0; // No icon

        Shell_NotifyIcon(NIM_MODIFY, ref _iconData);
    }

    /// <summary>
    /// Cleans up resources used by the tray icon service.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Disposes of resources used by this service.
    /// </summary>
    /// <param name="disposing">True if called from Dispose(), false if called from finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // Managed resources cleanup
                if (_isInitialized)
                {
                    // Remove the icon from the tray
                    Shell_NotifyIcon(NIM_DELETE, ref _iconData);

                    // Detach event handlers
                    if (_mainWindow != null)
                    {
                        _mainWindow.StateChanged -= MainWindow_StateChanged;
                    }

                    // Remove window hook
                    if (_hwndSource != null)
                    {
                        _hwndSource.RemoveHook(WndProc);
                        _hwndSource = null;
                    }
                }
            }

            // Unmanaged resources cleanup
            _isInitialized = false;
            _isDisposed = true;
        }
    }

    /// <summary>
    /// Event handler for window state changes.
    /// </summary>
    private void MainWindow_StateChanged(object? sender, EventArgs e)
    {
        // When window is minimized, hide it from the taskbar
        if (_mainWindow?.WindowState == WindowState.Minimized)
        {
            _mainWindow.Hide();
        }
    }

    /// <summary>
    /// Gets the icon handle for the tray icon.
    /// Attempts to load an icon from embedded resources, otherwise falls back to a system icon.
    /// </summary>
    private IntPtr GetIconHandle()
    {
        try
        {
            // Try to load the app icon from resources
            var uri = new Uri("pack://application:,,,/timetrace.ui;component/Resources/AppIcon/timeTrace.ico");
            var streamResourceInfo = Application.GetResourceStream(uri);

            if (streamResourceInfo != null)
            {
                using var stream = streamResourceInfo.Stream;

                // Create a bitmap from the stream
                var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.DecodePixelWidth = 32; // Set size for the tray icon
                bitmap.DecodePixelHeight = 32;
                bitmap.EndInit();

                // Convert BitmapImage to bitmap and create icon
                var bitmapSource = bitmap;
                var bmp = new System.Drawing.Bitmap(
                    bitmapSource.PixelWidth,
                    bitmapSource.PixelHeight,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                var data = CopyPixels(bitmapSource);
                var bmpData = bmp.LockBits(
                    new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                Marshal.Copy(data, 0, bmpData.Scan0, data.Length);
                bmp.UnlockBits(bmpData);

                // Create icon from bitmap
                var iconHandle = bmp.GetHicon();
                return iconHandle;
            }
        }
        catch
        {
            // If anything fails, use the default application icon
        }

        // Use default Windows application icon as fallback
        return LoadIcon(IntPtr.Zero, new IntPtr(32512)); // IDI_APPLICATION
    }

    // Add this extension method for BitmapSource
    private static byte[] CopyPixels(System.Windows.Media.Imaging.BitmapSource source)
    {
        var stride = source.PixelWidth * ((source.Format.BitsPerPixel + 7) / 8);
        var bytes = new byte[stride * source.PixelHeight];
        source.CopyPixels(bytes, stride, 0);
        return bytes;
    }

    /// <summary>
    /// Creates an icon handle from a resource stream.
    /// </summary>
    /// <param name="stream">Stream containing icon data.</param>
    /// <returns>Handle to the created icon.</returns>
    private IntPtr CreateIconFromResourceStream(System.IO.Stream stream)
    {
        // Read the icon file into memory
        byte[] iconData = new byte[stream.Length];
        stream.Read(iconData, 0, iconData.Length);

        // Create the icon from the memory image
        return CreateIconFromResourceEx(
            iconData,
            (uint)iconData.Length,
            true,
            0x00030000, // Version 3
            128, // Desired width
            128, // Desired height
            0   // Default color format
        );
    }

    [DllImport("user32.dll")]
    private static extern IntPtr CreateIconFromResourceEx(
        byte[] presbits,
        uint dwResSize,
        bool fIcon,
        uint dwVer,
        int cxDesired,
        int cyDesired,
        uint Flags);

    /// <summary>
    /// Window procedure hook that receives window messages.
    /// Processes messages from the tray icon.
    /// </summary>
    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        // Check if the message is from our tray icon
        if (msg == WM_TRAYICON && wParam.ToInt32() == _iconData.uID)
        {
            int mouseMsg = lParam.ToInt32() & 0xFFFF;

            switch (mouseMsg)
            {
                case WM_LBUTTONUP:
                    // Left click - restore window
                    if (_mainWindow != null)
                    {
                        _mainWindow.Show();
                        _mainWindow.WindowState = WindowState.Normal;
                        _mainWindow.Activate();
                    }
                    handled = true;
                    break;

                case WM_RBUTTONUP:
                    // Right click - show context menu
                    ShowContextMenu();
                    handled = true;
                    break;
            }
        }
        return IntPtr.Zero;
    }

    /// <summary>
    /// Displays a context menu for the tray icon.
    /// </summary>
    private void ShowContextMenu()
    {
        var contextMenu = new System.Windows.Controls.ContextMenu();

        // Open application menu item
        var openItem = new System.Windows.Controls.MenuItem { Header = "Open TimeTrace" };
        openItem.Click += (s, e) =>
        {
            if (_mainWindow != null)
            {
                _mainWindow.Show();
                _mainWindow.WindowState = WindowState.Normal;
                _mainWindow.Activate();
            }
        };

        // Exit application menu item
        var exitItem = new System.Windows.Controls.MenuItem { Header = "Exit" };
        exitItem.Click += (s, e) =>
        {
            Application.Current.Shutdown();
        };

        // Add items to the context menu
        contextMenu.Items.Add(openItem);
        contextMenu.Items.Add(new System.Windows.Controls.Separator());
        contextMenu.Items.Add(exitItem);

        // Position and show the context menu
        contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
        contextMenu.IsOpen = true;
    }

    #endregion

    #region P/Invoke

    /// <summary>
    /// Sends a message to the system to add, modify, or delete a tray icon.
    /// </summary>
    [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool Shell_NotifyIcon(uint dwMessage, ref NotifyIconData lpData);

    /// <summary>
    /// Loads a standard system icon.
    /// </summary>
    [DllImport("user32.dll")]
    private static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);

    /// <summary>
    /// Data structure for interacting with the system tray API.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NotifyIconData
    {
        public int cbSize;                  // Size of structure in bytes
        public IntPtr hwnd;                 // Handle to the window that receives notification messages
        public int uID;                     // Application-defined identifier of the taskbar icon
        public int uFlags;                  // Flags that indicate which members contain valid data
        public int uCallbackMessage;        // Application-defined message ID
        public IntPtr hIcon;                // Handle to the icon to display
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szTip;               // Tooltip text
        public int dwState;                 // State of the icon
        public int dwStateMask;             // A value that specifies which bits of the dwState member are valid
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szInfo;              // Text for a balloon tooltip
        public int uTimeout;                // Timeout for the balloon tooltip
        public int uVersion;                // Version of the notification
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szInfoTitle;         // Title for a balloon tooltip
        public int dwInfoFlags;             // Flags that can be set to add an icon to a balloon tooltip
        public Guid guidItem;               // GUID for the icon
    }

    #endregion
}