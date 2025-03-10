using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Legacy_Launcher.Utils;
using System.Reflection;

namespace Legacy_Launcher.LaunchService
{
    public class LaunchService
    {
        public static Process FNLauncherProcess;
        public static Process FNAntiCheatProcess;
        public static Process FNEACProcess;
        public static Process FortniteGame;

        public static void InitializeLaunching(string ExchangeCode, string GamePath, float GameVer)
        {
            File.Copy(
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "GFSDK_Aftermath_Lib.x64.dll"),
                Path.Combine(GamePath, "Engine\\Binaries\\ThirdParty\\NVIDIA\\NVaftermath\\Win64", "GFSDK_Aftermath_Lib.x64.dll"),
                overwrite: true
            );
            Utils.Logger.good("Started launching the game client!");
            if (/*GameVer*/ true)
            {
                LaunchGame(ExchangeCode, GamePath, "-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -nobe -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck");
            } // ripped the launch args from eonv1 for now
            FakeAC.Start(GamePath, "FortniteClient-Win64-Shipping_BE.exe", $"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck");
            FakeAC.Start(GamePath, "FortniteLauncher.exe", $"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck");
            FakeAC.Start(GamePath, "FortniteClient-Win64-Shipping_EAC.exe", $"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck");
            LaunchService.FortniteGame.WaitForExit();
            try
            {
                LaunchService.FNLauncherProcess.Close();
                LaunchService.FNAntiCheatProcess.Close();
            }
            catch (Exception ex)
            {
                Utils.Logger.error("There has been a critical error: " + ex);
            }
        }

        public static void LaunchGame(string ExchangeCode, string GamePath, string Args) // args will change based on game version
        {
            if (File.Exists(Path.Combine(GamePath, "FortniteGame\\Binaries\\Win64\\", "FortniteClient-Win64-Shipping.exe")))
            {
                LaunchService.FortniteGame = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        Arguments = $"-AUTH_LOGIN=unused -AUTH_PASSWORD={ExchangeCode} AUTH_TYPE=exchangecode " + Args,
                        FileName = Path.Combine(GamePath, "FortniteGame\\Binaries\\Win64\\", "FortniteClient-Win64-Shipping.exe")
                    },
                    EnableRaisingEvents = true
                };
                LaunchService.FortniteGame.Exited += new EventHandler(LaunchService.OnFortniteExit);
                LaunchService.FortniteGame.Start();
                Utils.Logger.good("Successfully launched the fortnite game client!");
            }
        }

        public static void OnFortniteExit(object sender, EventArgs e)
        {
            Process fortniteProcess = LaunchService.FortniteGame;
            if (fortniteProcess != null && fortniteProcess.HasExited)
            {
                LaunchService.FortniteGame = (Process)null;
            }
            LaunchService.FNLauncherProcess?.Kill();
            LaunchService.FNAntiCheatProcess?.Kill();
            LaunchService.FNEACProcess?.Kill();
        }
    }

    public static class FreezeService
    {

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenThread(int dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32.dll")]
        private static extern uint SuspendThread(IntPtr hThread);

        public static void Freeze(this Process process)
        {

            foreach (object obj in process.Threads)
            {
                ProcessThread thread = (ProcessThread)obj;
                var Thread = OpenThread(2, false, (uint)thread.Id);
                if (Thread == IntPtr.Zero)
                {
                    break;
                }
                SuspendThread(Thread);
            }
        }
    }

    public static class FakeAC
    {
        public static void Start(string Path69, string FileName, string args = "")
        {
            try
            {
                if (File.Exists(Path.Combine(Path69, "FortniteGame\\Binaries\\Win64\\", FileName)))
                {
                    ProcessStartInfo ProcessIG = new ProcessStartInfo()
                    {
                        FileName = Path.Combine(Path69, "FortniteGame\\Binaries\\Win64\\", FileName),
                        Arguments = args,
                        CreateNoWindow = true,
                    };

                    if (FileName == "FortniteClient-Win64-Shipping_BE.exe")
                    {
                        LaunchService.FNAntiCheatProcess = Process.Start(ProcessIG);
                        if (LaunchService.FNAntiCheatProcess.Id == 0)
                        {
                            Utils.Logger.error("Failed To Start BattleEye Process!");
                        } else
                        {
                            Utils.Logger.good("Started BattleEye Process!");
                        }
                        LaunchService.FNAntiCheatProcess.Freeze();
                    } else if (FileName == "FortniteClient-Win64-Shipping_EAC.exe")
                    {
                        LaunchService.FNEACProcess = Process.Start(ProcessIG);
                        if (LaunchService.FNEACProcess.Id == 0)
                        {
                            Utils.Logger.error("Failed To Start EAC Process!");
                        }
                        else
                        {
                            Utils.Logger.good("Started EAC Process!");
                        }
                        LaunchService.FNEACProcess.Freeze();
                    }
                    else
                    {
                        LaunchService.FNLauncherProcess = Process.Start(ProcessIG);
                        if (LaunchService.FNLauncherProcess.Id == 0)
                        {
                            Utils.Logger.error("Failed To Start Launcher Process!");
                        } else
                        {
                            Utils.Logger.good("Started Launcher Process!");
                        }
                        LaunchService.FNLauncherProcess.Freeze();
                    }

                }
            }
            catch (Exception ex)
            {
                Utils.Logger.error("There has been a critical error: " + ex);
            }
        }
    }
}
