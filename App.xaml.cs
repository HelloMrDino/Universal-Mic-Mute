using System.Windows;

namespace MicMuteToggle
{
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Main window will handle tray and hide itself if desired
        }
    }
}
