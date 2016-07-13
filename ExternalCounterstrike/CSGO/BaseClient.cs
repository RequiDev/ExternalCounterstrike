using ExternalCounterstrike.CSGO.Structs;
using ExternalCounterstrike.MemorySystem;

namespace ExternalCounterstrike.CSGO
{
    internal static class BaseClient
    {
        private static MemoryScanner Memory => ExternalCounterstrike.Memory;
        private static bool readYet;
        private static int convarPtr;
        private static int w2sViewMatrixPtr;
        private static GlobalVars globalVars;
        private static ViewMatrix vMatrix;
        private static ConvarManager cvars;

        public static ConvarManager ConVars
        {
            get
            {
                if (cvars == null)
                {
                    if (convarPtr == 0)
                    {
                        convarPtr = SignatureManager.GetConvarPtr();
                    }
                    cvars = new ConvarManager(convarPtr);
                }
                return cvars;
            }
        }

        public static ViewMatrix ViewMatrix
        {
            get
            {
                if(w2sViewMatrixPtr == 0)
                {
                    w2sViewMatrixPtr = SignatureManager.GetWorldToViewMatrix();
                }
                if(!readYet)
                    vMatrix = Memory.Read<ViewMatrix>(w2sViewMatrixPtr);
                return vMatrix;
            }
        }

        public static Vector2D WorldToScreen(Vector3D world)
        {
            Vector2D vec2;
            Vector2D vector2D;
            ViewMatrix viewMatrix = ViewMatrix;
            float m41 = viewMatrix.M41 * world.X + viewMatrix.M42 * world.Y + viewMatrix.M43 * world.Z + viewMatrix.M44;
            if (m41 < 0.01)
            {
                vector2D = new Vector2D()
                {
                    X = 0f,
                    Y = 0f
                };
                vec2 = vector2D;
            }
            else
            {
                float single = 1f / m41;
                float m11 = (viewMatrix.M11 * world.X + viewMatrix.M12 * world.Y + viewMatrix.M13 * world.Z + viewMatrix.M14) * single;
                float m21 = (viewMatrix.M21 * world.X + viewMatrix.M22 * world.Y + viewMatrix.M23 * world.Z + viewMatrix.M24) * single;
                vector2D = new Vector2D()
                {
                    X = (m11 + 1f) * 0.5f * ExternalCounterstrike.Overlay.Width,
                    Y = (m21 - 1f) * -0.5f * ExternalCounterstrike.Overlay.Height
                };
                vec2 = vector2D;
            }
            return vec2;
        }

        public static void ClearCache()
        {
            readYet = false;
        }

        public static void Update()
        {
            globalVars = Memory.Read<GlobalVars>(ExternalCounterstrike.ClientDll.BaseAddress.ToInt32() + 0x1337);
        }
    }
}
