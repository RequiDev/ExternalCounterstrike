namespace ExternalCounterstrike.CSGO.Structs
{
    internal struct BaseBone
    {
        private float pad01;
        private float unk02;
        private float unk03;
        //private fixed byte skip1 [12];
        public float X;

        private float unk04;
        private float unk05;
        private float unk06;
        //private fixed byte skip2[12];
        public float Y;

        private float unk07;
        private float unk08;
        private float unk09;
        //private fixed byte skip3[12];
        public float Z;

        public Vector3D ToVector3D()
        {
            return new Vector3D { X = X, Y = Y, Z = Z };
        }

        public static implicit operator Vector3D(BaseBone val)
        {
            return new Vector3D(val.X, val.Y, val.Z);
        }
    }
}
