using System;
using System.IO;
using System.Linq;

namespace ExternalCounterstrike.CSGO.BSP
{
    internal class UtilityReader
    {
        #region VARIABLES
        private static bool bigEndian = false;
        #endregion

        #region PROPERTIES
        public static bool BigEndian
        {
            get { return bigEndian; }
            set { bigEndian = value; }
        }
        #endregion

        #region METHODS
        static public byte ReadByte(Stream stream)
        {
            byte[] buffer = ReadBytes(stream, 1);
            return buffer[0];
        }

        static public short ReadShort(Stream stream)
        {
            byte[] buffer = ReadBytes(stream, 2);
            if (bigEndian) buffer.Reverse();
            return BitConverter.ToInt16(buffer, 0);
        }

        static public ushort ReadUShort(Stream stream)
        {
            byte[] buffer = ReadBytes(stream, 2);
            if (bigEndian) buffer.Reverse();
            return BitConverter.ToUInt16(buffer, 0);
        }

        static public int ReadInt(Stream stream)
        {
            byte[] buffer = ReadBytes(stream, 4);
            if (bigEndian) buffer.Reverse();
            return BitConverter.ToInt32(buffer, 0);
        }

        static public uint ReadUInt(Stream stream)
        {
            byte[] buffer = ReadBytes(stream, 4);
            if (bigEndian) buffer.Reverse();
            return BitConverter.ToUInt32(buffer, 0);
        }

        static public long ReadLong(Stream stream)
        {
            byte[] buffer = ReadBytes(stream, 8);
            if (bigEndian) buffer.Reverse();
            return BitConverter.ToInt64(buffer, 0);
        }

        static public float ReadFloat(Stream stream)
        {
            byte[] buffer = ReadBytes(stream, 4);
            if (bigEndian) buffer.Reverse();
            return BitConverter.ToSingle(buffer, 0);
        }

        public static byte[] ReadBytes(Stream stream, int count)
        {
            byte[] buffer = new byte[count];
            stream.Read(buffer, 0, count);
            return buffer;
        }
        #endregion
    }
}
