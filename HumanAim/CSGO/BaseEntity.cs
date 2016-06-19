using HumanAim.CSGO.Enum;
using System.Linq;
using System;

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
    }
}
