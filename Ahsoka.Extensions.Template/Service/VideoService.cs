#pragma warning disable CS1591
using Ahsoka.Core;
using Ahsoka.Core.Utility;
using System.Collections.Generic;

using Ahsoka.Installer.InstallEngine;
using System;
using Ahsoka.Core.Hardware;
using Ahsoka.Installer.Components;
using System.IO;

namespace Ahsoka.Services.Video;

[AhsokaService(Name)]
public class VideoService : AhsokaServiceBase<VideoMessageTypes.Ids>
{
    #region Fields
    public const string Name = "VideoService";
    readonly Dictionary<int, VideoPlayerImpl> players = new();
    #endregion

    #region Methods
    internal VideoPlayerImpl GetInterop(VideoPlayer playerInfo)
    {
        switch (SystemInfo.CurrentPlatform)
        {
            case PlatformFamily.OpenViewLinux:
            case PlatformFamily.OpenViewLinuxPro:
                return new LinuxVideoOutOfProcServiceImplementation()
                {
                    PlayerInfo = playerInfo,
                    VideoStatusChanged = SendStatusNotification
                };

            case PlatformFamily.Windows64:
            case PlatformFamily.Ubuntu64:
            case PlatformFamily.MacOSArm64:
            default:
                return new DesktopVideoServiceImplementation()
                {
                    PlayerInfo = playerInfo,
                    VideoStatusChanged = SendStatusNotification
                };


        }
    }

    public VideoService() :
        this(ConfigurationLoader.GetServiceConfig(Name))
    {

    }

    public VideoService(ServiceConfiguration config) :
        base(config, new VideoServiceMessages())
    {
        using var stopwatch = new AhsokaStopwatch("Create VideoService");

        VideoPlayerInfo videoPlayers = GetVideoPlayerInfoFromConfig();
        foreach (var player in videoPlayers.Players)
            players[player.VideoPlayerID] = GetInterop(player);
    }

    VideoPlayerInfo GetVideoPlayerInfoFromConfig()
    {
        string configPath = Path.Combine
        (
            SystemInfo.HardwareInfo.TargetPathInfo.GetRootPaths(RootPaths.ApplicationRoot),
            ApplicationOutputConstants.AppInfoFolder,
            VideoPlayerConfigurationPlugin.ConfigFileName
        );

        try
        {
            var configPbuffContents = File.ReadAllBytes(configPath);
            return ProtoBuf.Serializer.Deserialize<VideoPlayerInfo>(new MemoryStream(configPbuffContents));
        }
        catch (Exception ex)
        {
            AhsokaLogging.LogMessage(AhsokaVerbosity.High, "Failed to get Video Player Config: ");
            AhsokaLogging.LogMessage(AhsokaVerbosity.High, ex.Message);
            return new VideoPlayerInfo();
        }
    }

    protected override void OnHandleServiceRequest(AhsokaServiceRequest message)
    {
        switch (message.TransportId)
        {
            case VideoMessageTypes.Ids.GetVideoPlayers:
                HandleGetPlayers(message);
                break;
            case VideoMessageTypes.Ids.PlayVideo:
                HandlePlayVideo(message, message.Message as VideoPlaybackRequest);
                break;
            case VideoMessageTypes.Ids.PauseVideo:
                HandlePauseVideo(message, message.Message as VideoPlayer);
                break;
            case VideoMessageTypes.Ids.ResumeVideo:
                HandleResumeVideo(message, message.Message as VideoPlayer);
                break;
            case VideoMessageTypes.Ids.PositionVideo:
                HandlePositionVideo(message, message.Message as VideoPosition);
                break;
            case VideoMessageTypes.Ids.StopVideo:
                HandleStopVideo(message, message.Message as VideoPlayer);
                break;
            case VideoMessageTypes.Ids.RequestStatus:
                HandleRequestStatus(message, message.Message as VideoPlayer);
                break;
        }
    }

    private void HandleGetPlayers(AhsokaServiceRequest message)
    {
        var playerInfo = new VideoPlayerInfo();
        foreach (var player in players.Values)
            playerInfo.Players.Add(player.PlayerInfo);

        SendResponse(message, playerInfo);
    }

    private void HandleRequestStatus(AhsokaServiceRequest message, VideoPlayer videoPlayer)
    {
        VideoStatus status = new();

        if (players.TryGetValue(videoPlayer.VideoPlayerID, out VideoPlayerImpl value))
            status = value.GetStatus(status);

        SendResponse(message, status);
    }

    private void HandlePlayVideo(AhsokaServiceRequest message, VideoPlaybackRequest videoPlaybackRequest)
    {
        VideoStatus status = new VideoStatus() { VideoState = PlaybackState.Playing };
        if (players.TryGetValue(videoPlaybackRequest.VideoPlayerID, out VideoPlayerImpl value))
            status = value.PlayVideo(videoPlaybackRequest);

        SendResponse(message, status);
    }

    private void HandlePauseVideo(AhsokaServiceRequest message, VideoPlayer player)
    {
        VideoStatus status = new VideoStatus() { VideoState = PlaybackState.Paused };
        if (players.TryGetValue(player.VideoPlayerID, out VideoPlayerImpl value))
            status = value.PauseVideo(player);

        SendResponse(message, status);
    }
    private void HandleResumeVideo(AhsokaServiceRequest message, VideoPlayer player)
    {
        VideoStatus status = new VideoStatus() { VideoState = PlaybackState.Playing };
        if (players.TryGetValue(player.VideoPlayerID, out VideoPlayerImpl value))
            status = value.ResumeVideo(player);

        SendResponse(message, status);
    }

    private void HandlePositionVideo(AhsokaServiceRequest message, VideoPosition position)
    {
        VideoStatus status = new VideoStatus() { VideoState = PlaybackState.Playing };
        if (players.TryGetValue(position.VideoPlayerID, out VideoPlayerImpl value))
            status = value.PositionVideo(position);

        SendResponse(message, status);
    }

    private void HandleStopVideo(AhsokaServiceRequest message, VideoPlayer videoPlayer)
    {
        VideoStatus status = new VideoStatus() { VideoState = PlaybackState.Stopped };
        if (players.TryGetValue(videoPlayer.VideoPlayerID, out VideoPlayerImpl value))
            status = value.StopVideo();

        SendResponse(message, status);
    }

    private void SendStatusNotification(VideoStatus statusEvent)
    {
        // Create a Header for our Notification.
        SendNotification(VideoMessageTypes.Ids.StatusUpdated, statusEvent);
    }
    #endregion
}
