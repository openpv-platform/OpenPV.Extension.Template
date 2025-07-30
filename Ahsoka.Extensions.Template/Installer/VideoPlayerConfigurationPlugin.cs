using Ahsoka.Core.Utility;
using Ahsoka.Installer.Components;
using Ahsoka.Services.Video;
using Ahsoka.Services.Install;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ahsoka.Installer.InstallEngine;

internal class VideoPlayerConfigurationPlugin : InstallerPlugin
{
    public const string ConfigFileName = "VideoPlayerConfiguration.pbuff";

    public override void AddAdditionalApplicationFiles(string buildLocation,
        Dictionary<string, string> attributes,
        Dictionary<string, byte[]> additionalFiles,
        PackageInformation info,
        IProgress<PackageProgressInfo> progress)
    {
        var extensionInfo = info.ServiceInfo.RuntimeConfiguration.ExtensionInfo.FirstOrDefault(x => x.ExtensionName.Equals("Video Player Extension"));
        if (extensionInfo == null && extensionInfo.ConfigurationFile == null)
            throw new Exception("Video Extension Not Configured.");

        var configJsonContents = File.ReadAllText(Path.Combine(Path.GetDirectoryName(info.GetPackageInfoPath()), extensionInfo.ConfigurationFile));
        VideoPlayerInfo videoPlayerConfig = JsonUtility.Deserialize<VideoPlayerInfo>(configJsonContents);
        var protobufferStream = new MemoryStream();
        ProtoBuf.Serializer.Serialize(protobufferStream, videoPlayerConfig);

        additionalFiles[$"{ApplicationOutputConstants.AppInfoFolder}/{ConfigFileName}"] = protobufferStream.ToArray();
        additionalFiles[$"{ApplicationOutputConstants.AppInfoFolder}/{ConfigFileName}.json"] = UTF8Encoding.UTF8.GetBytes(configJsonContents);
    }
}
