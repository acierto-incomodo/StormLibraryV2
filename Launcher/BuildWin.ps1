./Clear.ps1
python -m PyInstaller --onefile --windowed --noconsole --icon=../img/logo.ico installer_updater.py
echo 1.0.0 > version_win_launcher.txt