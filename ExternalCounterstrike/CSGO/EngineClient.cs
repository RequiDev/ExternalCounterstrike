using ExternalCounterstrike.CSGO.BSP;
using ExternalCounterstrike.CSGO.Structs;
using ExternalCounterstrike.MemorySystem;
using System.Threading;

namespace ExternalCounterstrike.CSGO
{
    internal static class EngineClient
    {
        private static MemoryScanner Memory => ExternalCounterstrike.Memory;
        private static int clientState;
        private static int signOnState;
        private static int viewAngle;
        private static int localIndex;
        private static int mapName;
        private static int gameDir;
        private static string previousMap;
        private static Vector3D cachedViewAngle = new Vector3D();
        private static BspFile cachedMap;

        public static void ClearCache()
        {
            cachedViewAngle = Vector3D.Zero;
        }

        public static int ClientState
        {
            get
            {
                while (clientState == 0)
                {
                    Thread.Sleep(10);
                    clientState = SignatureManager.GetClientState();
                }
                return clientState;
            }
        }

        public static bool IsInMenu
        {
            get
            {
                if (signOnState == 0)
                    signOnState = SignatureManager.GetSignOnState();
                return Memory.Read<int>(ClientState + signOnState) == 0;
            }
        }

        public static bool IsInGame
        {
            get
            {
                if (signOnState == 0)
                    signOnState = SignatureManager.GetSignOnState();
                var state = Memory.Read<int>(ClientState + signOnState);
                return state > 1 || state < 7;
            }
        }

        public static int LocalPlayerIndex
        {
            get
            {
                if (localIndex == 0)
                {
                    localIndex = SignatureManager.GetLocalIndex();
                }
                var index = Memory.Read<int>(ClientState + localIndex);
                return index;
            }
        }

        public static string MapName
        {
            get
            {
                if(mapName == 0)
                {
                    mapName = SignatureManager.GetMapName();
                }
                var name = Memory.ReadString(ClientState + mapName);
                return name;
            }
        }

        public static string GameDirectory
        {
            get
            {
                if(gameDir == 0)
                {
                    gameDir = SignatureManager.GetGameDir();
                }
                return Memory.ReadString(gameDir);
            }
        }

        public static BspFile Map
        {
            get
            {
                var currentMap = MapName;
                if(previousMap != currentMap || cachedMap == null)
                {
                    previousMap = currentMap;
                    cachedMap = new BspFile($"{GameDirectory}\\{currentMap}");
                }
                return cachedMap;
            }
        }

        public static Vector3D ViewAngles
        {
            get
            {
                if (viewAngle == 0)
                    viewAngle = SignatureManager.GetViewAngle();
                if(cachedViewAngle == Vector3D.Zero)
                    cachedViewAngle = Memory.Read<Vector3D>(ClientState + viewAngle);
                return cachedViewAngle;
            }
            set
            {
                if (viewAngle == 0)
                    viewAngle = SignatureManager.GetViewAngle();
                Memory.Write(ClientState + viewAngle, value);

            }
        }
    }
}
