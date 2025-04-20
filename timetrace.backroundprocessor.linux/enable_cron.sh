#!/usr/bin/env bash

CRON_JOB="@reboot /opt/TimeTrace/TimeTraceCapture.sh &"
(crontab -l 2>/dev/null | grep -v "TimeTraceCapture.sh"; echo "$CRON_JOB") | crontab -

echo "✅ TimeTrace background process added to crontab (runs at reboot)."
