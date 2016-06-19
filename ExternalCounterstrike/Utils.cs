using System;
using System.Diagnostics;
using System.Linq;

namespace ExternalCounterstrike
{
    internal class Utils
    {
        public static bool IsModuleLoaded(Process p, string moduleName)
        {
            var q = from m in p.Modules.OfType<ProcessModule>()
                    select m;
            return q.Any(pm => pm.ModuleName == moduleName && (int)pm.BaseAddress != 0);
        }

        public static ProcessModule GetModuleHandle(Process p, string moduleName)
        {
            var q = from m in p.Modules.OfType<ProcessModule>()
                    select m;
            return q.FirstOrDefault(pm => pm.ModuleName == moduleName && pm.BaseAddress.ToInt32() != 0);
        }

        public static Int32 GetModule(Process p, string moduleName)
        {
            return (int)GetModuleHandle(p, moduleName).BaseAddress;
        }
        public static string RandomString(int length)
        {
            string charArray = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!\"§$%&/()=?`+#-.,<>|²³{[]}\\~´";
            char[] chars = new char[length];
            var random = new Random();
            random = new Random(random.Next(int.MinValue, int.MaxValue));
            for (int i = 0; i < length; i++)
                chars[i] = charArray[random.Next(0, charArray.Length)];
            return new string(chars);
        }
        public static string ByteSizeToString(long size)
        {
            string[] strArrays = new string[] { "B", "KB", "MB", "GB", "TB" };
            int num = 0;
            while (size > (long)1024)
            {
                size = size / (long)1024;
                num++;
            }
            string str = string.Format("{0} {1}", size.ToString(), strArrays[num]);
            return str;
        }
    }
}
