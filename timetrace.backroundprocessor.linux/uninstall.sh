#!/usr/bin/env bash

echo "This will remove all TimeTrace files and cron jobs."
read -p "Are you sure? (y/N): " confirm

if [[ "$confirm" != "y" ]]; then
  echo "Cancelled."
  exit 0
fi

# Remove crontab
crontab -l 2>/dev/null | grep -v "TimeTraceCapture.sh" | crontab -

# Remove files
rm -rf /opt/TimeTrace

echo "✅ TimeTrace has been uninstalled."
