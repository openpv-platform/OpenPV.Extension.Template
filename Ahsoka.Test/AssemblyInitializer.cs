using Ahsoka.Core;
using Ahsoka.Core.Hardware;
using Ahsoka.Core.Utility;
using Ahsoka.Test.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Ahsoka.Test;

[TestClass]
public class TestInitializer : LinearTestBase
{
    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
        // Fall back to Developer Support Folder if running Standalone.
        if (HardwareInfo.GetHardwareInfoDescriptions().Count == 0)
        {
            var hd = HardwareInfo.DesktopHardwareInfo;
            HardwareInfo.AddHardwareInfo(hd);
        }

        /// Prep Windows for Tests
        var progress = new Progress<string>(Console.WriteLine);

        var prep = RemoteTargetToolFactory.GetToolsForPlatform(PlatformFamily.Windows64);

        // Load Hardware Info
        HardwareInfo hardware = HardwareInfo.GetHardwareInfo(PlatformFamily.Windows64, "Desktop");


        // Update DeviceID's to match to ensure that Encryption / Decryption uses the same value.
        SystemInfo.HardwareInfo.FactoryInfo.DeviceId = hardware.FactoryInfo.DeviceId = Guid.NewGuid();

        var info = new TargetConnectionInfo() { PlatformFamily = hardware.PlatformFamily, PlatformQualifier = hardware.PlatformQualifier, HostName = "localhost", UserName = "" };

        // Prep Local Machine
        bool returnValue = prep.Prep(info, null, progress);
    }
}
