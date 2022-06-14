using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OmegaPackageInstaller
{
    class DeleteFile
    {
        public DeleteFile(String path, String file)
        {
            this.delete_path = path;
            this.filename = file;
            this.shortcut_dest = null;
        }
        public DeleteFile(String path, String file, String shortcut_dest)
        {
            this.delete_path = path;
            this.filename = file;
            this.shortcut_dest = shortcut_dest;
        }

        public String delete_path;
        public String filename;
        public String shortcut_dest;

        public void Delete()
        {
            //Delete the file if it exists
            if (File.Exists(Path.Combine(delete_path, filename)))
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

                File.Delete(Path.Combine(delete_path, filename));
                OmegaSettingsInstaller.textbox_console.AppendText("Deleted " + filename + ".\r\n");
            }

            //Delete the file's shortcut if one exists
            if (shortcut_dest != null)
            {
                String shortcut_path = Path.Combine(shortcut_dest, Path.GetFileNameWithoutExtension(filename) + ".lnk");
                if (File.Exists(shortcut_path))
                {
                    File.Delete(shortcut_path);
                    OmegaSettingsInstaller.textbox_console.AppendText("Deleted shortcut: " + shortcut_path + ".\r\n");
                }
            }

        }
    }
}
