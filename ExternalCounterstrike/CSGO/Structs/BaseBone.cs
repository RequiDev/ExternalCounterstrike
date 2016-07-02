namespace ExternalCounterstrike.CSGO.Structs
{
    internal struct BaseBone
    {
        private float pad01;
        private float pad02;
        private float pad03;
        //private fixed byte skip1 [12];
        public float X;

        private float pad04;
        private float pad05;
        private float pad06;
        //private fixed byte skip2[12];
        public float Y;

        private float pad07;
        private float pad08;
        private float pad09;
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
