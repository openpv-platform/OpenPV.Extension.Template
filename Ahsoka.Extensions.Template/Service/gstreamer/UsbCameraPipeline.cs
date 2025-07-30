namespace Ahsoka.Services.Video;

internal class UsbCameraPipeline : IPipelineBuilder
{
    public string BuildPipeline(VideoPlaybackRequest request)
    {
        return $"v4l2src device={request.VideoURL} ! video/x-raw, format=YUY2, framerate=15/1 ! waylandsink";
    }

    public bool VerifyPipeline()
    {
        return true;
    }
}
