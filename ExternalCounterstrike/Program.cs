using ExternalCounterstrike.CommandSystem;
using ExternalCounterstrike.ConsoleSystem;
using ExternalCounterstrike.CSGO;
using ExternalCounterstrike.CSGO.Structs;
using ExternalCounterstrike.MemorySystem;
using ExternalCounterstrike.Overlay;
using ExternalCounterstrike.ThreadingSystem;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace ExternalCounterstrike
{
    internal class Program
    {
        private static MemoryScanner Memory => ExternalCounterstrike.Memory;
        private static OverlayWindow Overlay;
        private static void Main(string[] args)
        {
            CommandHandler.Setup();
            
            ThreadManager.Add(new ThreadFunction("CommandHandler", CommandHandler.Worker));
            ThreadManager.Add(new ThreadFunction("AttachToGame", AttachToGame));
            ThreadManager.Add(new ThreadFunction("Example", Example));
            ThreadManager.Add(new ThreadFunction("DrawingLoop", DrawingLoop));
            ThreadManager.Run("AttachToGame");
        }

        private static void Example()
        {
            while (ExternalCounterstrike.IsAttached)
            {
                Thread.Sleep(1);
                ExternalCounterstrike.IsAttached = !ExternalCounterstrike.Process.HasExited;
                if (EngineClient.IsInMenu) continue;
                EngineClient.ClearCache();
                
                EntityList.Update();
                var localPlayer = EntityList.GetLocalPlayer();
                if (localPlayer == null)
                    continue;

                var closestPlayer = GetClosestPlayer();
                if (closestPlayer == null)
                    continue;

                var bone = closestPlayer.GetBonesPos(CommandHandler.GetParameter("aimbot", "bone").Value.ToInt32());
                var calculatedBone = CalculateAngle(localPlayer.GetEyePos(), bone);
                EngineClient.ViewAngles = calculatedBone;
            }
        }

        private static void DrawingLoop()
        {
            Overlay = new OverlayWindow(ExternalCounterstrike.Process.MainWindowHandle, false);
            Overlay.Show();

            var greenColor = Color.FromArgb(255, Color.Green);
            var blackColor = Color.Black;

            var brushGreen = Overlay.Graphics.CreateBrush(greenColor);
            var brushBlack = Overlay.Graphics.CreateBrush(blackColor);
            var brushWhite = Overlay.Graphics.CreateBrush(Color.White);

            var font = Overlay.Graphics.CreateFont("Visitor TT2 (BRK)", 17);
            var memes = Overlay.Graphics.CreateFont("Comic Sans MS", 500);
            var pointCrosshair = new Vector2D(Overlay.Width / 2, Overlay.Height / 2);

            while (ExternalCounterstrike.IsAttached)
            {
                Thread.Sleep(10);
                // Begin scene of direct2d device to initialize drawing
                Overlay.Graphics.BeginScene();
                // Clear the scene so everything what have drawn is getting deleted
                Overlay.Graphics.ClearScene();
                // Draw text (kek)
                Overlay.Graphics.DrawText("External Counterstrike", font, brushWhite, 10, 10);
                Overlay.Graphics.DrawText("MEMES", memes, brushWhite, pointCrosshair.X - 900, pointCrosshair.Y - 400);

                // Tell the direct2d device to end the scene and apply all drawings
                Overlay.Graphics.EndScene();
            }
        }

        public static Vector3D CalculateAngle(Vector3D src, Vector3D dst, bool usePunch = true)
        {
            var localPlayer = EntityList.GetLocalPlayer();
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

        private static BasePlayer GetClosestPlayer()
        {
            var fov = CommandHandler.GetParameter("aimbot", "fov").Value.ToInt32();
            var radius = fov * (1080 / 90);
            var pointCrosshair = new Vector2D(960, 540);

            BasePlayer result = null;
            var localPlayer = EntityList.GetLocalPlayer();
            float maxDistance = float.MaxValue;

            foreach(var player in EntityList.Players)
            {
                if (player == localPlayer) continue;
                if (player.GetTeam() == localPlayer.GetTeam()) continue;
                if (player.IsDormant()) continue;
                if (player.GetHealth() < 1) continue;

                var distance = Vector3D.Distance(localPlayer.GetPosition(), player.GetPosition());

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
            Console.WriteNotification($"\n  Looking for {ExternalCounterstrike.GameName}...");
            while (!ExternalCounterstrike.IsAttached)
            {
                Thread.Sleep(100);
                try
                {
                    ExternalCounterstrike.Process = Process.GetProcessesByName(ExternalCounterstrike.ProcessName).FirstOrDefault(p => p.Threads.Count > 0);
                    if (ExternalCounterstrike.Process == null || !Utils.IsModuleLoaded(ExternalCounterstrike.Process, "client.dll") || !Utils.IsModuleLoaded(ExternalCounterstrike.Process, "engine.dll")) continue;
                }
                catch
                {
                    continue;
                }

                while (ExternalCounterstrike.ClientDll == null)
                {
                    Thread.Sleep(100);
                    ExternalCounterstrike.ClientDll = Utils.GetModuleHandle(ExternalCounterstrike.Process, "client.dll");
                }

                while (ExternalCounterstrike.EngineDll == null)
                {
                    Thread.Sleep(100);
                    ExternalCounterstrike.EngineDll = Utils.GetModuleHandle(ExternalCounterstrike.Process, "engine.dll");
                }
                ExternalCounterstrike.Memory = new MemoryScanner(ExternalCounterstrike.Process);
                ExternalCounterstrike.SigScanner = new SignatureScanner(ExternalCounterstrike.Process);
                ExternalCounterstrike.IsAttached = true;
            }
            Console.WriteLine("\n  Modules:");
            Console.WriteSuccess("  \tclient.dll | 0x" + ExternalCounterstrike.ClientDll.BaseAddress.ToString("X").PadLeft(8, '0') + "\t| " + Utils.ByteSizeToString(ExternalCounterstrike.ClientDll.ModuleMemorySize));
            Console.WriteSuccess("  \tengine.dll | 0x" + ExternalCounterstrike.EngineDll.BaseAddress.ToString("X").PadLeft(8, '0') + "\t|  " + Utils.ByteSizeToString(ExternalCounterstrike.EngineDll.ModuleMemorySize));

            var cvarptr = SignatureManager.FindConvar();
            Console.WriteLine("\n  Offsets:");
            Console.WriteOffset("EntityBase", 0x04A4BA64);
            Console.WriteOffset("ConCommand", cvarptr);

            Console.WriteLine("\n  NetVars:");
            ExternalCounterstrike.NetVars = new SortedDictionary<string, int>();
            ExternalCounterstrike.NetVars.Add("m_aimPunchAngle", NetvarManager.GetOffset("DT_BasePlayer", "m_Local") + NetvarManager.GetOffset("DT_BasePlayer", "m_aimPunchAngle"));
            ExternalCounterstrike.NetVars.Add("m_vecOrigin", NetvarManager.GetOffset("DT_BasePlayer", "m_vecOrigin"));
            ExternalCounterstrike.NetVars.Add("m_iHealth", NetvarManager.GetOffset("DT_BasePlayer", "m_iHealth"));
            ExternalCounterstrike.NetVars.Add("m_iTeamNum", NetvarManager.GetOffset("DT_BasePlayer", "m_iTeamNum"));
            ExternalCounterstrike.NetVars.Add("m_vecViewOffset", NetvarManager.GetOffset("DT_BasePlayer", "m_vecViewOffset[0]"));
            ExternalCounterstrike.NetVars.Add("m_dwIndex", 0x64);
            ExternalCounterstrike.NetVars.Add("m_dwBoneMatrix", NetvarManager.GetOffset("DT_BaseAnimating", "m_nForceBone") + 0x1C);
            var m_bDormant = SignatureManager.GetDormantOffset();
            ExternalCounterstrike.NetVars.Add("m_bDormant", m_bDormant);

            var sortedDict = from entry in ExternalCounterstrike.NetVars orderby entry.Value ascending select entry;
            foreach (var netvar in sortedDict)
            {
                Console.WriteOffset(netvar.Key, netvar.Value, true);
            }
            Console.WriteOffset("m_numHighest", ExternalCounterstrike.NetVars.Values.Max());

            Console.WriteNotification("\n  Found and attached to it!\n");
            var cvar = new ConvarManager(cvarptr);
            var sv_cheats = cvar.FindFast("name");
            Console.WriteLine($"\"{sv_cheats.GetName()}\" = \"{sv_cheats.GetString()}\"( def. \"{sv_cheats.GetDefaultValue()}\" )\t- {sv_cheats.GetDescription()}");
            Console.WriteCommandLine();
            ThreadManager.Run("CommandHandler");
            ThreadManager.Run("Example");

        }
    }
}
