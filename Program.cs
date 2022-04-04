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
            Console.WriteLine("Attempting to find VRChat install path... (Method 1)");
            var b32 = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", null);
            if (b32 == null)
            {

                Console.WriteLine("Failed to find VRChat install path. (Method 1)");
                Console.WriteLine("Attempting to find VRChat install path... (Method 2)");
                var b64 = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", null);
                if (b64 == null)
                {
                    Console.WriteLine("Failed to find VRChat install path. (Method 2)");
                    Console.WriteLine("Attempting to find VRChat install path... (Method 3)");
                    var alt = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "SteamPath", null);
                    if (alt == null)
                    {
                        MessageBox.Show("Could not find VRChat install path automatically. Please select the path manually.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
                    Install(Path.Combine(appPath, "VRChat.exe"));
                }
            }
            else
            {
                FileDialog();
            }
        }

        private static void FileDialog()
        {
            Application.EnableVisualStyles();
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
            Console.WriteLine("Installing to " + path);
            string dir = Path.GetDirectoryName(path);
            string file = Path.GetFileName(path);
            FileInfo fi = new FileInfo(path);

            if (file.ToLower() == "vrchat.exe")
            {
                using (WebClient c = new WebClient())
                {

                    Console.WriteLine("Downloading MelonLoader...");
                    c.DownloadFile("https://github.com/LavaGang/MelonLoader/releases/latest/download/MelonLoader.x64.zip", fi.Directory.FullName + "\\MelonLoader.x64.zip");

                    try
                    {
                        Console.WriteLine("Extracting MelonLoader...");
                        ZipArchive zipArchive = ZipFile.OpenRead(fi.Directory.FullName + "\\MelonLoader.x64.zip");
                        foreach (ZipArchiveEntry entry in zipArchive.Entries) { entry.ExtractToFile(fi.Directory.FullName + entry.Name, true); }
                        zipArchive.Dispose();
                    }
                    catch { }

                    try { Console.WriteLine("Removing MelonLoader Temp files..."); ; File.Delete(fi.Directory.FullName + "\\MelonLoader.x64.zip"); } catch { }


                    Console.WriteLine("Downloading Mod...");
                    Directory.CreateDirectory(fi.Directory.FullName + "\\Mods");
                    c.DownloadFile("https://github.com/CodeAngel3/RipperStoreCredits/releases/latest/download/RipperStoreCredits.dll", fi.Directory.FullName + "\\Mods\\RipperStoreCredits.dll");
                    Console.WriteLine("\n----\nDone!\nYou can now launch VRChat and use the RipperStoreCredits mod.\nDon't forget to enter your Ripper.Store API key when prompted during the first launch.");
                    Console.WriteLine("\nPress any key to exit...");
                    Console.ReadKey();
                }
            }
        }
    }
}
