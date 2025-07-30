using Ahsoka.Core;
namespace Ahsoka.Services.Video;

internal class FilePipeline : IPipelineBuilder
{
    public string BuildPipeline(VideoPlaybackRequest request)
    {
        string pipeline = string.Empty;
        if (SystemInfo.CurrentPlatform == PlatformFamily.OpenViewLinux)
        {
            pipeline = $" ! decodebin ! videoconvert ! videoscale ! waylandsink fullscreen=true";
            pipeline = $"filesrc location={request.VideoURL} {pipeline}";
        }
        else if (SystemInfo.CurrentPlatform == PlatformFamily.OpenViewLinuxPro)
            pipeline = $"playbin uri=file://{request.VideoURL} video-sink=waylandsink";

        return pipeline;
    }

    public bool VerifyPipeline()
    {
        return true;
    }
}
