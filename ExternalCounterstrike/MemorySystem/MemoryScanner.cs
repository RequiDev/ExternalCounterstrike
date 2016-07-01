using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ExternalCounterstrike.MemorySystem
{
    internal class MemoryScanner
    {
        private static Process _process;
        public MemoryScanner(Process process)
        {
            _process = process;
        }

        public T Read<T>(int address) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            var data = ReadMemory(address, size);
            return GetStructure<T>(data);
        }

        public T[] ReadArray<T>(int address, int length) where T : struct
        {
            byte[] data;
            int size = Marshal.SizeOf(typeof(T));

            data = ReadMemory(address, size * length);
            T[] result = new T[length];
            for (int i = 0; i < length; i++)
                result[i] = GetStructure<T>(data, i * size);

            return result;
        }

        public byte[] ReadMemory(int address, int length)
        {
            var numArray = new byte[length];
            ReadProcessMemory(_process.Handle, address, numArray, numArray.Length, 0);
            return numArray;
        }

        public void Write<T>(int adress, T input) where T : struct
        {
            int size = Marshal.SizeOf(input);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(input, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            WriteMemory(adress, arr);
        }
        public void WriteString(int mAddress, string str)
        {
            WriteMemory(mAddress, System.Text.Encoding.Default.GetBytes(str));
        }
        public void WriteMemory(int mAddress, byte[] pBytes)
        {
            WriteProcessMemory(_process.Handle, (uint)mAddress, pBytes, (uint)pBytes.Length, 0);
        }

        public string ReadString(int address, bool unicode = false)
        {
            var encoding = unicode ? Encoding.UTF8 : Encoding.Default;
            var numArray = ReadMemory(address, 255);
            var str = encoding.GetString(numArray);

            if (str.Contains('\0'))
                str = str.Substring(0, str.IndexOf('\0'));
            return str;
        }

        public static T GetStructure<T>(byte[] bytes)
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return structure;
        }

        public static T GetStructure<T>(byte[] bytes, int index)
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] tmp = new byte[size];
            Array.Copy(bytes, index, tmp, 0, size);
            return GetStructure<T>(tmp);
        }

        #region Kernel32
        const int PROCESS_WM_READ = 0x0010;
        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_OPERATION = 0x0008;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        static extern int CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, int lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, UInt32 lpBaseAddress, byte[] lpBuffer, UInt32 dwSize, int lpNumberOfBytesWritten);
        #endregion
    }
}
