using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
        String LaunchBoxFolder;

        public OmegaSettingsInstaller()
        {
            InitializeComponent();

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

            //Only pull in what was modified from the last baseline release...
            //            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\", "RebootBigBox.exe"));
            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\Plugins\\", "OmegaSettingsMenu.dll"));
            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\Plugins\\", "XamlAnimatedGif.dll"));
            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\Themes\\Unified Redux\\Views\\", "GameMarqueeView.xaml"));
            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\Themes\\Unified Redux\\Views\\", "PlatformMarqueeView.xaml"));
//           InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\Themes\\Unified Redux\\Views\\", "WheelGamesView.xaml"));
//            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\Themes\\Unified Redux\\Views\\", "Wheel2GamesView.xaml"));
//            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\Themes\\Unified Redux\\Views\\", "Wheel3GamesView.xaml"));
//            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\Themes\\Unified Redux\\Views\\", "Wheel4GamesView.xaml"));

//            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\", "BigBoxWithStartupMarquee.exe", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)));
//            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\Plugins\\", "OmegaStartupMarquee.dll"));

            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\", "OmegaBigBoxMonitor.exe"));

//            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\Plugins\\", "ManagePlatformVideoMarquees.dll"));
//            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\Plugins\\", "MediaPlayer.Wpf.dll"));
//            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\Plugins\\", "MediaPlayer.Wpf.Mpv.dll"));
//            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\Plugins\\", "Mpv.NET.dll"));

//            InstallFileList.Add(new InstallFile(LaunchBoxFolder + "\\ThirdParty\\MPV\\", "mpv-2.dll"));

            start_timer.Interval = 250;
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

            if(File.Exists(ps_reboot_bigbox.StartInfo.FileName))
                ps_reboot_bigbox.Start();

            //Exit
            Thread.Sleep(4000);
            this.Close();
        }

        private void start_timer_Tick(object sender, EventArgs e)
        {
            if (InstallFileList.Count == 0)
                this.Close();
            else
            {
                InstallFile file = InstallFileList[count];

                if (removing)
                {
                    if (File.Exists(Path.Combine(file.install_path, file.filename)))
                    {
                        //Kill process if it is running
                        if(Path.GetExtension(file.filename) == ".exe")
                        {
                            Process[] Processes = System.Diagnostics.Process.GetProcesses();
                            for (int i = 0; i < Processes.Length; i++)
                            {
                                if (Processes[i].ProcessName.StartsWith(Path.GetFileNameWithoutExtension(file.filename)))
                                {
                                    Processes[i].Kill();
                                    textbox_console.AppendText("Killing process " + file.filename + ".\r\n");
                                    while (!Processes[i].HasExited) ;
                                    Thread.Sleep(3000);
                                }
                            }
                        }

                        File.Delete(Path.Combine(file.install_path, file.filename));
                        textbox_console.AppendText("Deleted " + file.filename + ".\r\n");
                    }

                    if (++count >= InstallFileList.Count)
                    {
                        removing = false;
                        count = 0;
                    }
                }
                else
                {
                    //Create the parent directory if it doesn't exist.
                    if(!Directory.Exists(file.install_path))
                    {
                        Directory.CreateDirectory(file.install_path);
                        textbox_console.AppendText("Created directory " + file.install_path + ".\r\n");
                    }

                    //Write the file.
                    Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("OmegaPackageInstaller." + file.filename);
                    FileStream fileStream = new FileStream(Path.Combine(file.install_path, file.filename), FileMode.CreateNew);
                    for (int i = 0; i < stream.Length; i++)
                        fileStream.WriteByte((byte)stream.ReadByte());
                    fileStream.Close();

                    textbox_console.AppendText("Updated " + Path.Combine(file.install_path, file.filename) + ".\r\n");

                    if(file.shortcut_dest != null)
                    {
                        String shortcut_path = Path.Combine(file.shortcut_dest, Path.GetFileNameWithoutExtension(file.filename) + ".lnk");
                        if (Directory.Exists(file.shortcut_dest))
                        {
                            string[] files = Directory.GetFiles(file.shortcut_dest);

                            for(int index = 0; index < files.Length; index++)
                            {
                                if(Path.GetFileNameWithoutExtension(files[index])  == Path.GetFileNameWithoutExtension(file.filename))
                                {
                                    File.Delete(files[index]);
                                    textbox_console.AppendText("Deleted shortcut: " + files[index] + ".\r\n");
                                }
                            }

                            //Create the shortcut.
                            WshShell shell = new WshShell();
                            IWshShortcut link = (IWshShortcut)shell.CreateShortcut(shortcut_path);
                            link.TargetPath = Path.Combine(file.install_path, file.filename);
                            link.Save();

                            textbox_console.AppendText("Updated shortcut " + shortcut_path + ".\r\n");
                        }
                    }

                    if (++count >= InstallFileList.Count)
                    {
                        start_timer.Enabled = false;


                        ////Set BigBox video setting to WMP (for v1.8.3)
                        //XDocument xdoc = XDocument.Load(LaunchBoxFolder + "/Data/BigBoxSettings.xml");
                        //var element = xdoc
                        //        .XPathSelectElement("/LaunchBox/BigBoxSettings")
                        //        .Element("VideoPlaybackEngine");
                        //element.Value = "Windows Media Player";
                        //xdoc.Save(LaunchBoxFolder + "/Data/BigBoxSettings.xml");
                        //textbox_console.AppendText("Set BigBox video setting to Windows Media Player.\r\n");

                        ////Adjust Background image priorities (for v1.8.3)
                        //xdoc = XDocument.Load(LaunchBoxFolder + "/Data/Settings.xml");
                        //element = xdoc
                        //        .XPathSelectElement("/LaunchBox/Settings")
                        //        .Element("BackgroundImageTypePriorities");
                        //element.Value = "Screenshot - Game Title,Screenshot - Game Select,Screenshot - Gameplay,Epic Games Background,Uplay Background,Origin Background,Amazon Background,Fanart - Background,Advertisement Flyer - Front,Arcade - Cabinet,Clear Logo";
                        //xdoc.Save(LaunchBoxFolder + "/Data/Settings.xml");
                        //textbox_console.AppendText("Adjusted LaunchBox background image path priorities to support static image themes.\r\n");


                        //Remove BigBoxMonitor.exe startup shortcut (for v1.15)
                        String montor_shortcut = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "OmegaBigBoxMonitor.lnk");
                        if (File.Exists(montor_shortcut))
                        {
                            File.Delete(montor_shortcut);
                            textbox_console.AppendText("Deleted shortcut: " + montor_shortcut + ".\r\n");
                        }

                        //Start the monitor
                        Process ps_monitor = new Process();
                        ps_monitor.StartInfo.UseShellExecute = false;
                        ps_monitor.StartInfo.RedirectStandardInput = false;
                        ps_monitor.StartInfo.RedirectStandardOutput = false;
                        ps_monitor.StartInfo.CreateNoWindow = true;
                        ps_monitor.StartInfo.UserName = null;
                        ps_monitor.StartInfo.Password = null;
                        ps_monitor.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                        ps_monitor.StartInfo.FileName = LaunchBoxFolder + "/OmegaBigBoxMonitor.exe";
                        if (File.Exists(ps_monitor.StartInfo.FileName))
                        {
                            ps_monitor.Start();
                        }
                        textbox_console.AppendText("Started Omega BigBox Monitor.\r\n");



                        button_complete.Visible = true;
                        textbox_console.AppendText("Completed.\r\n\r\n");

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
                }
            }
        }

        private FolderBrowserDialog dlg;
        private List<InstallFile> InstallFileList = new List<InstallFile>();
        private int count = 0;
        private Boolean removing = true;
    }


    class InstallFile
    {
        public InstallFile(String path, String file)
        {
            this.install_path = path;
            this.filename = file;
            this.shortcut_dest = null;
        }
        public InstallFile(String path, String file, String shortcut_dest)
        {
            this.install_path = path;
            this.filename = file;
            this.shortcut_dest = shortcut_dest;
        }

        public String install_path;
        public String filename;
        public String shortcut_dest;
    }
}
