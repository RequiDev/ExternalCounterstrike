namespace ExternalCounterstrike.CSGO.BSP.Structs
{
    internal struct Header
    {
        public int ident;
        public int version;
        public Lump[] lumps;
        public int mapRevision;
    }
}
