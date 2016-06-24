using ExternalCounterstrike.CSGO.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalCounterstrike.CSGO
{
    internal static class BaseClient
    {
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
                globalVars = ExternalCounterstrike.Memory.Read<GlobalVars>(ExternalCounterstrike.ClientDll.BaseAddress.ToInt32() + 0x1337);
            }
        }
    }
}
