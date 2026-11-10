using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace UniversalMicMute
{
    public partial class OverlayWindow : Window
    {
        private bool _editMode;
        private bool _isDragging;
        private bool _isResizing;
        private System.Windows.Point _dragStart;
        private double _startSize;

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_LAYERED = 0x00080000;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public OverlayWindow()
        {
            InitializeComponent();
            SetMuted(false);
            Opacity = 0.9;
            Loaded += (_, _) => SetClickThrough(true);
        }

        private void SetClickThrough(bool enabled)
        {
            try
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                if (hwnd == IntPtr.Zero) return;
                int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                if (enabled)
                    SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_TRANSPARENT | WS_EX_LAYERED);
                else
                    SetWindowLong(hwnd, GWL_EXSTYLE, exStyle & ~WS_EX_TRANSPARENT);
            }
            catch { }
        }

        public void SetMuted(bool muted)
        {
            Visibility = muted ? Visibility.Visible : Visibility.Hidden;
            if (muted)
            {
                Border.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xCC, 0xFF, 0x33, 0x33));
            }
        }

        public void SetOpacity(double value)
        {
            Opacity = Math.Clamp(value, 0.2, 1.0);
        }

        public void EnableEditMode()
        {
            _editMode = true;
            SetClickThrough(false);
            Border.BorderBrush = System.Windows.Media.Brushes.Yellow;
            Border.BorderThickness = new Thickness(2);
            ResizeHandle.Visibility = Visibility.Visible;
            Cursor = System.Windows.Input.Cursors.SizeAll;
        }

        public void DisableEditMode()
        {
            _editMode = false;
            _isDragging = false;
            _isResizing = false;
            SetClickThrough(true);
            Border.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xCC, 0xFF, 0x44, 0x44));
            Border.BorderThickness = new Thickness(1.5);
            ResizeHandle.Visibility = Visibility.Collapsed;
            Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_editMode) return;
            if (e.OriginalSource == ResizeHandle) return;

            _isDragging = true;
            _dragStart = e.GetPosition(this);
            CaptureMouse();
            MouseMove += OnMouseMove;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
        }

        private void ResizeHandle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_editMode) return;
            _isResizing = true;
            _startSize = Width;
            _dragStart = e.GetPosition(this);
            CaptureMouse();
            MouseMove += OnMouseMove;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
            e.Handled = true;
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!IsMouseCaptured) return;
            var pos = e.GetPosition(this);

            if (_isDragging)
            {
                Left += pos.X - _dragStart.X;
                Top += pos.Y - _dragStart.Y;
            }
            else if (_isResizing)
            {
                double delta = (pos.X - _dragStart.X + pos.Y - _dragStart.Y) / 2.0;
                double newSize = Math.Max(40, _startSize + delta);
                Width = newSize;
                Height = newSize;
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
            MouseMove -= OnMouseMove;
            MouseLeftButtonUp -= OnMouseLeftButtonUp;
            _isDragging = false;
            _isResizing = false;
        }
    }
}
