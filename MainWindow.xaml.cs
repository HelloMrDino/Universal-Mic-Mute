using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using NAudio.CoreAudioApi;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using NotifyIcon = System.Windows.Forms.NotifyIcon;
using ContextMenuStrip = System.Windows.Forms.ContextMenuStrip;
using ToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem;

namespace UniversalMicMute
{
    public partial class MainWindow : Window
    {
        private NotifyIcon? _trayIcon;
        private OverlayWindow? _overlay;
        private MMDeviceEnumerator? _enumerator;
        private MMDevice? _selectedDevice;
        private Settings _settings = new();
        private bool _isListeningForHotkey;
        private KeyboardHook? _hook;
        private bool _forceClose;

        public MainWindow()
        {
            InitializeComponent();
            Title = "Universal Mic Mute (UMM)";
            LoadSettings();
            InitializeAudio();
            PopulateDevices();
            InitializeTray();
            InitializeOverlay();
            StartHook();
            ApplyTheme();
            ThemeToggleButton.Content = _settings.DarkMode ? "Light Mode" : "Dark Mode";
            OpacitySlider.Value = _settings.OverlayOpacity;
            UpdateStartupButtonText();
            Hide();
        }

        private void LoadSettings()
        {
            try
            {
                using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\UniversalMicMute", false);
                if (key == null) return;
                _settings.DeviceId = key.GetValue("DeviceId") as string ?? "";
                _settings.HotkeyVk = (int)(key.GetValue("HotkeyVk") ?? 0);
                _settings.HotkeyCtrl = (int)(key.GetValue("HotkeyCtrl") ?? 0) != 0;
                _settings.HotkeyAlt = (int)(key.GetValue("HotkeyAlt") ?? 0) != 0;
                _settings.HotkeyShift = (int)(key.GetValue("HotkeyShift") ?? 0) != 0;
                _settings.HotkeyWin = (int)(key.GetValue("HotkeyWin") ?? 0) != 0;
                _settings.OverlayX = Convert.ToDouble(key.GetValue("OverlayX") ?? 100);
                _settings.OverlayY = Convert.ToDouble(key.GetValue("OverlayY") ?? 100);
                _settings.OverlayWidth = Convert.ToDouble(key.GetValue("OverlayWidth") ?? 80);
                _settings.OverlayHeight = Convert.ToDouble(key.GetValue("OverlayHeight") ?? 80);
                _settings.OverlayOpacity = Convert.ToDouble(key.GetValue("OverlayOpacity") ?? 0.9);
                _settings.DarkMode = (int)(key.GetValue("DarkMode") ?? 0) != 0;
            }
            catch { }
        }

        private void SaveSettings()
        {
            try
            {
                using var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\\UniversalMicMute");
                if (key == null) return;
                key.SetValue("DeviceId", _settings.DeviceId ?? "");
                key.SetValue("HotkeyVk", _settings.HotkeyVk);
                key.SetValue("HotkeyCtrl", _settings.HotkeyCtrl ? 1 : 0);
                key.SetValue("HotkeyAlt", _settings.HotkeyAlt ? 1 : 0);
                key.SetValue("HotkeyShift", _settings.HotkeyShift ? 1 : 0);
                key.SetValue("HotkeyWin", _settings.HotkeyWin ? 1 : 0);
                key.SetValue("OverlayX", _settings.OverlayX);
                key.SetValue("OverlayY", _settings.OverlayY);
                key.SetValue("OverlayWidth", _settings.OverlayWidth);
                key.SetValue("OverlayHeight", _settings.OverlayHeight);
                key.SetValue("OverlayOpacity", _settings.OverlayOpacity);
                key.SetValue("DarkMode", _settings.DarkMode ? 1 : 0);
            }
            catch { }
        }

        // ... remaining methods (full file is large; see local project for complete source)
    }
}
