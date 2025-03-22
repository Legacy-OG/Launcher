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
        public static Process FNEOSEACProcess;
        public static Process FortniteGame;

        public static void InitializeLaunching(string ExchangeCode, string GamePath, float GameVer)
        {
            File.Copy(
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "GFSDK_Aftermath_Lib.x64.dll"),
                Path.Combine(GamePath, "Engine\\Binaries\\ThirdParty\\NVIDIA\\NVaftermath\\Win64", "GFSDK_Aftermath_Lib.x64.dll"),
                overwrite: true
            );
            Utils.Logger.good("Started launching the game client!");
            if (Properties.Settings.Default.Season > 27)
            {
                LaunchEAC(ExchangeCode, GamePath, "-obfuscationid=PM3G4r-oF6DIG-fbJUON8rECU73FWA -AUTH_LOGIN=unused -AUTH_PASSWORD={ExchangeCode} AUTH_TYPE=exchangecode -epicapp=Fortnite -epicenv=Prod -EpicPortal -epiclocale=en -epicsandboxid=fn -nobe -noeac -fromfl=eaceos -caldera=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiUG9uZ28iLCJnZW5lcmF0ZWQiOjE3NDI2NzYxNDksImNhbGRlcmFHVUlEIjoiZjUyZGQxZGItYjA0OC00NWFmLTgwMDQtYjc5MWEzMGZhYzYwIiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXRFT1MiLCJub3RlcyI6IiIsInByZSI6ZmFsc2UsImZhbGxiYWNrIjpmYWxzZX0.KX6JMAd_dlxyyuZp6DFQ058LDmVo3qJNFrxyMyEQ70g -fltoken=77b9908d91e7aa96c6bd6814 ");
            }
            else {
                LaunchGame(ExchangeCode, GamePath, "-obfuscationid=PM3G4r-oF6DIG-fbJUON8rECU73FWA epicsandboxid=fn -epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -nobe -noeac -fromfl=eaceos -fltoken=77b9908d91e7aa96c6bd6814 -skippatchcheck caldera=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiUG9uZ28iLCJnZW5lcmF0ZWQiOjE3NDI2NzU0MjMsImNhbGRlcmFHVUlEIjoiY2YxMWU5N2YtNWUxYy00ZTVkLThiZmEtOWJkMzFhNDkzYWE3IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXRFT1MiLCJub3RlcyI6IiIsInByZSI6ZmFsc2UsImZhbGxiYWNrIjpmYWxzZX0.pePMt33y13XBdJSvGUA5_erD6Ahg9ojQMqJ-W2Xctp8");
                FakeAC.Start(GamePath, "FortniteClient-Win64-Shipping_BE.exe", $"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck");
                FakeAC.Start(GamePath, "FortniteLauncher.exe", $"");
                FakeAC.Start(GamePath, "FortniteClient-Win64-Shipping_EAC.exe", $"");
                FakeAC.Start(GamePath, "FortniteClient-Win64-Shipping_EAC_EOS.exe", $"-obfuscationid=PM3G4r-oF6DIG-fbJUON8rECU73FWA -AUTH_LOGIN=unused -AUTH_PASSWORD={ExchangeCode} AUTH_TYPE=exchangecode -epicapp=Fortnite -epicenv=Prod -EpicPortal -epiclocale=en -epicsandboxid=fn -nobe -noeac -fromfl=eaceos -caldera=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiUG9uZ28iLCJnZW5lcmF0ZWQiOjE3NDI2NzU4MDAsImNhbGRlcmFHVUlEIjoiNmQyY2MxZjgtNTE1OS00NTY5LTk3NzYtYzBjNzc3NjQzMTQ0IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXRFT1MiLCJub3RlcyI6IiIsInByZSI6ZmFsc2UsImZhbGxiYWNrIjpmYWxzZX0.HOCK1NFKEZEOhes7ghOb8IU7WdpSQzVG6pJ-BNsNlzw -fltoken=77b9908d91e7aa96c6bd6814 ");
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
            }// ripped the launch args from eonv1 for now
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

        public static void LaunchEAC(string ExchangeCode, string GamePath, string Args) // args will change based on game version
        {
            if (File.Exists(Path.Combine(GamePath, "FortniteGame\\Binaries\\Win64\\", "FortniteClient-Win64-Shipping_EAC_EOS.exe")))
            {
                LaunchService.FNEOSEACProcess = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        Arguments = $"-AUTH_LOGIN=unused -AUTH_PASSWORD={ExchangeCode} AUTH_TYPE=exchangecode " + Args,
                        FileName = Path.Combine(GamePath, "FortniteGame\\Binaries\\Win64\\", "FortniteClient-Win64-Shipping_EAC_EOS.exe")
                    },
                    EnableRaisingEvents = true
                };
                LaunchService.FNEOSEACProcess.Start();
                Utils.Logger.good("Successfully launched the EOS EasyAntiCheat!");
                //int result = DllInjector.InjectDLL(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "GFSDK_Aftermath_Lib.x64.dll"), LaunchService.FNEOSEACProcess);
                //Console.WriteLine(result);
                //LaunchService.FNEOSEACProcess.WaitForExit();
                //Process process = Process.GetProcessesByName("FortniteClient-Win64-Shipping")[0];
                //process.Exited += new EventHandler(LaunchService.OnFortniteExit);
            }
        }

        public static void OnFortniteExit(object sender, EventArgs e)
        {
            Process fortniteProcess = LaunchService.FortniteGame;
            if (fortniteProcess != null && fortniteProcess.HasExited)
            {
                LaunchService.FortniteGame = (Process)null;
            }
            LaunchService.FNLauncherProcess.Kill();
            LaunchService.FNAntiCheatProcess.Kill();
            LaunchService.FNEACProcess.Kill();
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
                    else if (FileName == "FortniteClient-Win64-Shipping_EAC_EOS.exe")
                    {
                        LaunchService.FNEOSEACProcess = Process.Start(ProcessIG);
                        if (LaunchService.FNEACProcess.Id == 0)
                        {
                            Utils.Logger.error("Failed To Start EOS EasyAntiCheat Process!");
                        }
                        else
                        {
                            Utils.Logger.good("Started EOS EasyAntiCheat Process!");
                        }
                        LaunchService.FNEOSEACProcess.Freeze();
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

    internal class DllInjector
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
            uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess,
            IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        // privileges
        const int PROCESS_CREATE_THREAD = 0x0002;
        const int PROCESS_QUERY_INFORMATION = 0x0400;
        const int PROCESS_VM_OPERATION = 0x0008;
        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_READ = 0x0010;

        // used for memory allocation
        const uint MEM_COMMIT = 0x00001000;
        const uint MEM_RESERVE = 0x00002000;
        const uint PAGE_READWRITE = 4;

        public static int InjectDLL(string dllName, Process target)
        {
            try
            {
                const int PROCESS_ALL_ACCESS = 0x1F0FFF;
                IntPtr procHandle = OpenProcess(PROCESS_ALL_ACCESS, false, target.Id);
                if (procHandle == IntPtr.Zero)
                {
                    return -1;
                }

                IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                if (loadLibraryAddr == IntPtr.Zero)
                {
                    return -1;
                }

                IntPtr allocMemAddress = VirtualAllocEx(procHandle, IntPtr.Zero, (uint)((dllName.Length + 1) * Marshal.SizeOf(typeof(char))), MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
                if (allocMemAddress == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    return -1;
                }

                if (!WriteProcessMemory(procHandle, allocMemAddress, Encoding.Default.GetBytes(dllName), (uint)((dllName.Length + 1) * Marshal.SizeOf(typeof(char))), out UIntPtr bytesWritten))
                {
                    return -1;
                }

                IntPtr remoteThread = CreateRemoteThread(procHandle, IntPtr.Zero, 0, loadLibraryAddr, allocMemAddress, 0, IntPtr.Zero);
                if (remoteThread == IntPtr.Zero)
                {
                    return -1;
                }

                return 0;
            }
            catch
            {
                return -1;
            }
        }
    }
}
