using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IWshRuntimeLibrary;
using File = System.IO.File;
using System.Reflection;

namespace OmegaPackageInstaller
{
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

        public void Install()
        {
            if (System.IO.File.Exists(Path.Combine(install_path, filename)))
            {
                //Kill process if it is running
                if (Path.GetExtension(filename) == ".exe")
                {
                    Process[] Processes = System.Diagnostics.Process.GetProcesses();
                    for (int i = 0; i < Processes.Length; i++)
                    {
                        if (Processes[i].ProcessName.StartsWith(Path.GetFileNameWithoutExtension(filename)))
                        {
                            Processes[i].Kill();
                            OmegaSettingsInstaller.textbox_console.AppendText("Killing process " + filename + ".\r\n");
                            while (!Processes[i].HasExited) ;
                            Thread.Sleep(3000);
                        }
                    }
                }

                File.Delete(Path.Combine(install_path, filename));
                OmegaSettingsInstaller.textbox_console.AppendText("Deleted " + filename + ".\r\n");
            }

            //Create the parent directory if it doesn't exist.
            if (!Directory.Exists(install_path))
            {
                Directory.CreateDirectory(install_path);
                OmegaSettingsInstaller.textbox_console.AppendText("Created directory " + install_path + ".\r\n");
            }

            //Write the file.
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("OmegaPackageInstaller." + filename);
            FileStream fileStream = new FileStream(Path.Combine(install_path, filename), FileMode.CreateNew);
            for (int i = 0; i < stream.Length; i++)
                fileStream.WriteByte((byte)stream.ReadByte());
            fileStream.Close();

            OmegaSettingsInstaller.textbox_console.AppendText("Updated " + Path.Combine(install_path, filename) + ".\r\n");

            if (shortcut_dest != null)
            {
                String shortcut_path = Path.Combine(shortcut_dest, Path.GetFileNameWithoutExtension(filename) + ".lnk");
                if (Directory.Exists(shortcut_dest))
                {
                    string[] files = Directory.GetFiles(shortcut_dest);

                    for (int index = 0; index < files.Length; index++)
                    {
                        if (Path.GetFileNameWithoutExtension(files[index]) == Path.GetFileNameWithoutExtension(filename))
                        {
                            File.Delete(files[index]);
                            OmegaSettingsInstaller.textbox_console.AppendText("Deleted shortcut: " + files[index] + ".\r\n");
                        }
                    }

                    //Create the shortcut.
                    WshShell shell = new WshShell();
                    IWshShortcut link = (IWshShortcut)shell.CreateShortcut(shortcut_path);
                    link.TargetPath = Path.Combine(install_path, filename);
                    link.Save();

                    OmegaSettingsInstaller.textbox_console.AppendText("Updated shortcut " + shortcut_path + ".\r\n");
                }
            }
        }
    }
}
