using ExternalCounterstrike.CSGO.Enum;
using System.Linq;
using System;
using ExternalCounterstrike.CSGO.Structs;
using System.Runtime.InteropServices;

namespace ExternalCounterstrike.CSGO
{
    internal class BasePlayer
    {
        private byte[] readData;
        private int address;

        public BasePlayer(int address)
        {
            this.address = address;
        }

        public void ClearCache()
        {
            readData = new byte[0];
        }

        public void Update()
        {
            readData = ExternalCounterstrike.Memory.ReadMemory(address, ExternalCounterstrike.NetVars.Values.Max() + 12); // Memory.ReadMemory(address, 0x3150);
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

        public int GetHealth()
        {
            return BitConverter.ToInt32(readData, ExternalCounterstrike.NetVars["m_iHealth"]);
        }

        public Team GetTeam()
        {
            return (Team)BitConverter.ToInt32(readData, ExternalCounterstrike.NetVars["m_iTeamNum"]);
        }

        public int GetIndex()
        {
            return BitConverter.ToInt32(readData, ExternalCounterstrike.NetVars["m_dwIndex"]);
        }
        public Vector3D GetPosition()
        {
            byte[] vecData = new byte[12];
            Buffer.BlockCopy(readData, ExternalCounterstrike.NetVars["m_vecOrigin"], vecData, 0, 12);
            return MemorySystem.MemoryScanner.GetStructure<Vector3D>(vecData);
        }

        public Vector3D GetPunchAngle()
        {
            byte[] vecData = new byte[12];
            Buffer.BlockCopy(readData, ExternalCounterstrike.NetVars["m_aimPunchAngle"], vecData, 0, 12);
            return MemorySystem.MemoryScanner.GetStructure<Vector3D>(vecData);
        }

        public Vector3D GetViewOffset()
        {
            byte[] vecData = new byte[12];
            Buffer.BlockCopy(readData, ExternalCounterstrike.NetVars["m_vecViewOffset[0]"], vecData, 0, 12);
            return MemorySystem.MemoryScanner.GetStructure<Vector3D>(vecData);
        }

        public Vector3D GetEyePos()
        {
            return GetPosition() + GetViewOffset();
        }

        public bool IsDormant()
        {
            return BitConverter.ToBoolean(readData, ExternalCounterstrike.NetVars["m_bDormant"]);
        }

        public int GetBoneMatrix()
        {
            return BitConverter.ToInt32(readData, ExternalCounterstrike.NetVars["m_dwBoneMatrix"]);
        }

        public Vector3D GetBonesPos(int boneId)
        {
            var size = Marshal.SizeOf(typeof(BaseBone));
            return ExternalCounterstrike.Memory.Read<BaseBone>(GetBoneMatrix() + (size * boneId)).ToVector3D();
        }
    }
}
