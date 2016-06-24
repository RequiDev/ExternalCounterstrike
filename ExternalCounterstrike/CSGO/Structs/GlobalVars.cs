namespace ExternalCounterstrike.CSGO.Structs
{
    internal struct GlobalVars
    {
        public float realtime; // 0x00
        public int framecount; // 0x4
        public float absoluteframetime; // 0x8
        public float absoluteframestarttimestddev; // 0xC
        public float curtime; // 0x10
        public float frametime; // 0x14
        public int maxClients; // 0x18
        public int tickcount; // 0x1C
        public float interval_per_tick; // 0x20
        public float interpolation_amount; // 0x24
    }
}
