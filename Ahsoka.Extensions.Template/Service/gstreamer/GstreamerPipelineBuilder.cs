namespace Ahsoka.Services.Video;

public class GstreamerPipelineBuilder
{
    public static string BuildPipeline(InputType type, VideoPlaybackRequest videoPlaybackRequest)
    {
        return type switch
        {
            InputType.File => new FilePipeline().BuildPipeline(videoPlaybackRequest),
            InputType.Usb => new UsbCameraPipeline().BuildPipeline(videoPlaybackRequest),
            InputType.Csi => new CsiCameraPipeline().BuildPipeline(videoPlaybackRequest),
            InputType.Gmsl => new GmslCameraPipeline().BuildPipeline(videoPlaybackRequest),
            _ => string.Empty,
        };
    }
}
