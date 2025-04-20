#!/usr/bin/env bash

crontab -l 2>/dev/null | grep -v "TimeTraceCapture.sh" | crontab -
echo "🛑 TimeTrace crontab entry removed."
