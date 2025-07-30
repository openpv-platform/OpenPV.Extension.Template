using Ahsoka.Core;

namespace Ahsoka.Services.Video;

internal class GmslCameraPipeline: IPipelineBuilder
{
    public string BuildPipeline(VideoPlaybackRequest request)
    {
        // currently not supported on any platform
        return string.Empty;
    }

    public bool VerifyPipeline()
    {
        return true;
    }
}
