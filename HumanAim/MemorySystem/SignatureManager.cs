using System;
using System.Diagnostics;
using System.Text;

namespace HumanAim.MemorySystem
{
    internal class SignatureManager
    {
        public static int GetViewAngle()
        {
            byte[] pattern = new byte[] { 139, 21, 0, 0, 0, 0, 139, 77, 8, 139, 130, 0, 0, 0, 0, 137, 1, 139, 130, 0, 0, 0, 0, 137, 65, 4 };
            string mask = MaskFromPattern(pattern);
            int address = FindAddress(pattern, 11, mask, HumanAim.EngineDll);
            int result = HumanAim.Memory.Read<int>(address);
            return result;
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

            address = FindAddress(pattern, 7, mask, HumanAim.EngineDll);
            val1 = HumanAim.Memory.Read<int>(address);
            return HumanAim.Memory.Read<int>(val1);
        }

        public static int GetLocalIndex()
        {
            byte[] pattern = new byte[]
            {
                0x8B, 0x80, 0x00, 0x00, 0x00, 0x00, 0x40, 0xC3
            };

            string mask = MaskFromPattern(pattern);
            var address = FindAddress(pattern, 2, mask, HumanAim.EngineDll);
            return HumanAim.Memory.Read<int>(address);
        }

        public static int GetDormantOffset()
        {
            byte[] pattern = new byte[]
            {
                0x88, 0x9E, 0x00, 0x00, 0x00, 0x00, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x53, 0x8D, 0x8E, 0x00, 0x00, 0x00, 0x00, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x8B, 0x06, 0x8B, 0xCE, 0x53, 0xFF, 0x90, 0x00, 0x00, 0x00, 0x00, 0x8B, 0x46, 0x64, 0x0F, 0xB6, 0xCB, 0x5E, 0x5B, 0x66, 0x89, 0x0C, 0xC5, 0x00, 0x00, 0x00, 0x00, 0x5D, 0xC2, 0x04, 0x00
            };
            string mask = MaskFromPattern(pattern);
            var address = FindAddress(pattern, 2, mask, HumanAim.ClientDll);
            return HumanAim.Memory.Read<int>(address);
        }

        public static int GetSignOnState()
        {
            //83 B9 ? ? ? ? 06 0F 94 C0 C3
            byte[] pattern = new byte[]
            {
                0x83, 0xB9, 0x00, 0x00, 0x00, 0x00, 0x06, 0x0F, 0x94, 0xC0, 0xC3
            };
            string mask = MaskFromPattern(pattern);
            var address = FindAddress(pattern, 2, mask, HumanAim.EngineDll);
            return HumanAim.Memory.Read<int>(address);
        }

        public static int GetClientClassesHead()
        {
            byte[] pattern = Encoding.Default.GetBytes("DT_TEWorldDecal");
            string mask = MaskFromPattern(pattern);
            int address, result;

            address = FindAddress(pattern, 0, "xxxxxxxxxxxxxxx", HumanAim.ClientDll);
            int address1 = FindAddress(BitConverter.GetBytes(address), 0x2B, "xxxx", HumanAim.ClientDll);
            result = HumanAim.Memory.Read<int>(address1);
            result -= HumanAim.ClientDll.BaseAddress.ToInt32();
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
                HumanAim.SigScanner.Address = new IntPtr(baseAddress + i);
                address = HumanAim.SigScanner.FindPattern(pattern, mask, offset).ToInt32();
                HumanAim.SigScanner.ResetRegion();
            }

            return address;
        }
    }
}
