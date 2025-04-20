#!/usr/bin/env bash
set -e

APP_NAME="TimeTrace"
INSTALL_DIR="/opt/$APP_NAME"
SCRIPT_NAME="TimeTraceCapture.sh"
SCRIPT_PATH="$INSTALL_DIR/$SCRIPT_NAME"
DB_PATH="$INSTALL_DIR/config.db"
SCHEMA_FILE="./timetracedb.sql"

detect_pkg_mgr() {
  if command -v apt-get &> /dev/null; then
    INSTALL_CMD="apt-get install -y"
  elif command -v dnf &> /dev/null; then
    INSTALL_CMD="dnf install -y"
  elif command -v pacman &> /dev/null; then
    INSTALL_CMD="pacman -S --noconfirm"
  else
    echo "Unsupported distro. Please install dependencies manually."
    exit 1
  fi
}

install_deps() {
  echo "Installing dependencies..."

  if ! command -v scrot &> /dev/null; then
    read -p "Install 'scrot' (X11 screenshot tool)? [Y/n] " ans
    [[ "$ans" != "n" && "$ans" != "N" ]] && $INSTALL_CMD scrot
  fi

  if ! command -v grim &> /dev/null; then
    read -p "Install 'grim' (Wayland screenshot tool)? [Y/n] " ans
    [[ "$ans" != "n" && "$ans" != "N" ]] && $INSTALL_CMD grim
  fi

  if ! command -v sqlite3 &> /dev/null; then
    read -p "Install sqlite3? [Y/n] " ans
    [[ "$ans" != "n" && "$ans" != "N" ]] && $INSTALL_CMD sqlite3
  fi
}

create_config() {
  mkdir -p "$INSTALL_DIR"

  if [[ -f "$DB_PATH" ]]; then
    echo "Config already exists at $DB_PATH"
    return
  fi

  echo "Creating config..."
  read -p "Enter screenshot interval (in seconds): " interval
  read -e -p "Enter directory to save screenshots: " savePath

  if [[ ! -f "$SCHEMA_FILE" ]]; then
    echo "❌ Missing schema file: $SCHEMA_FILE"
    exit 1
  fi

  sqlite3 "$DB_PATH" < "$SCHEMA_FILE"

  sqlite3 "$DB_PATH" <<SQL
INSERT INTO ConfigurationSetting (Key) VALUES ('screenshot_interval');
INSERT INTO ConfigurationSetting (Key) VALUES ('save_directory');
INSERT INTO ConfigurationSettingDetail (ConfigurationSettingId, Value)
  SELECT Id, '$interval' FROM ConfigurationSetting WHERE Key = 'screenshot_interval';
INSERT INTO ConfigurationSettingDetail (ConfigurationSettingId, Value)
  SELECT Id, '$savePath' FROM ConfigurationSetting WHERE Key = 'save_directory';
SQL

  echo "✅ Config saved to $DB_PATH"
}

setup_script() {
  echo "Copying main script..."
  cp "./$SCRIPT_NAME" "$SCRIPT_PATH"
  chmod +x "$SCRIPT_PATH"
}

detect_pkg_mgr
install_deps
create_config
setup_script
