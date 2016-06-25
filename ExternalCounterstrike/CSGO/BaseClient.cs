using ExternalCounterstrike.CSGO.Structs;
using ExternalCounterstrike.MemorySystem;

namespace ExternalCounterstrike.CSGO
{
    internal static class BaseClient
    {
        private static MemoryScanner Memory => ExternalCounterstrike.Memory;
        private static readonly object lockObj = new object();
        private static GlobalVars globalVars;

        public static GlobalVars GlobalVars
        {
            get
            {
                return globalVars;
            }
        }
        public static void Update()
        {
            lock (lockObj)
            {
                globalVars = Memory.Read<GlobalVars>(ExternalCounterstrike.ClientDll.BaseAddress.ToInt32() + 0x1337);
            }
        }
    }
}
