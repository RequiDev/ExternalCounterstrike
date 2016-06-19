using HumanAim.CSGO.Enum;
using System.Linq;
using System;
using HumanAim.CSGO.Structs;

namespace HumanAim.CSGO
{
    internal class BaseEntity
    {
        private byte[] readData;
        private int address;

        public BaseEntity(int address)
        {
            this.address = address;
        }

        public void ClearCache()
        {
            readData = new byte[0];
        }

        public void Update()
        {
            readData = HumanAim.Memory.ReadMemory(address, HumanAim.NetVars.Values.Max()); // Memory.ReadMemory(address, 0x3150);
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
            return BitConverter.ToInt32(readData, HumanAim.NetVars["m_iHealth"]);
        }

        public Team GetTeam()
        {
            return (Team)BitConverter.ToInt32(readData, HumanAim.NetVars["m_iTeamNum"]);
        }

        public int GetId()
        {
            return BitConverter.ToInt32(readData, HumanAim.NetVars["m_dwIndex"]);
        }

        public bool IsDormant()
        {
            return BitConverter.ToBoolean(readData, HumanAim.NetVars["m_bDormant"]);
        }

        public int GetBoneMatrix()
        {
            return BitConverter.ToInt32(readData, HumanAim.NetVars["m_dwBoneMatrix"]);
        }

        public Vector3D GetViewAngle()
        {
            return EngineClient.ViewAngle;
        }
    }
}
