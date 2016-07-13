using ExternalCounterstrike.CSGO.Structs;
using ExternalCounterstrike.MemorySystem;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace ExternalCounterstrike.CSGO.Models
{
    internal class BaseEntity
    {
        protected static MemoryScanner Memory => ExternalCounterstrike.Memory;
        protected byte[] readData;
        protected int address;

        public BaseEntity(int address)
        {
            this.address = address;
            Update();
        }

        public void Update()
        {
            readData = Memory.ReadMemory(address, ExternalCounterstrike.NetVars.Values.Max() + Marshal.SizeOf(typeof(Vector3D)));
        }

        public int Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
            }
        }

        public int GetIndex()
        {
            return BitConverter.ToInt32(readData, ExternalCounterstrike.NetVars["m_dwIndex"]);
        }

        public bool IsDormant()
        {
            return BitConverter.ToBoolean(readData, ExternalCounterstrike.NetVars["m_bDormant"]);
        }

        public ClientClass GetClientClass()
        {
            var vt = BitConverter.ToInt32(readData, 8);
            var fn = Memory.Read<int>(vt + 8);
            var result = Memory.Read<int>(fn + 1);
            return Memory.Read<ClientClass>(result);
        }

        public Vector3D GetPosition()
        {
            byte[] vecData = new byte[12];
            Buffer.BlockCopy(readData, ExternalCounterstrike.NetVars["m_vecOrigin"], vecData, 0, 12);
            return MemoryScanner.GetStructure<Vector3D>(vecData);
        }
    }
}
