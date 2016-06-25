using System.Runtime.InteropServices;

namespace ExternalCounterstrike.Overlay.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Margin
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }
}
