using HumanAim.CommandSystem;
using HumanAim.ConsoleSystem;
using HumanAim.MemorySystem;
using HumanAim.ThreadingSystem;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace HumanAim
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CommandHandler.Setup();
            ThreadManager.Add(new ThreadFunction("CommandHandler", CommandHandler.Worker));
            ThreadManager.Add(new ThreadFunction("AttachToGame", AttachToGame));
            ThreadManager.Run("AttachToGame");
        }

        private static void AttachToGame()
        {
            Console.WriteNotification($"\n  Looking for {HumanAim.GameName}...");
            while (!HumanAim.IsAttached)
            {
                Thread.Sleep(100);
                try
                {
                    HumanAim.Process = Process.GetProcessesByName(HumanAim.ProcessName).FirstOrDefault(p => p.Threads.Count > 0);
                    if (HumanAim.Process == null || !Utils.IsModuleLoaded(HumanAim.Process, "client.dll") || !Utils.IsModuleLoaded(HumanAim.Process, "engine.dll")) continue;
                }
                catch
                {
                    continue;
                }

                while (HumanAim.ClientDll == null)
                {
                    Thread.Sleep(100);
                    HumanAim.ClientDll = Utils.GetModuleHandle(HumanAim.Process, "client.dll");
                }

                while (HumanAim.EngineDll == null)
                {
                    Thread.Sleep(100);
                    HumanAim.EngineDll = Utils.GetModuleHandle(HumanAim.Process, "engine.dll");
                }
                HumanAim.Memory = new MemoryScanner(HumanAim.Process);
                HumanAim.SigScanner = new SignatureScanner(HumanAim.Process);
                HumanAim.IsAttached = true;
            }
            Console.WriteLine("\n  Modules:");
            Console.WriteSuccess("  \tclient.dll | 0x" + HumanAim.ClientDll.BaseAddress.ToString("X").PadLeft(8, '0') + "\t| " + Utils.ByteSizeToString(HumanAim.ClientDll.ModuleMemorySize));
            Console.WriteSuccess("  \tengine.dll | 0x" + HumanAim.EngineDll.BaseAddress.ToString("X").PadLeft(8, '0') + "\t|  " + Utils.ByteSizeToString(HumanAim.EngineDll.ModuleMemorySize));

            Console.WriteLine("\n  Offsets:");
            Console.WriteOffset("DT_BaseEntity", 888888);

            Console.WriteLine("\n  NetVars:");
            HumanAim.NetVars = new SortedDictionary<string, int>();
            HumanAim.NetVars.Add("m_aimPunchAngle", NetvarManager.GetOffset("DT_BasePlayer", "m_Local") + NetvarManager.GetOffset("DT_BasePlayer", "m_aimPunchAngle"));
            HumanAim.NetVars.Add("m_iHealth", NetvarManager.GetOffset("DT_BasePlayer", "m_iHealth"));
            HumanAim.NetVars.Add("m_iTeamNum", NetvarManager.GetOffset("DT_BasePlayer", "m_iTeamNum"));
            HumanAim.NetVars.Add("m_dwIndex", 0x64);
            HumanAim.NetVars.Add("m_dwBoneMatrix", NetvarManager.GetOffset("DT_BaseAnimating", "m_nForceBone") + 0x1C);
            var m_bDormant = SignatureManager.GetDormantOffset();
            HumanAim.NetVars.Add("m_bDormant", m_bDormant);

            var sortedDict = from entry in HumanAim.NetVars orderby entry.Value ascending select entry;
            foreach (var netvar in sortedDict)
            {
                Console.WriteOffset(netvar.Key, netvar.Value);
            }
            Console.WriteOffset("m_numHighest", HumanAim.NetVars.Values.Max());

            Console.WriteNotification("\n  Found and attached to it!\n");
            Console.WriteCommandLine();
            ThreadManager.Run("CommandHandler");
        }
    }
}
