using ExternalCounterstrike.CommandSystem;
using ExternalCounterstrike.ConsoleSystem;
using ExternalCounterstrike.CSGO;
using ExternalCounterstrike.CSGO.Models;
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
        private static OverlayWindow Overlay => ExternalCounterstrike.Overlay;
        private static void Main(string[] args)
        {
            CommandHandler.Setup();
            
            ThreadManager.Add(new ThreadFunction("CommandHandler", CommandHandler.Worker));
            ThreadManager.Add(new ThreadFunction("DrawingLoop", DrawingLoop));
            ThreadManager.Add(new ThreadFunction("SkinChanger", SkinChangerLoop));
            AttachToGame();
        }

        private static void SkinChangerLoop()
        {
            while(ExternalCounterstrike.IsAttached)
            {
                EntityBase.Update();
                var localPlayer = EntityBase.GetEntityList().GetLocalPlayer();
                if (localPlayer == null) continue;
                var weapon = localPlayer.GetWeapon();
                if (weapon == null) continue;
                weapon.ItemIDHigh = -1;
                weapon.PaintKit = 180;
            }
        }

        private static void DrawingLoop()
        {
            ExternalCounterstrike.Overlay = new OverlayWindow(ExternalCounterstrike.Process.MainWindowHandle, false);

            Overlay.Show();

            var greenColor = Color.FromArgb(255, Color.Green);
            var blackColor = Color.Black;

            var brushGreen = Overlay.Graphics.CreateBrush(greenColor);
            var brushBlack = Overlay.Graphics.CreateBrush(blackColor);
            var brushWhite = Overlay.Graphics.CreateBrush(Color.White);
            var oldAngle = new Vector3D();

            var font = Overlay.Graphics.CreateFont("Visitor TT2 (BRK)", 17);

            var pointCrosshair = new Vector2D(Overlay.Width / 2, Overlay.Height / 2);

            while (ExternalCounterstrike.IsAttached)
            {
                Thread.Sleep(0);
                if (ExternalCounterstrike.Process.HasExited)
                {
                    ExternalCounterstrike.IsAttached = false;
                    System.Environment.Exit(0);
                }
                EngineClient.ClearCache();
                BaseClient.ClearCache();
                //EntityList.ClearCache();
                EntityBase.ClearCache();

                var entityList = EntityBase.GetEntityList();

                // Begin scene of direct2d device to initialize drawing
                Overlay.Graphics.BeginScene();
                // Clear the scene so everything what we have drawn is getting deleted
                Overlay.Graphics.ClearScene();
                // Draw text (kek)
                Overlay.Graphics.DrawText("External Counterstrike", font, brushWhite, 10, 10);

                var localPlayer = entityList.GetLocalPlayer();
                if (localPlayer == null)
                {
                    Overlay.Graphics.EndScene();
                    continue;
                }

                foreach (var player in entityList.Players)
                {
                    if (player.IsDormant()) continue;
                    if (player.GetHealth() < 1) continue;
                    if (player == localPlayer) continue;

                    var origin = player.GetPosition();
                    var w2sOrigin = BaseClient.WorldToScreen(origin);
                    if (w2sOrigin.IsEmpty()) continue;

                    var brush = EngineClient.Map.IsVisible(localPlayer.GetEyePos(), origin) ? brushGreen : brushWhite;

                    Overlay.Graphics.DrawText(player.GetClientClass().GetClassName(), font, brush, w2sOrigin.X, w2sOrigin.Y);
                }

                //foreach(var ent in EntityList.Entities)
                //{
                //    var origin = ent.GetPosition();
                //    var w2sOrigin = BaseClient.WorldToScreen(origin);
                //    Overlay.Graphics.DrawText(ent.GetClientClass().GetClassName(), font, brushWhite, w2sOrigin.X, w2sOrigin.Y);
                //}

                // Tell the direct2d device to end the scene and apply all drawings
                Overlay.Graphics.EndScene();

                Vector3D newViewangle = EngineClient.ViewAngles + oldAngle;
                //var closestPlayer = GetClosestPlayer();
                //if (closestPlayer != null)
                //{
                //    var bone = closestPlayer.GetBonesPos(CommandHandler.GetParameter("aimbot", "bone").Value.ToInt32());
                //    newViewangle = CalculateAngle(localPlayer.GetEyePos(), bone);
                //}

                if (CommandHandler.GetParameter("misc", "norecoil").Value.ToBool())
                {
                    newViewangle -= localPlayer.GetAimPunchAngle() * BaseClient.ConVars.FindFast("weapon_recoil_scale").GetFloat();
                    oldAngle = localPlayer.GetAimPunchAngle() * BaseClient.ConVars.FindFast("weapon_recoil_scale").GetFloat();
                }
                BaseClient.ConVars.FindFast("sv_cheats").SetValue(1);

                localPlayer.FlashAlpha = CommandHandler.GetParameter("misc", "flashalpha").Value.ToFloat();

                EngineClient.ViewAngles = newViewangle;
            }
        }

        public static Vector3D CalculateAngle(Vector3D src, Vector3D dst)
        {
            var localPlayer = EntityBase.GetEntityList().GetLocalPlayer();
            var delta = new Vector3D { X = (src.X - dst.X), Y = (src.Y - dst.Y), Z = (src.Z - dst.Z) };
            var hyp = (float)System.Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
            var angles = new Vector3D();

            angles = new Vector3D
            {
                X = (float)(System.Math.Atan(delta.Z / hyp) * 57.295779513082f),
                Y = (float)(System.Math.Atan(delta.Y / delta.X) * 57.295779513082f),
                Z = 0.0f
            };

            if (delta.X >= 0.0) { angles.Y += 180.0f; }
            return angles;
        }

        private static BasePlayer GetClosestPlayer()
        {
            var fov = CommandHandler.GetParameter("aimbot", "fov").Value.ToInt32();
            var radius = fov * Overlay.Width / 90;
            var pointCrosshair = new Vector2D(Overlay.Width / 2, Overlay.Height / 2);

            BasePlayer result = null;
            var localPlayer = EntityBase.GetEntityList().GetLocalPlayer();
            float maxDistance = float.MaxValue;

            foreach (var player in EntityBase.GetEntityList().Players)
            {
                if (player == localPlayer) continue;
                if (player.GetTeam() == localPlayer.GetTeam()) continue;
                if (player.IsDormant()) continue;
                if (player.GetHealth() < 1) continue;
                if (!EngineClient.Map.IsVisible(localPlayer.GetEyePos(), player.GetBonesPos(6))) continue;

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

                Console.WriteLine("\n  Modules:");
                while (ExternalCounterstrike.ClientDll == null)
                {
                    Thread.Sleep(100);
                    ExternalCounterstrike.ClientDll = Utils.GetModuleHandle(ExternalCounterstrike.Process, "client.dll");
                }
                Console.WriteSuccess("  \tclient.dll | 0x" + ExternalCounterstrike.ClientDll.BaseAddress.ToString("X").PadLeft(8, '0') + "\t| " + Utils.ByteSizeToString(ExternalCounterstrike.ClientDll.ModuleMemorySize));

                while (ExternalCounterstrike.EngineDll == null)
                {
                    Thread.Sleep(100);
                    ExternalCounterstrike.EngineDll = Utils.GetModuleHandle(ExternalCounterstrike.Process, "engine.dll");
                }
                Console.WriteSuccess("  \tengine.dll | 0x" + ExternalCounterstrike.EngineDll.BaseAddress.ToString("X").PadLeft(8, '0') + "\t|  " + Utils.ByteSizeToString(ExternalCounterstrike.EngineDll.ModuleMemorySize));

                ExternalCounterstrike.Memory = new MemoryScanner(ExternalCounterstrike.Process);
                ExternalCounterstrike.SigScanner = new SignatureScanner(ExternalCounterstrike.Process);
                ExternalCounterstrike.Overlay = new OverlayWindow(ExternalCounterstrike.Process.MainWindowHandle, false);
                ExternalCounterstrike.IsAttached = true;
            }

            Console.WriteLine("\n  Offsets:");
            Console.WriteOffset("EntityBase", SignatureManager.GetEntityList());
            Console.WriteOffset("ConCommand", SignatureManager.GetConvarPtr());

            Console.WriteLine("\n  NetVars:");
            ExternalCounterstrike.NetVars = new SortedDictionary<string, int>();
            var attribute = NetvarManager.GetOffset("DT_BaseCombatWeapon", "m_Item") + NetvarManager.GetOffset("DT_BaseCombatWeapon", "m_AttributeManager");
            ExternalCounterstrike.NetVars.Add("m_vecAimPunch", NetvarManager.GetOffset("DT_BasePlayer", "m_Local") + NetvarManager.GetOffset("DT_BasePlayer", "m_aimPunchAngle"));
            ExternalCounterstrike.NetVars.Add("m_vecViewPunch", NetvarManager.GetOffset("DT_BasePlayer", "m_Local") + NetvarManager.GetOffset("DT_BasePlayer", "m_viewPunchAngle"));
            ExternalCounterstrike.NetVars.Add("m_ItemDefIndex", attribute + NetvarManager.GetOffset("DT_BaseCombatWeapon", "m_iItemDefinitionIndex"));
            ExternalCounterstrike.NetVars.Add("m_vecOrigin", NetvarManager.GetOffset("DT_BasePlayer", "m_vecOrigin"));
            ExternalCounterstrike.NetVars.Add("m_iHealth", NetvarManager.GetOffset("DT_BasePlayer", "m_iHealth"));
            ExternalCounterstrike.NetVars.Add("m_iTeamNum", NetvarManager.GetOffset("DT_BasePlayer", "m_iTeamNum"));
            ExternalCounterstrike.NetVars.Add("m_vecViewOffset", NetvarManager.GetOffset("DT_BasePlayer", "m_vecViewOffset[0]"));
            ExternalCounterstrike.NetVars.Add("m_dwIndex", 0x64);
            ExternalCounterstrike.NetVars.Add("m_dwBoneMatrix", NetvarManager.GetOffset("DT_BaseAnimating", "m_nForceBone") + 0x1C);
            ExternalCounterstrike.NetVars.Add("m_hActiveWeapon", NetvarManager.GetOffset("DT_BasePlayer", "m_hActiveWeapon"));
            ExternalCounterstrike.NetVars.Add("m_hOwner", NetvarManager.GetOffset("DT_BaseCombatWeapon", "m_hOwner"));
            ExternalCounterstrike.NetVars.Add("m_bDormant", SignatureManager.GetDormantOffset());
            ExternalCounterstrike.NetVars.Add("m_nPaintKit", NetvarManager.GetOffset("DT_BaseCombatWeapon", "m_nFallbackPaintKit"));
            ExternalCounterstrike.NetVars.Add("m_iItemIDHigh", attribute + NetvarManager.GetOffset("DT_BaseCombatWeapon", "m_iItemIDHigh"));
            ExternalCounterstrike.NetVars.Add("m_flFlashAlpha", NetvarManager.GetOffset("DT_CSPlayer", "m_flFlashMaxAlpha"));

            var sortedDict = from entry in ExternalCounterstrike.NetVars orderby entry.Value ascending select entry;
            foreach (var netvar in sortedDict)
            {
                Console.WriteOffset(netvar.Key, netvar.Value, true);
            }
            Console.WriteOffset("m_numHighest", ExternalCounterstrike.NetVars.Values.Max() + Marshal.SizeOf(typeof(Vector3D)), true);

            Console.WriteNotification("\n  Found and attached to it!\n");
            Console.WriteCommandLine();
            ThreadManager.Run("CommandHandler");
            ThreadManager.Run("DrawingLoop");
            //ThreadManager.Run("SkinChanger");
        }
    }
}
