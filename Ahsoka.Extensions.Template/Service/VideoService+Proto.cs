using ProtoBuf;
using System.Collections.Generic;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Ahsoka.Services.Video;

[ProtoContract]
public class VideoMessageTypes
{
    public enum Ids
    {
        NONE = 0,
        GetVideoPlayers = 1,
        PlayVideo = 2,
        PauseVideo = 3,
        ResumeVideo = 4,
        PositionVideo = 5,
        StopVideo = 6,
        RequestStatus = 7,
        StatusUpdated = 10, // Notification if Configured
    }
}

[ProtoContract]
public class VideoPlaybackRequest
{
    [ProtoMember(1)]
    public int VideoPlayerID { get; set; }

    [ProtoMember(2)]
    public string VideoURL { get; set; }

    [ProtoMember(3)]
    public bool PlayFullScreen { get; set; }

    [ProtoMember(4)]
    public int VideoX { get; set; }

    [ProtoMember(5)]
    public int VideoY { get; set; }

    [ProtoMember(6)]
    public int VideoWidth { get; set; }

    [ProtoMember(7)]
    public int VideoHeight { get; set; }

    [ProtoMember(8)]
    public int StatusUpdateInMs { get; set; }
}

[ProtoContract]
public class VideoPlayerInfo
{
    [ProtoMember(1, IsRequired = true)]
    public List<VideoPlayer> Players { get; set; } = new();
}

[ProtoContract]
public class VideoPlayer
{
    [ProtoMember(1)]
    public int VideoPlayerID { get; set; }

    [ProtoMember(2)]
    public int SurfaceID { get; set; }

    [ProtoMember(3)]
    public int SourceID { get; set; }

    [ProtoMember(4)]
    public string SourceName { get; set; } = string.Empty;

    [ProtoMember(5)]
    public InputType InputType { get; set; }

    [ProtoMember(6)]
    public int Brightness { get; set; }

    [ProtoMember(7)]
    public int Contrast { get; set; }

    [ProtoMember(8)]
    public int Saturation { get; set; }
}

public enum InputType
{
    Other = 0,
    Gmsl = 1,
    Usb = 2,
    Csi = 3,
    File = 4,
}

[ProtoContract]
public class VideoPosition
{
    [ProtoMember(1)]
    public int VideoPlayerID { get; set; }

    [ProtoMember(2)]
    public int PositionInMs { get; set; }
}

[ProtoContract]
public class VideoStatus
{
    [ProtoMember(1)]
    public int VideoPlayerID { get; set; }

    [ProtoMember(2)]
    public PlaybackState VideoState { get; set; }

    [ProtoMember(3)]
    public int TotalTimeInMs { get; set; }

    [ProtoMember(4)]
    public int PositionInMs { get; set; }
}

public enum PlaybackState
{
    Unavailable = 0,
    Ready = 1,
    Playing = 2,
    Paused = 3,
    Stopped = 4,
}