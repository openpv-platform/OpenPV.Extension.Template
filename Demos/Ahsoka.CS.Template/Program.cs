using Ahsoka.Core;
using Ahsoka.Services.System;
using Ahsoka.Services.Video;
using System;
using System.IO;

namespace Ahsoka.CS.VideoPlayer;

public class Program
{
    public static void Main()
    {
        string videoDemoUrl = @"/usr/local/Ahsoka/BigBuck.mp4";
        string usbCameraDevice = @"/dev/video-usb-cam1";
        string serialCameraDevice = @"/dev/video-imx219-cam0";

        Console.WriteLine("Starting Ahsoka Video Player");

        // Create a Service Client for our Ahsoka Runtimes
        // by default this client will run in our current process and use TCPIP
        // for communcation.  Most developers can use this basic setup.
        SystemService service = new SystemService();
        service.Start();

        // Create Video Service and Client to Run Video Player
        VideoServiceClient client = new();
        client.NotificationReceived += Client_NotificationReceived;
        client.Start();

        // request available video players:
        VideoPlayerInfo players = client.GetVideoPlayers();

        foreach(var player in players.Players)
        {
            switch (player.InputType)
            {
                case InputType.File:
                    if (File.Exists(videoDemoUrl))
                    {
                        client.PlayVideo(new VideoPlaybackRequest()
                        {
                            VideoPlayerID = player.VideoPlayerID,
                            PlayFullScreen = false,
                            StatusUpdateInMs = 1000,
                            VideoURL = videoDemoUrl
                        });
                    }
                    break;

                case InputType.Usb:
                    if (File.Exists(usbCameraDevice))
                    {
                        client.PlayVideo(new VideoPlaybackRequest()
                        {
                            VideoPlayerID = player.VideoPlayerID,
                            PlayFullScreen = false,
                            StatusUpdateInMs = 1000,
                            VideoURL = usbCameraDevice
                        });
                    }
                    break;

                case InputType.Csi:
                    if (File.Exists(serialCameraDevice))
                    {
                        client.PlayVideo(new VideoPlaybackRequest()
                        {
                            VideoPlayerID = player.VideoPlayerID,
                            PlayFullScreen = false,
                            StatusUpdateInMs = 1000,
                            VideoURL = serialCameraDevice
                        });
                    }
                    break;

                case InputType.Other:
                case InputType.Gmsl:
                default:
                    break;
            }

        }

        // Wait for Exit.
        ApplicationContext.WaitForExitSignal();

        // Request that the system shutdown any services, dispatchers or clients that are running
        // as well as ask linux to terminate Ahsoka.Application.
        ApplicationContext.Exit();
    }

    private static void Client_NotificationReceived(object sender, AhsokaClientBase<VideoMessageTypes.Ids>.AhsokaNotificationArgs e)
    {
        if (e.TransportId == VideoMessageTypes.Ids.StatusUpdated)
        {
            VideoStatus stats = e.NotificationObject as VideoStatus;
            Console.WriteLine($"Player: {stats.VideoPlayerID} - {stats.VideoState} ({stats.PositionInMs} of {stats.TotalTimeInMs})");
        }
    }

    /// <summary>
    /// Simple function to wait for Enter to be pressed
    /// </summary>
    /// <returns></returns>
    private static bool WaitForKeyboardEntry()
    {
        if (OperatingSystem.IsWindows())
        {
            AhsokaLogging.LogMessage(AhsokaVerbosity.High, "Press <enter> to Stop Application");
            Console.ReadLine();
        }
        else
        {
            AhsokaLogging.LogMessage(AhsokaVerbosity.High, "Waiting for Signal");
            ApplicationContext.WaitForExitSignal();
        }

        return true;
    }


}
