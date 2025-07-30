using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Ahsoka.Extensions.VideoPlayerConfig.UX;

public partial class VideoPlayerConfig : UserControl
{
    public VideoPlayerConfig()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}