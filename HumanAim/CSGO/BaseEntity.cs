using HumanAim.CSGO.Enum;
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
            readData = null; // Memory.ReadMemory(address, 0x3150);
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
            return BitConverter.ToInt32(readData, 0xFC);
        }

        public Team GetTeam()
        {
            return (Team)BitConverter.ToInt32(readData, 0xF0);
        }
    }
}
