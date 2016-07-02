using System;

namespace ExternalCounterstrike.CSGO.BSP.Enums
{
    [Flags]
    internal enum SurfFlag
    {
        SURF_LIGHT = 0x1,
        SURF_SKY2D = 0x2,
        SURF_SKY = 0x4,
        SURF_WARP = 0x8,
        SURF_TRANS = 0x10,
        SURF_NOPORTAL = 0x20,
        SURF_TRIGGER = 0x40,
        SURF_NODRAW = 0x80,
        SURF_HINT = 0x100,
        SURF_SKIP = 0x200,
        SURF_NOLIGHT = 0x400,
        SURF_BUMPLIGHT = 0x800,
        SURF_NOSHADOWS = 0x1000,
        SURF_NODECALS = 0x2000,
        SURF_NOCHOP = 0x4000,
        SURF_HITBOX = 0x8000
    }
}
