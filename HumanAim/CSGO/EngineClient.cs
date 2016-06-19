using HumanAim.CSGO.Structs;
using HumanAim.MemorySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HumanAim.CSGO
{
    internal class EngineClient
    {
        private static int clientState;
        private static int signOnState;
        private static int viewAngle;
        private static int localIndex;
        private static Vector3D cachedViewAngle = new Vector3D();

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
                return HumanAim.Memory.Read<int>(ClientState + signOnState) == 0;
            }
        }

        public static bool IsInGame
        {
            get
            {
                if (signOnState == 0)
                    signOnState = SignatureManager.GetSignOnState();
                var state = HumanAim.Memory.Read<int>(ClientState + signOnState);
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
                var index = HumanAim.Memory.Read<int>(ClientState + localIndex);
                return index;
            }
        }

        public static Vector3D ViewAngles
        {
            get
            {
                if (viewAngle == 0)
                    viewAngle = SignatureManager.GetViewAngle();
                if(cachedViewAngle == Vector3D.Zero)
                    cachedViewAngle = HumanAim.Memory.Read<Vector3D>(ClientState + viewAngle);
                return cachedViewAngle;
            }
            set
            {
                if (viewAngle == 0)
                    viewAngle = SignatureManager.GetViewAngle();
                HumanAim.Memory.Write(ClientState + viewAngle, value);

            }
        }
    }
}
