using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanAim.CSGO.Structs
{
    internal struct Vector3D
    {
        public float X;

        public float Y;

        public float Z;

        public Vector3D(float x = 0.0f, float y = 0.0f, float z = 0.0f)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float Length
        {
            get
            {
                return (float)Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
            }
        }

        public static Vector3D Zero
        {
            get
            {
                return new Vector3D();
            }
        }

        public bool IsEmpty()
        {
            return (int)X == 0 && (int)Y == 0 && (int)Z == 0;
        }

        public static float Distance(Vector3D a, Vector3D b)
        {
            var vec3 = a - b;
            var single = (float)Math.Sqrt((double)(vec3.X * vec3.X + vec3.Y * vec3.Y + vec3.Z * vec3.Z));
            return single;
        }

        public float DistanceFrom(Vector3D vec)
        {
            return Distance(this, vec);
        }

        public static float Dot(Vector3D left, Vector3D right)
        {
            float single = left.X * right.X + left.Y * right.Y + left.Z * right.Z;
            return single;
        }

        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            var vec3 = new Vector3D
            {
                X = a.X + b.X,
                Y = a.Y + b.Y,
                Z = a.Z + b.Z
            };
            return vec3;
        }

        public static Vector3D operator *(Vector3D a, float b)
        {
            var vec3 = new Vector3D
            {
                X = a.X * b,
                Y = a.Y * b,
                Z = a.Z * b
            };
            return vec3;
        }

        public static Vector3D operator /(Vector3D a, float b)
        {
            float oofl = 1.0f / b;
            var vec3 = new Vector3D
            {
                X = a.X * oofl,
                Y = a.Y * oofl,
                Z = a.Z * oofl
            };
            return vec3;
        }

        public static Vector3D operator *(Vector3D a, Vector3D b)
        {
            var vec3 = new Vector3D
            {
                X = a.Y * b.Z - a.Z * b.Y,
                Y = a.Z * b.X - a.X * b.Z,
                Z = a.X * b.Y - a.Y * b.X
            };
            return vec3;
        }

        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            var vec3 = new Vector3D
            {
                X = a.X - b.X,
                Y = a.Y - b.Y,
                Z = a.Z - b.Z
            };
            return vec3;
        }

        public static bool operator ==(Vector3D a, Vector3D b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(Vector3D a, Vector3D b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
        }
    }
}