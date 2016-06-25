using System.Runtime.InteropServices;

namespace ExternalCounterstrike.Overlay.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int X;
        public int Y;
    }
}
