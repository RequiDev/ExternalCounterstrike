using System;
using System.Diagnostics;
using System.Text;

namespace ExternalCounterstrike.MemorySystem
{
    internal static class SignatureManager
    {
        private static MemoryScanner Memory => ExternalCounterstrike.Memory;

        public static int GetViewAngle()
        {
            byte[] pattern = new byte[] { 139, 21, 0, 0, 0, 0, 139, 77, 8, 139, 130, 0, 0, 0, 0, 137, 1, 139, 130, 0, 0, 0, 0, 137, 65, 4 };
            string mask = MaskFromPattern(pattern);
            int address = FindAddress(pattern, 11, mask, ExternalCounterstrike.EngineDll);
            int result = Memory.Read<int>(address);
            return result;
        }

        public static int GetEntityList()
        {
            int tmp1, tmp2;
            byte[] pattern = new byte[] { 0x05, 0x00, 0x00, 0x00, 0x00, 0xC1, 0xe9, 0x00, 0x39, 0x48, 0x04 };
            string mask = MaskFromPattern(pattern);
            int address = FindAddress(pattern, 0, mask, ExternalCounterstrike.ClientDll);
            tmp1 = Memory.Read<int>(address + 1);
            tmp2 = Memory.Read<byte>(address + 7);
            return tmp2 + tmp1;
        }

        public static int GetConvarPtr()
        {
            ProcessModule lib = Utils.GetModuleHandle(ExternalCounterstrike.Process, "vstdlib.dll");
            byte[] pattern = new byte[] { 232, 0, 0, 0, 0, 184, 0, 0, 0, 0 };
            string mask = MaskFromPattern(pattern);
            int address = FindAddress(pattern, 6, mask, lib);
            return Memory.Read<int>(address);
        }

        public static int GetWorldToViewMatrix()
        {
            byte[] pattern = new byte[] { 243, 15, 111, 5, 0, 0, 0, 0, 141, 133 };
            string mask = MaskFromPattern(pattern);
            int address = FindAddress(pattern, 4, mask, ExternalCounterstrike.ClientDll);
            return Memory.Read<int>(address) + 176;
        }

        public static int GetClientState()
        {
            byte[] pattern = new byte[]
            {
                0xC2, 0x00, 0x00,
                0xCC,
                0xCC,
                0x8B, 0x0D, 0x00, 0x00, 0x00, 0x00,
                0x33, 0xC0,
                0x83, 0xB9
                };
            string mask = MaskFromPattern(pattern);
            int address, val1;

            address = FindAddress(pattern, 7, mask, ExternalCounterstrike.EngineDll);
            val1 = Memory.Read<int>(address);
            return Memory.Read<int>(val1);
        }

        public static int GetLocalIndex()
        {
            byte[] pattern = new byte[]
            {
                0x8B, 0x80, 0x00, 0x00, 0x00, 0x00, 0x40, 0xC3
            };

            string mask = MaskFromPattern(pattern);
            var address = FindAddress(pattern, 2, mask, ExternalCounterstrike.EngineDll);
            return Memory.Read<int>(address);
        }

        public static int GetGameDir()
        {
            byte[] pattern = new byte[]
            {
                0x68, 0x00, 0x00, 0x00, 0x00, 0x8D, 0x85, 0x00, 0x00, 0x00, 0x00, 0x50, 0x68, 0x00, 0x00, 0x00, 0x00, 0x68
            };
            string mask = MaskFromPattern(pattern);
            var address = FindAddress(pattern, 1, mask, ExternalCounterstrike.EngineDll);
            return Memory.Read<int>(address);
        }

        public static int GetMapName()
        {
            byte[] pattern = new byte[]
            {
                0x05, 0x00, 0x00, 0x00, 0x00, 0xC3, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0x80, 0x3D
            };
            string mask = MaskFromPattern(pattern);
            var address = FindAddress(pattern, 1, mask, ExternalCounterstrike.EngineDll);
            return Memory.Read<int>(address);
        }

        public static int GetDormantOffset()
        {
            byte[] pattern = new byte[]
            {
                0x88, 0x9E, 0x00, 0x00, 0x00, 0x00, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x53, 0x8D, 0x8E
            };
            string mask = MaskFromPattern(pattern);
            var address = FindAddress(pattern, 2, mask, ExternalCounterstrike.ClientDll);
            return Memory.Read<int>(address);
        }

        public static int GetSignOnState()
        {
            //83 B9 ? ? ? ? 06 0F 94 C0 C3
            byte[] pattern = new byte[]
            {
                0x83, 0xB9, 0x00, 0x00, 0x00, 0x00, 0x06, 0x0F, 0x94, 0xC0, 0xC3
            };
            string mask = MaskFromPattern(pattern);
            var address = FindAddress(pattern, 2, mask, ExternalCounterstrike.EngineDll);
            return Memory.Read<int>(address);
        }

        public static int GetClientClassesHead()
        {
            byte[] pattern = Encoding.Default.GetBytes("DT_TEWorldDecal");
            string mask = MaskFromPattern(pattern);
            int address, result;

            address = FindAddress(pattern, 0, "xxxxxxxxxxxxxxx", ExternalCounterstrike.ClientDll);
            int address1 = FindAddress(BitConverter.GetBytes(address), 0x2B, "xxxx", ExternalCounterstrike.ClientDll);
            result = Memory.Read<int>(address1);
            return result;
        }

        private static string MaskFromPattern(byte[] pattern)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte data in pattern)
                if (data == 0x00)
                    builder.Append('?');
                else
                    builder.Append('x');
            return builder.ToString();
        }

        private static int FindAddress(byte[] pattern, int offset, string mask, ProcessModule module)
        {
            int address = 0;
            var baseAddress = module.BaseAddress.ToInt32();
            var moduleSize = module.ModuleMemorySize;
            for (int i = 0; i < moduleSize && address == 0; i += (int)(0xFFFF * 0.75))
            {
                ExternalCounterstrike.SigScanner.Address = new IntPtr(baseAddress + i);
                address = ExternalCounterstrike.SigScanner.FindPattern(pattern, mask, offset).ToInt32();
                ExternalCounterstrike.SigScanner.ResetRegion();
            }

            return address;
        }

    }
}
