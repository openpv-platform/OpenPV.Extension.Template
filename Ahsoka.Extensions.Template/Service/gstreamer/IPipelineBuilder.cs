namespace Ahsoka.Services.Video;

internal interface IPipelineBuilder
{
    string BuildPipeline(VideoPlaybackRequest request);
    bool VerifyPipeline();
}
