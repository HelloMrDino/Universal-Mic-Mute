using System.Windows.Input;
using System.Windows.Interop;

namespace UniversalMicMute
{
    public class Settings
    {
        public string DeviceId { get; set; } = "";
        public int HotkeyVk { get; set; }
        public bool HotkeyCtrl { get; set; }
        public bool HotkeyAlt { get; set; }
        public bool HotkeyShift { get; set; }
        public bool HotkeyWin { get; set; }
        public double OverlayX { get; set; } = 100;
        public double OverlayY { get; set; } = 100;
        public double OverlayWidth { get; set; } = 80;
        public double OverlayHeight { get; set; } = 80;
        public double OverlayOpacity { get; set; } = 0.9;
        public bool DarkMode { get; set; } = true;

        public string GetHotkeyString()
        {
            if (HotkeyVk == 0) return "None";
            var parts = new List<string>();
            if (HotkeyCtrl) parts.Add("Ctrl");
            if (HotkeyAlt) parts.Add("Alt");
            if (HotkeyShift) parts.Add("Shift");
            if (HotkeyWin) parts.Add("Win");
            parts.Add(KeyToString(HotkeyVk));
            return string.Join(" + ", parts);
        }

        private static string KeyToString(int vk)
        {
            try
            {
                var key = KeyInterop.KeyFromVirtualKey(vk);
                return key.ToString();
            }
            catch
            {
                return $"VK_{vk:X2}";
            }
        }
    }
}
