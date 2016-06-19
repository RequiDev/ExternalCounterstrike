using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanAim.CSGO.Structs
{
    internal struct BaseBone
    {
        public float M11;
        public float M12;
        public float M13;
        //private fixed byte skip1 [12];
        public float X;

        public float M21;
        public float M22;
        public float M23;
        //private fixed byte skip2[12];
        public float Y;

        public float M31;
        public float M32;
        public float M33;
        //private fixed byte skip3[12];
        public float Z;

        public Vector3D ToVector3D()
        {
            return new Vector3D { X = X, Y = Y, Z = Z };
        }
    }
}
