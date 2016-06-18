using HumanAim.CommandSystem;
using HumanAim.ConsoleSystem;
using HumanAim.MemorySystem;
using HumanAim.ThreadingSystem;
using System.Diagnostics;
using System.Linq;

namespace HumanAim
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CommandHandler.Setup();
            ThreadManager.Add(new ThreadFunction("CommandHandler", CommandHandler.Worker));
            AttachToCSGO();
            ThreadManager.Run("CommandHandler");
        }

        private static void AttachToCSGO()
        {
            Console.WriteNotification("\n  Looking for Counter-Strike Global Offensive...");
            while (!HumanAim.IsAttached)
            {
                try
                {
                    HumanAim.Process = Process.GetProcessesByName("csgo").FirstOrDefault(p => p.Threads.Count > 0);
                    if (HumanAim.Process == null || !Utils.IsModuleLoaded(HumanAim.Process, "client.dll") || !Utils.IsModuleLoaded(HumanAim.Process, "engine.dll")) continue;
                }
                catch
                {
                    continue;
                }

                while (HumanAim.ClientDll == null)
                {
                    HumanAim.ClientDll = Utils.GetModuleHandle(HumanAim.Process, "client.dll");
                }

                while (HumanAim.EngineDll == null)
                {
                    HumanAim.EngineDll = Utils.GetModuleHandle(HumanAim.Process, "engine.dll");
                }
                HumanAim.Memory = new MemoryManager(HumanAim.Process);
                HumanAim.IsAttached = true;
            }
            Console.WriteLine("\n  Modules:");
            Console.WriteSuccess("  \tclient.dll | 0x" + HumanAim.ClientDll.BaseAddress.ToString("X").PadLeft(8, '0') + "\t| " + Utils.ByteSizeToString(HumanAim.ClientDll.ModuleMemorySize));
            Console.WriteSuccess("  \tengine.dll | 0x" + HumanAim.EngineDll.BaseAddress.ToString("X").PadLeft(8, '0') + "\t|  " + Utils.ByteSizeToString(HumanAim.EngineDll.ModuleMemorySize));

            Console.WriteLine("\n  Offsets:");
            Console.WriteOffset("DT_BaseEntity", 888888);

            Console.WriteLine("\n  NetVars:");
            Console.WriteOffset("m_iHealth", 88);

            Console.WriteNotification("\n  Found and attached to it!\n");
            Console.WriteCommandLine();
        }
    }
}
