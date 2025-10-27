using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MetalSelling.Data;

namespace MetalSelling
{
    public partial class App : Application
    {
        public static DataBaseContext DataBaseContext { get; set; } = new DataBaseContext();
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}