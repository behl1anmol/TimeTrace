#!/usr/bin/env bash

APP_DIR="/opt/TimeTrace"
DB="$APP_DIR/config.db"

if [[ ! -f "$DB" ]]; then
  echo "❌ Config database not found at $DB"
  exit 1
fi

interval=$(sqlite3 "$DB" "SELECT CSD.Value FROM ConfigurationSetting CS JOIN ConfigurationSettingDetail CSD ON CS.Id = CSD.ConfigurationSettingId WHERE CS.Key = 'screenshot_interval' LIMIT 1;")
savepath=$(sqlite3 "$DB" "SELECT CSD.Value FROM ConfigurationSetting CS JOIN ConfigurationSettingDetail CSD ON CS.Id = CSD.ConfigurationSettingId WHERE CS.Key = 'save_directory' LIMIT 1;")

mkdir -p "$savepath"

detect_session_type() {
  [[ "$XDG_SESSION_TYPE" == "wayland" ]] && echo "wayland" || echo "x11"
}

take_screenshot() {
  filename="screenshot_$(date +'%Y%m%d_%H%M%S').png"
  fullpath="$savepath/$filename"

  session=$(detect_session_type)

  if [[ "$session" == "wayland" && $(command -v grim) ]]; then
    grim "$fullpath"
  elif [[ "$session" == "x11" && $(command -v scrot) ]]; then
    DISPLAY=:0 scrot "$fullpath"
  else
    echo "⚠️ No screenshot tool found for session type: $session"
  fi
}

while true; do
  take_screenshot
  sleep "$interval"
done
