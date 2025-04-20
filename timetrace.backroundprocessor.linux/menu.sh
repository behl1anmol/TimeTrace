#!/usr/bin/env bash

show_menu() {
  echo "========== TimeTrace Menu =========="
  echo "1. Install TimeTrace"
  echo "2. Uninstall TimeTrace"
  echo "3. Enable background screenshot capture"
  echo "4. Disable background screenshot capture"
  echo "5. Exit"
  echo "===================================="
}

while true; do
  show_menu
  read -p "Choose an option [1-5]: " choice

  case $choice in
    1) sudo ./install.sh ;;
    2) sudo ./uninstall.sh ;;
    3) ./enable_cron.sh ;;
    4) ./disable_cron.sh ;;
    5) echo "Goodbye!" && exit 0 ;;
    *) echo "❌ Invalid choice. Try again." ;;
  esac
done
