using System.Diagnostics;

namespace HumanAim
{
    internal static class HumanAim
    {
        public static bool IsAttached { get; set; }
        public static Process Process { get; set; }
        public static ProcessModule ClientDll { get; set; }
        public static ProcessModule EngineDll { get; set; }
        public static MemorySystem.MemoryManager Memory { get; set; }
    }
}