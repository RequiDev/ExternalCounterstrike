using ExternalCounterstrike.CSGO.BSP.Enums;

namespace ExternalCounterstrike.CSGO.BSP.Structs
{
    internal struct Lump
    {
        public int offset, length, version, fourCC;
        public LumpType type;
    }
}
