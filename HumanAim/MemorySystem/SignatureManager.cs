using System;
using System.Diagnostics;
using System.Text;

namespace HumanAim.MemorySystem
{
    internal class SignatureManager
    {

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
