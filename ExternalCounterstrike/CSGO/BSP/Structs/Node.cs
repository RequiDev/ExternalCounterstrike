namespace ExternalCounterstrike.CSGO.BSP.Structs
{
    internal struct Node
    {
        public int planenum;	    // index into plane array
        public int[] children;	//[2] negative numbers are -(leafs + 1), not nodes
        public short[] mins;	    //[3] for frustum culling
        public short[] maxs;       //[3]
        public ushort firstface;	    // index into face array
        public ushort numfaces;	    // counting both sides
        public short area;		// If all leaves below this node are in the same area, then
        // this is the area index. If not, this is -1.
        public short paddding;	// pad to 32 bytes length
    };
}
