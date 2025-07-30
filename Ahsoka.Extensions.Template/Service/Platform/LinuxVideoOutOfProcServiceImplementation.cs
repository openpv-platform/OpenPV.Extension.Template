using Ahsoka.Core;
using Ahsoka.Core.Utility;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ahsoka.Services.Video;

internal class LinuxVideoOutOfProcServiceImplementation : VideoPlayerImpl
{
    static int playCount = 0;

    PlaybackState playbackState = PlaybackState.Ready;
    ulong totalMs = 0;
    ulong positionMs = 0;
    Process videoProcess = null;
    readonly string waylandSinkEnvVar = "WAYLANDSINK_SURFACE_ID";
    readonly string videoPlayerExeName = "Ahsoka.VideoPlayer";
    readonly bool VideoPlayerExeAvailable = false;

    public LinuxVideoOutOfProcServiceImplementation()
    {
        VideoPlayerExeAvailable = VerifyVideoPlayerExeAvailable();
    }

    bool VerifyVideoPlayerExeAvailable()
    {
        string path = GetVideoPlayerExePath();
        if (path == null)
        {
            AhsokaLogging.LogMessage(AhsokaVerbosity.High, $"No Video Player found relative to application. Ensure that Ahsoka.VideoPlayer.{SystemInfo.CurrentPlatform} exists in the AhsokaLib Directory");
            return false;
        }

        AhsokaLogging.LogMessage(AhsokaVerbosity.High, $"VideoPlayer Executable found at {path}");
        ProcessUtility.RunProcess("chmod", $"+x {path}", null, out string result, out string error);
        return true;
    }

    string GetVideoPlayerExePath()
    {
        return Directory.GetFiles(Path.GetDirectoryName(Environment.ProcessPath), videoPlayerExeName, SearchOption.AllDirectories).FirstOrDefault();
    }

    protected override VideoStatus OnStartVideo(VideoPlaybackRequest videoPlaybackRequest)
    {
        if (!VideoPlayerExeAvailable)
            return new VideoStatus()
            {
                VideoState = PlaybackState.Unavailable,
                TotalTimeInMs = 0,
                PositionInMs = 0,
                VideoPlayerID = this.PlayerInfo.VideoPlayerID
            };


        // Handle Play Video
        if (playbackState != PlaybackState.Playing)
        {
            playbackState = PlaybackState.Playing;

            AhsokaLogging.LogMessage(AhsokaVerbosity.High, $"Starting Video at {videoPlaybackRequest.VideoURL}");

            StartPlaybackProcess(videoPlaybackRequest);

            playCount++;
        }

        return CreateStatus();

    }

    protected override void StartPlaybackTimer(VideoPlaybackRequest videoPlaybackRequest)
    {
        // Pauses Video
        if (!videoProcess.HasExited)
            videoProcess.StandardInput.WriteLine($"NOTIFY:{videoPlaybackRequest.StatusUpdateInMs}");

    }

    private void StartPlaybackProcess(VideoPlaybackRequest videoPlaybackRequest)
    {
        if (!VideoPlayerExeAvailable)
            return;

        string args = GstreamerPipelineBuilder.BuildPipeline(this.PlayerInfo.InputType, videoPlaybackRequest);
        AhsokaLogging.LogMessage(AhsokaVerbosity.Medium, $"PLAYER {this.PlayerInfo.VideoPlayerID} SETUP: {args}");

        videoProcess = ProcessUtility.CreateProcess(GetVideoPlayerExePath(), args, null);
        videoProcess.StartInfo.Environment[waylandSinkEnvVar] = this.PlayerInfo.SurfaceID.ToString();
        videoProcess.StartInfo.RedirectStandardInput = true;
        videoProcess.Start();

        videoProcess.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            if (e.Data != null && !HandleMessage(e.Data))
                AhsokaLogging.LogMessage(AhsokaVerbosity.Low, $"PLAYER {this.PlayerInfo.VideoPlayerID}: " + e.Data);
        });

        videoProcess.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            AhsokaLogging.LogMessage(AhsokaVerbosity.High, e.Data);
        });

        videoProcess.BeginOutputReadLine();
        videoProcess.BeginErrorReadLine();

        var task = Task.Run(() =>
        {
            if (videoProcess == null)
            {
                videoProcess.WaitForExit();
                playbackState = PlaybackState.Stopped;
            }
        });

    }

    private bool HandleMessage(string data)
    {
        try
        {
            if (data.Trim().StartsWith("DURATION:"))
            {
                string timeString = data.Substring(data.IndexOf(":") + 2);
                var duration = TimeOnly.Parse(timeString);
                totalMs = (ulong)(duration - new TimeOnly()).TotalMilliseconds;

                // Send Notification on Each Duration Message (Comes After Position)
                var currentVideoStatus = OnGetVideoStatus();
                VideoStatusChanged?.Invoke(currentVideoStatus);
                return true;
            }
            else if (data.Trim().StartsWith("POSITION:"))
            {
                string timeString = data.Substring(data.IndexOf(":") + 2);
                var duration = TimeOnly.Parse(timeString);
                positionMs = (ulong)(duration - new TimeOnly()).TotalMilliseconds;

                return true;
            }
            else if (data.Trim().StartsWith("END"))
            {
                if (playbackState != PlaybackState.Stopped)
                {
                    playbackState = PlaybackState.Stopped;

                    var currentVideoStatus = OnGetVideoStatus();
                    VideoStatusChanged?.Invoke(currentVideoStatus);
                }

            }
        }
        catch (Exception e)
        {
            AhsokaLogging.LogMessage(AhsokaVerbosity.High, e.ToString());
        }
        return false;
    }


    protected override VideoStatus OnPauseVideo()
    {
        // Advance To End
        playbackState = PlaybackState.Paused;

        // Pauses Video
        if (!videoProcess.HasExited)
            videoProcess.StandardInput.WriteLine("PAUSE");

        return CreateStatus();
    }

    protected override VideoStatus OnResumeVideo()
    {
        // Advance To End
        playbackState = PlaybackState.Playing;

        // Pauses Video
        if (!videoProcess.HasExited)
            videoProcess.StandardInput.WriteLine("PLAY");

        return CreateStatus();
    }

    protected override VideoStatus OnPositionVideo(VideoPosition position)
    {
        // Pauses Video
        if (!videoProcess.HasExited)
            videoProcess.StandardInput.WriteLine($"POSITION:{position.PositionInMs}");

        return CreateStatus();
    }

    protected override VideoStatus OnStopVideo()
    {
        playbackState = PlaybackState.Stopped;

        // Pauses Video
        if (!videoProcess.HasExited)
            videoProcess.StandardInput.WriteLine("QUIT");

        return CreateStatus();
    }

    protected override VideoStatus OnGetVideoStatus()
    {
        return CreateStatus();
    }

    private VideoStatus CreateStatus()
    {
        return new VideoStatus()
        {
            VideoState = playbackState,
            TotalTimeInMs = (int)totalMs,
            PositionInMs = (int)positionMs,
            VideoPlayerID = this.PlayerInfo.VideoPlayerID
        };
    }
}

