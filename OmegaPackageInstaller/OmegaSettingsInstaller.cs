﻿using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using File = System.IO.File;

namespace OmegaPackageInstaller
{
    public partial class OmegaSettingsInstaller : Form
    {
        //Bump the version number for each release
        private String NewVersion = "2.0";

        public OmegaSettingsInstaller()
        {
            InitializeComponent();
            PrepareForInstall();

            //Update the Install file and Delete file Lists.
            //Only pull in what was modified from the last baseline release...

            //These files will be added (or replaced if they exist).
            //For each of these, a correspondidng file must be added to the project with Build Action set to Embedded Resource
            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\Plugins\\", "OmegaSettingsMenu.dll"));
            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\Themes\\Unified Redux\\Views\\", "GameMarqueeView.xaml"));
            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\Themes\\Unified Redux\\Views\\", "PlatformMarqueeView.xaml"));
            //            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\Themes\\Unified Redux\\Views\\", "WheelGamesView.xaml"));
            //            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\Themes\\Unified Redux\\Views\\", "Wheel2GamesView.xaml"));
            //            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\Themes\\Unified Redux\\Views\\", "Wheel3GamesView.xaml"));
            //            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\Themes\\Unified Redux\\Views\\", "Wheel4GamesView.xaml"));
            //            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\", "RebootBigBox.exe"));


            //Thirdscreen files:
            ThirdScreenFileList.Add(new InstallFile(LaunchBoxFolder + "\\Plugins\\", "ThirdScreen.dll"));
            ThirdScreenFileList.Add(new InstallFile(LaunchBoxFolder + "\\Plugins\\", "ThirdScreenSupportLib.dll"));
            ThirdScreenFileList.Add(new InstallFile(LaunchBoxFolder + "\\Plugins\\", "ManagePlatformVideoMarquees.dll"));
            ThirdScreenFileList.Add(new InstallFile(LaunchBoxFolder + "\\Data\\", "ThirdScreenSettings.xml"));

            ThirdScreenFileList.Add(new InstallFile(LaunchBoxFolder + "\\ThirdParty\\MediaInfo\\x64\\", "libcrypto-3-x64.dll"));
            ThirdScreenFileList.Add(new InstallFile(LaunchBoxFolder + "\\ThirdParty\\MediaInfo\\x64\\", "libcurl.dll"));
            ThirdScreenFileList.Add(new InstallFile(LaunchBoxFolder + "\\ThirdParty\\MediaInfo\\x64\\", "libssl-3-x64.dll"));
            ThirdScreenFileList.Add(new InstallFile(LaunchBoxFolder + "\\ThirdParty\\MediaInfo\\x64\\", "MediaInfo.dll"));

            ThirdScreenFileList.Add(new InstallFile(LaunchBoxFolder + "\\ThirdParty\\MediaInfo\\x86\\", "libcrypto-3.dll"));
            ThirdScreenFileList.Add(new InstallFile(LaunchBoxFolder + "\\ThirdParty\\MediaInfo\\x86\\", "libcurl.dll", true, "x86_libcurl.dll"));
            ThirdScreenFileList.Add(new InstallFile(LaunchBoxFolder + "\\ThirdParty\\MediaInfo\\x86\\", "libssl-3.dll"));
            ThirdScreenFileList.Add(new InstallFile(LaunchBoxFolder + "\\ThirdParty\\MediaInfo\\x86\\", "MediaInfo.dll", true, "x86_MediaInfo.dll"));


            // These files will be deleted if they exist
            DeleteFileList.Add(new DeleteFile(LaunchBoxFolder + "\\Plugins\\", "MediaPlayer.Wpf.dll"));
            DeleteFileList.Add(new DeleteFile(LaunchBoxFolder + "\\Plugins\\", "MediaPlayer.Wpf.Mpv.dll"));
            DeleteFileList.Add(new DeleteFile(LaunchBoxFolder + "\\Plugins\\", "Mpv.NET.dll"));
            DeleteFileList.Add(new DeleteFile(LaunchBoxFolder + "\\ThirdParty\\MPV\\", "mpv-2.dll"));

            DeleteFileList.Add(new DeleteFile(LaunchBoxFolder + "\\Plugins\\", "XamlAnimatedGif.dll"));
            DeleteFileList.Add(new DeleteFile(LaunchBoxFolder + "\\", "BigBoxWithStartupMarquee.exe", Environment.GetFolderPath(Environment.SpecialFolder.Startup)));
            DeleteFileList.Add(new DeleteFile(LaunchBoxFolder + "\\Plugins\\", "OmegaStartupMarquee.dll"));
            DeleteFileList.Add(new DeleteFile(LaunchBoxFolder + "\\", "OmegaBigBoxMonitor.exe", Environment.GetFolderPath(Environment.SpecialFolder.Startup)));
            DeleteFileList.Add(new DeleteFile(LaunchBoxFolder + "\\Data\\", "OmegaBigBoxMonitor.xml"));

            start_timer.Interval = 250;
            start_timer.Enabled = true;
        }

        private void AdditionalActions()
        {
            ////Set BigBox video setting to WMP
            //try
            //{
            //    XDocument xdoc = XDocument.Load(LaunchBoxFolder + "/Data/BigBoxSettings.xml");
            //    var element = xdoc
            //            .XPathSelectElement("/LaunchBox/BigBoxSettings")
            //            .Element("VideoPlaybackEngine");
            //    element.Value = "Windows Media Player";
            //    xdoc.Save(LaunchBoxFolder + "/Data/BigBoxSettings.xml");
            //    textbox_console.AppendText("Set BigBox video setting to Windows Media Player.\r\n");
            //}
            //catch { }

            ////Adjust Background image priorities (these are used for static image themes)
            //try
            //{
            //    XDocument xdoc = XDocument.Load(LaunchBoxFolder + "/Data/Settings.xml");
            //    var element = xdoc
            //            .XPathSelectElement("/LaunchBox/Settings")
            //            .Element("BackgroundImageTypePriorities");
            //    element.Value = "Screenshot - Game Title,Screenshot - Game Select,Screenshot - Gameplay,Epic Games Background,Uplay Background,Origin Background,Amazon Background,Fanart - Background,Advertisement Flyer - Front,Arcade - Cabinet,Clear Logo";
            //    xdoc.Save(LaunchBoxFolder + "/Data/Settings.xml");
            //    textbox_console.AppendText("Adjusted LaunchBox background image path priorities to support static image themes.\r\n");
            //}
            //catch{ }

            //Create the startup shortcut if it doesn't exist.
            string TargetPathName = Path.Combine(LaunchBoxFolder, "BigBox.exe");
            WshShell shell = new WshShell(); //Create a new WshShell Interface
            Boolean link_found = false;

            foreach (var file in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Startup)))
            {
                try
                {
                    IWshShortcut existing_link = (IWshShortcut)shell.CreateShortcut(Path.GetFullPath(file)); //Link the interface to our shortcut
                    if (Path.GetFileName(existing_link.TargetPath) == Path.GetFileName(TargetPathName))
                    {
                        //The shortcut already exists.
                        link_found = true;
                    }
                }
                catch { }
            }
            if (!link_found)
            {
                String shortcut_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "BigBox.lnk");
                IWshShortcut link = (IWshShortcut)shell.CreateShortcut(shortcut_path);
                link.TargetPath = TargetPathName;
                link.Save();
                textbox_console.AppendText("Added startup shortcut to BigBox.exe.\r\n");
            }

            //Remove startup shortcut to BigBoxWithStartupMarquee.exe
            TargetPathName = Path.Combine(LaunchBoxFolder, "BigBoxWithStartupMarquee.exe");
            foreach (var file in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Startup)))
            {
                try
                {
                    IWshShortcut existing_link = (IWshShortcut)shell.CreateShortcut(Path.GetFullPath(file)); //Link the interface to our shortcut
                    if (Path.GetFileName(existing_link.TargetPath) == Path.GetFileName(TargetPathName))
                    {
                        //Delete the shortcut
                        File.Delete(file);
                        textbox_console.AppendText("Deleted startup shortcut to BigBoxWithStartupMarquee.exe.\r\n");
                    }
                }
                catch { }
            }


            //Test if ThirdScreen is already installed
            String thirdscreen_path = Path.Combine(LaunchBoxFolder, "Plugins/ThirdScreen.dll");
            if (!File.Exists(thirdscreen_path))
            {
                //ThirdScreen not installed
                textbox_console.AppendText("Installing ThirdScreen version 2.0.12.\r\n");

                //Disable Marquee in BigBox
                try
                {
                    XDocument xdoc = XDocument.Load(LaunchBoxFolder + "/Data/BigBoxSettings.xml");
                    var element = xdoc
                            .XPathSelectElement("/LaunchBox/BigBoxSettings")
                            .Element("MarqueeMonitorIndex");
                    element.Value = "-1";
                    xdoc.Save(LaunchBoxFolder + "/Data/BigBoxSettings.xml");
                    textbox_console.AppendText("Disabled Marquee in BigBox settings.\r\n");
                }
                catch { }

                //Install ThirdScreen files along with initial settings file
                foreach (var ts_file in ThirdScreenFileList)
                {
                    ts_file.Install();
                }

                //Move any marquee startup videos to thirdscreen startup folder
                if (Directory.Exists(Path.Combine(LaunchBoxFolder, "Videos/StartupMarquee")))
                {
                    //Create the ThirdScreen startup folder for the main marquee
                    if (!Directory.Exists(Path.Combine(LaunchBoxFolder, "Videos/StartupThirdScreen")))
                    {
                        Directory.CreateDirectory(Path.Combine(LaunchBoxFolder, "Videos/StartupThirdScreen"));

                        if (!Directory.Exists(Path.Combine(LaunchBoxFolder, "Videos/StartupThirdScreen/Main Marquee")))
                        {
                            Directory.CreateDirectory(Path.Combine(LaunchBoxFolder, "Videos/StartupThirdScreen/Main Marquee"));
                        }
                    }

                    //Copy startup videos
                    foreach (var file in Directory.GetFiles(Path.Combine(LaunchBoxFolder, "Videos/StartupMarquee")))
                    {
                        String dest = Path.Combine(Path.Combine(LaunchBoxFolder, "Videos/StartupThirdScreen/Main Marquee"), Path.GetFileName(file));
                        if (!File.Exists(dest))
                        {
                            File.Copy(file, dest);
                        }
                    }
                }

                textbox_console.AppendText("******\r\n");
                textbox_console.AppendText("Installed ThirdScreen version 2.0.12.\r\nTo complete setup, go to LaunchBox->Tools->ThirdScreenSettings->Main Marquee and select the display to use for your marquee.\r\n");
                textbox_console.AppendText("******\r\n");
            }
            else
            {
                //ThirdScreen already installed

                //Check version. Replace files if older than 2.0.12. Do not overwrite the settings file.
                String ts_settings_path = Path.Combine(LaunchBoxFolder, "Data/ThirdScreenSettings.xml");
                if (File.Exists(ts_settings_path))
                {
                    XDocument tsSettingsDoc = null;
                    try { tsSettingsDoc = XDocument.Load(ts_settings_path); }
                    catch { tsSettingsDoc = null; }
                    if (tsSettingsDoc != null)
                    {
                        try
                        {
                            XElement ThirdScreenSettings = new XElement("ThirdScreenPlugin");

                            string XMLVersion = tsSettingsDoc
                                                .XPathSelectElement("/ThirdScreenPlugin/AppSettings")
                                                .Element("Version")
                                                .Value;

                            if (ThirdScreenVersion.Compare(XMLVersion, "2.0.12") < 0)
                            {
                                textbox_console.AppendText("Updating ThirdScreen to version 2.0.12.\r\n");
                                foreach (var ts_file in ThirdScreenFileList)
                                {
                                    if (ts_file.filename != "ThirdScreenSettings.xml")
                                        ts_file.Install();
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        private void DisplayHistory()
        {
            textbox_console.AppendText("Completed.\r\n\r\n");

            textbox_console.AppendText("What's new in v2.0:\r\n");
            textbox_console.AppendText(" - Removed Marquee support (replaced by ThirdScreen). \r\n");
            textbox_console.AppendText(" - Removed OmegaBigBoxMonitor. \r\n");
            textbox_console.AppendText(" - Removed OTA update support. \r\n");
            textbox_console.AppendText(" - No longer apply settings at startup (to improve startup time). \r\n");
            textbox_console.AppendText(" - Restored original unified redux marquee view files. \r\n");
            textbox_console.AppendText(" - Added BigBoxDailyReboot plugin (disabled by default). \r\n");
            textbox_console.AppendText(" - Added ThirdScreen plugin (if not already installed). \r\n");


            textbox_console.AppendText("What's new in v1.18:\r\n");
            textbox_console.AppendText(" - Restored the ManagePlatformVideoMarquees plugin that was removed in 1.17. \r\n");

            textbox_console.AppendText("What's new in v1.17:\r\n");
            textbox_console.AppendText(" - Removed MPV which had a bad memory leak. For movie file marquees switched from MPV to a new control derived from MediaElement. \r\n");

            textbox_console.AppendText("What's new in v1.16:\r\n");
            textbox_console.AppendText(" - OmegaBigBoxMonitor will now automatically restart BigBox if it runs out of memory.\r\n");

            textbox_console.AppendText("What's new in v1.15:\r\n");
            textbox_console.AppendText(" - For gif marquees, switched from the WpfAnimatedGif control included with LaunchBox to XamlAnimatedGif for better stability.\r\n");
            textbox_console.AppendText(" - Removed BigBoxMonitor shortcut from startup folder.\r\n");

            textbox_console.AppendText("What's new in v1.14:\r\n");
            textbox_console.AppendText(" - We now use RSA to cryptographically sign and authenticate OTA updates.\r\n");

            textbox_console.AppendText("What's new in v1.13:\r\n");
            textbox_console.AppendText(" - Ryan added Gun4IR license to the backup/restore utility.\r\n");

            textbox_console.AppendText("What's new in v1.12:\r\n");
            textbox_console.AppendText(" - Added Wii save data to the backup/restore utility.\r\n");

            textbox_console.AppendText("What's new in v1.11:\r\n");
            textbox_console.AppendText(" - Added menu option to check for OTA updates.\r\n");

            textbox_console.AppendText("What's new in v1.10.1:\r\n");
            textbox_console.AppendText(" - Ryan's updates for backup/restore of intro vids, high scores, stable IDs, Blinky, and BB License.\r\n");

            textbox_console.AppendText("What's new in v1.8.3:\r\n");
            textbox_console.AppendText(" - Fixed issue with static image game themes not displaying when video setting is set to WMP.\r\n");
            textbox_console.AppendText(" - Switched BigBox video setting from VLC to WMP for better stability.\r\n");

            textbox_console.AppendText("What's new in v1.8.2:\r\n");
            textbox_console.AppendText(" - We now force focus to the BigBox main screen intro video so that users will be able to skip the intro.\r\n");

            textbox_console.AppendText("What's new in v1.8.1:\r\n");
            textbox_console.AppendText(" - OmegaBigBoxMonitor is now smart enough to not reboot BigBox if a crash happened while it was trying to shut down.\r\n");

            textbox_console.AppendText("What's new in v1.7:\r\n");
            textbox_console.AppendText(" - Added video file motion marquee support for platforms,playlists, and games (in addition to the gif marquees we already support).\r\n");
            textbox_console.AppendText(" - We now display the clear logo if no marquee video or marquee image is available.\r\n");

            textbox_console.AppendText("What's new in v1.6:\r\n");
            textbox_console.AppendText(" - BigBox will now automatically restart if it unexpectedly crashes.\r\n");
            textbox_console.AppendText(" - Changes to marquee settings are now instantly previewed on the marquee.\r\n");
            textbox_console.AppendText(" - Marquee view xaml files have been updated to use the width from the settings menu instead of defaulting to fullscreen width.\r\n");

            textbox_console.AppendText("What's new in v1.5:\r\n");
            textbox_console.AppendText(" - Added utilities for importing and exporting favorites.\r\n");
            textbox_console.AppendText(" - Added automatic BigBox reboot if any settings have changed that require a reboot.\r\n");
            textbox_console.AppendText(" - For the adults-only wheel we now have separate options for just hiding the playlist vs hiding both the playlist and the games in their platform wheels.\r\n");
            textbox_console.AppendText(" - Display plugin version info in the settings menu.\r\n");
        }

        private void start_timer_Tick(object sender, EventArgs e)
        {
            start_timer.Enabled = false;

            if (InstallFileList.Count > 0)
            {
                //Install the next file
                InstallFileList.First().Install();
                InstallFileList.Remove(InstallFileList.First());
            }
            else if (DeleteFileList.Count > 0)
            {
                //Delete the next file
                DeleteFileList.First().Delete();
                DeleteFileList.Remove(DeleteFileList.First());
            }
            else
            {
                //Perform any additional actions
                AdditionalActions();

                //Update OmegaSupportPackage version number
                if(xSettingsDoc == null)
                {
                    //Create the settings file
                    XElement OmegaSettings = new XElement("OmegaSettings");
                    OmegaSettings.Add(new XElement("SupportPackageVersion", NewVersion));

                    xSettingsDoc = new XDocument();
                    xSettingsDoc.Add(OmegaSettings);
                }
                else
                {
                    var element = xSettingsDoc
                        .XPathSelectElement("/OmegaSettings")
                        .Element("SupportPackageVersion");

                    if(element == null)
                    {
                        xSettingsDoc
                        .XPathSelectElement("/OmegaSettings")
                        .Add(new XElement("SupportPackageVersion", NewVersion));
                    }
                    else
                    {
                        element.Value = NewVersion;
                    }
                }
                xSettingsDoc.Save(xml_path);

                button_complete.Visible = true;
                DisplayHistory();
                return;
            }

            //Delay and advance to the next item.       
            start_timer.Enabled = true;
        }

        private void button_complete_Click(object sender, EventArgs e)
        {
            //Restart BigBox
            Process ps_reboot_bigbox = null;
            ps_reboot_bigbox = new Process();
            ps_reboot_bigbox.StartInfo.UseShellExecute = false;
            ps_reboot_bigbox.StartInfo.RedirectStandardInput = false;
            ps_reboot_bigbox.StartInfo.RedirectStandardOutput = false;
            ps_reboot_bigbox.StartInfo.CreateNoWindow = true;
            ps_reboot_bigbox.StartInfo.UserName = null;
            ps_reboot_bigbox.StartInfo.Password = null;
            ps_reboot_bigbox.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            ps_reboot_bigbox.StartInfo.Arguments = "\"(Rebooting BigBox to complete installation)\"";
            ps_reboot_bigbox.StartInfo.FileName = LaunchBoxFolder + "/RebootBigBox.exe";

            if (File.Exists(ps_reboot_bigbox.StartInfo.FileName))
                ps_reboot_bigbox.Start();

            //Exit
            Thread.Sleep(4000);
            this.Close();
        }

        private void PrepareForInstall()
        {
            string[] args = Environment.GetCommandLineArgs();
            if ((args != null) && (args.Length > 1))
            {
                LaunchBoxFolder = args[1];
            }
            else
            {
                LaunchBoxFolder = "C:/Users/Administrator/LaunchBox";
            }


            //Kill BigBox process
            {
                Process[] Processes = System.Diagnostics.Process.GetProcesses();
                for (int i = 0; i < Processes.Length; i++)
                {
                    if (Processes[i].ProcessName.StartsWith("LaunchBox") ||
                        Processes[i].ProcessName.StartsWith("BigBox"))
                    {
                        Processes[i].Kill();
                    }
                }
            }

            //Check if BigBox is still running
            bool BigBoxRunning;
            do
            {
                Thread.Sleep(1000);

                BigBoxRunning = false;
                Process[] Processes = System.Diagnostics.Process.GetProcesses();

                for (int i = 0; i < Processes.Length; i++)
                {
                    if (Processes[i].ProcessName.StartsWith("LaunchBox") ||
                        Processes[i].ProcessName.StartsWith("BigBox"))
                    {
                        BigBoxRunning = true;
                    }
                }

            } while (BigBoxRunning);

            //Get the currently installed version
            xml_path = Path.Combine(LaunchBoxFolder, "Data/OmegaSettings.xml");
            try
            {
                xSettingsDoc = XDocument.Load(xml_path);
                CurrentVersion = xSettingsDoc
                        .XPathSelectElement("/OmegaSettings")
                        .Element("SupportPackageVersion").Value;
            }
            catch
            {
                CurrentVersion = "1.10"; //No version info. Assume an older version.
            }

            if(Convert.ToDouble(CurrentVersion) >= Convert.ToDouble(NewVersion))
            {
                String msg = "Currently installed version:" + CurrentVersion + "\r\n";
                msg += "New version:" + NewVersion + "\r\n\r\n";
                
                msg += (Convert.ToDouble(CurrentVersion) > Convert.ToDouble(NewVersion)) ?
                    "You are attempting to install a version of the Omega Support Package that is older than the currently installed version.\r\n\r\n" :
                    "You already have this version of the Omega Support Package installed.\r\n\r\n";

                msg += "Do you want to continue?";

                DialogResult dr = MessageBox.Show(msg,
                                   "Omega Support Package",
                                   MessageBoxButtons.YesNo,
                                   MessageBoxIcon.Question,
                                   MessageBoxDefaultButton.Button1);

                if (dr == DialogResult.No)
                    this.Close();
            }
        }

        private List<InstallFile> ThirdScreenFileList = new List<InstallFile>();

        private List<InstallFile> InstallFileList = new List<InstallFile>();
        private List<DeleteFile> DeleteFileList = new List<DeleteFile>();
        private String xml_path;
        XDocument xSettingsDoc = null;

        private String CurrentVersion;
        String LaunchBoxFolder;

    }
}
