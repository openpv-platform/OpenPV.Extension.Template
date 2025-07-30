using Ahsoka.Services.Video;
using Ahsoka.Core.Utility;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Ahsoka.Extensions.VideoPlayerConfig.UX;

internal class VideoPlayerViewModel : ChildViewModelBase<VideoPlayerConfigViewModel>
{
    public bool isShowing = false;
    public VideoPlayer Player { get; set; }

    public VideoPlayerViewModel(VideoPlayerConfigViewModel setupViewModel, VideoPlayer player) : base(setupViewModel)
    {
        this.Player = player;
        SupportedTypes = Enum.GetValues<InputType>().ToArray();
    }

    public int VideoPlayerID
    {
        get { return Player.VideoPlayerID; }
        set { Player.VideoPlayerID = value; OnPropertyChanged(); }
    }

    [Range(0, 10000)]
    public int VideoSurfaceID
    {
        get { return Player.SurfaceID; }
        set { Player.SurfaceID = value; OnPropertyChanged(); }
    }

    [Range(0, 10000)]
    public int VideoSourceID
    {
        get { return Player.SourceID; }
        set { Player.SourceID = value; OnPropertyChanged(); }
    }

    public string VideoSourceName
    {
        get { return Player.SourceName; }
        set { Player.SourceName = value; OnPropertyChanged(); }
    }


    public InputType VideoInputType
    {
        get { return Player.InputType; }
        set { Player.InputType = value; OnPropertyChanged(); }
    }

    [Range(0, 100)]
    public int VideoBrightness
    {
        get { return Player.Brightness; }
        set { Player.Brightness = value; OnPropertyChanged(); }
    }

    [Range(0, 100)]
    public int VideoContrast
    {
        get { return Player.Contrast; }
        set { Player.Contrast = value; OnPropertyChanged(); }
    }

    [Range(0, 100)]
    public int VideoSaturation
    {
        get { return Player.Saturation; }
        set { Player.Saturation = value; OnPropertyChanged(); }
    }

    private InputType[] SupportedTypes { get; set; }

    public void CloseAdvancedOptionsConfig()
    {
        isShowing = false;
        ParentViewModel.CloseAdvancedOptionsConfig();
    }
}


