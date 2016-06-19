using HumanAim.CommandSystem;
using HumanAim.ConsoleSystem;
using HumanAim.CSGO;
using HumanAim.CSGO.Structs;
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
            ThreadManager.Add(new ThreadFunction("Example", Example));
            ThreadManager.Run("AttachToGame");
        }

        private static void Example()
        {
            while(HumanAim.IsAttached)
            {
                Thread.Sleep(1);
                if (EngineClient.IsInMenu) continue;
                EngineClient.ClearCache();
                BaseClient.ClearCache();
                BaseClient.Update();
                var localPlayer = BaseClient.LocalPlayer;
                if (localPlayer == null)
                    continue;

                var closestPlayer = GetClosestPlayer();
                if (closestPlayer == null)
                    continue;

                var bone = closestPlayer.GetBonesPos(6);
                var calculatedBone = CalculateAngle(localPlayer.GetEyePos(), bone);
                EngineClient.ViewAngles = calculatedBone;
            }
        }

        public static Vector3D CalculateAngle(Vector3D src, Vector3D dst, bool usePunch = true)
        {
            var localPlayer = BaseClient.LocalPlayer;
            var delta = new Vector3D { X = (src.X - dst.X), Y = (src.Y - dst.Y), Z = (src.Z - dst.Z) };
            var hyp = (float)System.Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
            var angles = new Vector3D();

            angles = new Vector3D
            {
                X = (float)(System.Math.Atan(delta.Z / hyp) * 57.295779513082f),
                Y = (float)(System.Math.Atan(delta.Y / delta.X) * 57.295779513082f),
                Z = 0.0f
            };

            if (usePunch) angles -= localPlayer.GetPunchAngle() * 2.0f;

            if (delta.X >= 0.0) { angles.Y += 180.0f; }
            return angles;
        }

        private static BaseEntity GetClosestPlayer()
        {
            var fov = CommandHandler.GetParameter("aimbot", "fov").Value.ToInt32();
            var radius = fov * (1080 / 90);
            var pointCrosshair = new Vector2D(960, 540);

            BaseEntity result = null;
            var localPlayer = BaseClient.LocalPlayer;
            float maxDistance = float.MaxValue;

            foreach(var player in BaseClient.PlayerList)
            {
                if (player == localPlayer) continue;
                if (player.GetTeam() == localPlayer.GetTeam()) continue;
                if (player.IsDormant()) continue;
                if (player.GetHealth() < 1) continue;

                var distance = Vector3D.Distance(localPlayer.GetPosition(), player.GetBonesPos(6));

                if(distance < maxDistance)
                {
                    maxDistance = distance;
                    result = player;
                }
            }
            return result;
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
            HumanAim.NetVars.Add("m_vecOrigin", NetvarManager.GetOffset("DT_BasePlayer", "m_vecOrigin"));
            HumanAim.NetVars.Add("m_iHealth", NetvarManager.GetOffset("DT_BasePlayer", "m_iHealth"));
            HumanAim.NetVars.Add("m_iTeamNum", NetvarManager.GetOffset("DT_BasePlayer", "m_iTeamNum"));
            HumanAim.NetVars.Add("m_vecViewOffset[0]", NetvarManager.GetOffset("DT_BasePlayer", "m_vecViewOffset[0]"));
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
            ThreadManager.Run("Example");
        }
    }
}
