namespace ExternalCounterstrike.CSGO.Structs
{
    internal struct BaseBone
    {
        public float pad01;
        public float pad02;
        public float pad03;
        //private fixed byte skip1 [12];
        public float X;

        public float pad04;
        public float pad05;
        public float pad06;
        //private fixed byte skip2[12];
        public float Y;

        public float pad07;
        public float pad08;
        public float pad09;
        //private fixed byte skip3[12];
        public float Z;

        public Vector3D ToVector3D()
        {
            return new Vector3D { X = X, Y = Y, Z = Z };
        }
    }
}
