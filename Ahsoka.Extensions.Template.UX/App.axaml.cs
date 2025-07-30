using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Ahsoka.Services.Video;
using Ahsoka.Core.Utility;
using Ahsoka.DeveloperTools.Views;

namespace Ahsoka.Extensions.VideoPlayerConfig.UX;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        ClassLoader.AddAssembly(typeof(VideoService).Assembly);
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // load our video player config view model:
            desktop.MainWindow = new ExtensionMainWindow("VideoPlayer Service Extension", typeof(VideoPlayerConfigViewModel));
        }
        base.OnFrameworkInitializationCompleted();
    }

    public override void RegisterServices()
    {
        base.RegisterServices();
    }
}
