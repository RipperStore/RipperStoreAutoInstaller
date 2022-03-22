using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows.Forms;
using Microsoft.Win32;

namespace RipperStoreAutoInstaller
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            string steamPath = null;
            var b32 = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", null);
            if (b32 == null)
            {
                var b64 = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", null);
                if (b64 == null)
                {
                    var alt = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "SteamPath", null);
                    if (alt == null)
                    {
                        MessageBox.Show("Could not find Steam install path. Please select the path manually.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        FileDialog();
                    }
                    else steamPath = alt.ToString();
                }
                else steamPath = b64.ToString();
            }
            else steamPath = b32.ToString();

            if (!string.IsNullOrEmpty(steamPath))
            {
                var appPath = Path.Combine(steamPath, "steamapps", "common", "VRChat");

                if (!Directory.Exists(appPath))
                {
                    MessageBox.Show("Could not find VRChat install path. Please select the path manually.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    FileDialog();
                }
                else
                {
                    Install(appPath);
                }
            }
            else
            {
                FileDialog();
            }
        }

        private static void FileDialog()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Please navigate to the game install directory and select VRChat.exe";
            ofd.Filter = "VRChat (*.exe)|*.exe";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string path = ofd.FileName;

                Install(path);
            }
        }

        private static void Install(string path)
        {
            string dir = Path.GetDirectoryName(path);
            string file = Path.GetFileName(path);
            FileInfo fi = new FileInfo(path);

            if (file.ToLower() == "vrchat.exe")
            {
                using (WebClient c = new WebClient())
                {
                    c.DownloadFile("https://github.com/LavaGang/MelonLoader/releases/latest/download/MelonLoader.x64.zip", fi.Directory.FullName + "\\MelonLoader.x64.zip");

                    try
                    {
                        ZipArchive zipArchive = ZipFile.OpenRead(fi.Directory.FullName + "\\MelonLoader.x64.zip");
                        foreach (ZipArchiveEntry entry in zipArchive.Entries) { entry.ExtractToFile(fi.Directory.FullName + entry.Name, true); }
                        zipArchive.Dispose();
                    }
                    catch { }

                    try { File.Delete(fi.Directory.FullName + "\\MelonLoader.x64.zip"); } catch { }

                    c.DownloadFile("https://github.com/CodeAngel3/RipperStoreCredits/releases/latest/download/RipperStoreCredits.dll", fi.Directory.FullName + "\\Mods\\RipperStoreCredits.dll");

                    MessageBox.Show("Successfully installed, launch vrchat to proceed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
                }
            }
            else
            {
                MessageBox.Show("Invalid .exe detected, Please select your 'VRChat.exe'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FileDialog();
            }
        }
    }
}
