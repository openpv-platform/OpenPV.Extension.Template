using Ahsoka.Core;
using Ahsoka.Core.Hardware;
using Ahsoka.DeveloperTools.Core;

using Ahsoka.Core.Utility;
using Ahsoka.Services.Video;
using Avalonia.Controls;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace Ahsoka.Extensions.VideoPlayerConfig.UX;

internal class VideoPlayerConfigViewModel : ExtensionViewModelBase
{
    readonly string defaultConfigFileName = "VideoPlayerConfiguration.json";
    string configFilePath = string.Empty;
    string projectFolderPath = string.Empty;
    VideoPlayerInfo config;

    public ObservableCollection<VideoPlayerViewModel> Players { get; set; } = new();
    public ObservableCollection<VideoPlayerViewModel> ConfigurablePlayer { get; set; } = new();

    VideoPlayerViewModel selectedVideoPlayer;
    public VideoPlayerViewModel SelectedVideoPlayer
    {
        get { return selectedVideoPlayer; }
        set { selectedVideoPlayer = value; OnPropertyChanged(); }
    }

    protected override UserControl OnGetView()
    {
        return new VideoPlayerConfig() { DataContext = this };
    }

    protected override void OnInitExtension(HardwareInfo hardwareInfo, string projectInfoFolder, string configurationFile)
    {
        projectFolderPath = projectInfoFolder;
        configFilePath = configurationFile;
        InitVideoPlayerConfig(configurationFile, projectInfoFolder);
    }

    void InitVideoPlayerConfig(string configFilePath, string projectFolderPath)
    {
        if (string.IsNullOrEmpty(configFilePath))
        {
            // generate a default config
            string pathToDefaultConfig = Path.Combine(projectFolderPath, defaultConfigFileName);
            config = (File.Exists(pathToDefaultConfig))
                ? TryDesirializingConfig(pathToDefaultConfig) : new VideoPlayerInfo();
        }
        else
            config = TryDesirializingConfig(configFilePath);

        foreach (var player in config.Players)
            Players.Add(new VideoPlayerViewModel(this, player));

        // If no players exist, add one to start
        if (Players.Count == 0)
            AddVideoPlayer();
    }

    string GetDefaultConfigPath()
    {
        return Path.Combine(projectFolderPath, defaultConfigFileName);
    }

    VideoPlayerInfo TryDesirializingConfig(string path)
    {
        try
        {
            return ConfigurationFileLoader.LoadFile<VideoPlayerInfo>(path, false);
        }
        catch
        {
            return new VideoPlayerInfo();
        }
    }

    protected override string OnSave(string packageInfoFolder)
    {
        string videoPlayerData = JsonUtility.Serialize(config);

        if (string.IsNullOrEmpty(configFilePath))
        {
            // save to the default config
            string pathToDefaultConfig = GetDefaultConfigPath();
            File.WriteAllText(pathToDefaultConfig, videoPlayerData);
            return pathToDefaultConfig;
        }
        else
        {
            // save to original file
            try
            {
                File.WriteAllText(configFilePath, videoPlayerData);
                return configFilePath;
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    protected override void OnClose()
    {
        configFilePath = string.Empty;
        Players.Clear();
        config.Players.Clear();
    }

    protected override bool OnHasChanges()
    {
        return true;
    }

    internal void RemoveVideoPlayer()
    {
        if (config.Players.Count == 0 || SelectedVideoPlayer == null)
            return;

        var removablePlayer = config.Players.FirstOrDefault(player => player.VideoPlayerID == SelectedVideoPlayer.VideoPlayerID);
        if (removablePlayer != null)
            config.Players.Remove(removablePlayer);

        Players.Remove(SelectedVideoPlayer);
    }

    internal int GetUniqueID()
    {
        // here we will just find the max ID and increment it by 1 to give the next ID
        return Players.Count == 0 ? 1 : Players.Max(player => player.VideoPlayerID) + 1;
    }

    internal void AddVideoPlayer()
    {
        VideoPlayer player = new() { VideoPlayerID = GetUniqueID(), SourceName = "New Video" };
        Players.Add(new VideoPlayerViewModel(this, player));
        config.Players.Add(player);
    }

    public void ShowAdvancedOptions()
    {
        if (SelectedVideoPlayer != null)
            DisplayAdvancedOptionsConfig(SelectedVideoPlayer.VideoPlayerID);
    }

    bool advancedConfigShown = false;
    public bool AdvancedConfigShown
    {
        get { return advancedConfigShown; }
        set { advancedConfigShown = value; OnPropertyChanged(); }
    }

    void DisplayAdvancedOptionsConfig(int playerID)
    {
        // set the player to config
        var player = Players.FirstOrDefault(player => player.VideoPlayerID == playerID);
        if (player != null)
        {
            ConfigurablePlayer.Add(player);
            AdvancedConfigShown = true;
        }
    }

    public void CloseAdvancedOptionsConfig()
    {
        AdvancedConfigShown = false;
        ConfigurablePlayer.Clear();
        SelectedVideoPlayer = null;
    }
}
