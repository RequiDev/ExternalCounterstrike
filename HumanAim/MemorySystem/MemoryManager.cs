using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HumanAim.MemorySystem
{
    internal class MemoryManager
    {
        private int _processId;
        private IntPtr _handle;
        public IntPtr ProcessHandle
        {
            get
            {
                if (_handle == IntPtr.Zero)
                {
                    _handle = OpenProcess(PROCESS_VM_WRITE | PROCESS_VM_OPERATION | PROCESS_WM_READ, false, _processId);
                }
                return _handle;
            }
        }
        public MemoryManager(Process process)
        {
            _processId = process.Id;
        }

        ~MemoryManager()
        {
            CloseHandle(ProcessHandle);
        }
        public T Read<T>(int address) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            var data = ReadMemory(address, size);
            return GetStructure<T>(data);
        }

        public byte[] ReadMemory(int address, int length)
        {
            var numArray = new byte[length];
            ReadProcessMemory(ProcessHandle, address, numArray, (uint)numArray.Length, 0);
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

        public void WriteMemory(int mAddress, byte[] pBytes)
        {
            WriteProcessMemory(ProcessHandle, (uint)mAddress, pBytes, (uint)pBytes.Length, 0);
        }

        private static T GetStructure<T>(byte[] address)
        {
            var handle = GCHandle.Alloc(address, GCHandleType.Pinned);
            var structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return structure;
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
        static extern Int32 ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, [In, Out] byte[] lpBuffer, UInt32 dwSize, int lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, UInt32 lpBaseAddress, byte[] lpBuffer, UInt32 dwSize, int lpNumberOfBytesWritten);
        #endregion
    }
}
