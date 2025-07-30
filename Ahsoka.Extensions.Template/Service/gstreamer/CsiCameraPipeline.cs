using Ahsoka.Core;
namespace Ahsoka.Services.Video;

internal class CsiCameraPipeline : IPipelineBuilder
{
    public string BuildPipeline(VideoPlaybackRequest request)
    {
        // Only supported on Pro
        string pipeline = string.Empty;
        if (SystemInfo.CurrentPlatform == PlatformFamily.OpenViewLinuxPro)
            pipeline = $"v4l2src device={request.VideoURL} ! video/x-raw, format=YUY2, framerate=15/1 ! waylandsink";
        return pipeline;
    }

    public bool VerifyPipeline()
    {
        return true;
    }
}
