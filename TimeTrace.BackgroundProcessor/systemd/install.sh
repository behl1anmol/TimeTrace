#!/bin/bash
# TimeTrace Background Processor Installation Script for Linux

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
INSTALL_DIR="$HOME/.local/bin"
SYSTEMD_USER_DIR="$HOME/.config/systemd/user"
SERVICE_NAME="timetrace-capture"

echo "TimeTrace Background Processor Installer"
echo "========================================="
echo ""

# Check if running on Linux
if [[ "$(uname)" != "Linux" ]]; then
    echo "Error: This script is for Linux only."
    exit 1
fi

# Create directories
echo "Creating directories..."
mkdir -p "$INSTALL_DIR"
mkdir -p "$SYSTEMD_USER_DIR"
mkdir -p "$HOME/.local/share/timetrace/captures"

# Build the project
echo "Building TimeTrace.BackgroundProcessor..."
cd "$SCRIPT_DIR/.."
dotnet publish -c Release -r linux-x64 --self-contained false -o "$INSTALL_DIR/timetrace-capture-app"

# Create wrapper script
echo "Creating launcher script..."
cat > "$INSTALL_DIR/$SERVICE_NAME" << 'EOF'
#!/bin/bash
exec "$HOME/.local/bin/timetrace-capture-app/TimeTrace.BackgroundProcessor" "$@"
EOF
chmod +x "$INSTALL_DIR/$SERVICE_NAME"

# Install systemd service
echo "Installing systemd service..."
cp "$SCRIPT_DIR/timetrace-capture.service" "$SYSTEMD_USER_DIR/"

# Reload systemd
echo "Reloading systemd user daemon..."
systemctl --user daemon-reload

echo ""
echo "Installation complete!"
echo ""
echo "To enable and start the service:"
echo "  systemctl --user enable $SERVICE_NAME"
echo "  systemctl --user start $SERVICE_NAME"
echo ""
echo "To view logs:"
echo "  journalctl --user -u $SERVICE_NAME -f"
echo ""
echo "To check status:"
echo "  systemctl --user status $SERVICE_NAME"
echo ""
