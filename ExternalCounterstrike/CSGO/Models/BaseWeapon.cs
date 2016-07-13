using ExternalCounterstrike.CSGO.Enums;
using System;

namespace ExternalCounterstrike.CSGO.Models
{
    internal class BaseWeapon : BaseEntity
    {
        public BaseWeapon(int address) : base(address) { }

        public ItemDefinitionIndex GetItemDefinitionIndex()
        {
            return (ItemDefinitionIndex)BitConverter.ToInt32(readData, ExternalCounterstrike.NetVars["m_ItemDefIndex"]);
        }

        public int PaintKit
        {
            get
            {
                return BitConverter.ToInt32(readData, ExternalCounterstrike.NetVars["m_nPaintKit"]);
            }
            set
            {
                Memory.Write(address + ExternalCounterstrike.NetVars["m_nPaintKit"], value);
            }
        }

        public int ItemIDHigh
        {
            get
            {
                return BitConverter.ToInt32(readData, ExternalCounterstrike.NetVars["m_iItemIDHigh"]);
            }
            set
            {
                Memory.Write(address + ExternalCounterstrike.NetVars["m_iItemIDHigh"], value);
            }
        }

        public BasePlayer GetOwner()
        {
            int index = BitConverter.ToInt32(readData, ExternalCounterstrike.NetVars["m_hOwner"]);
            index &= 0xFFF;
            return EntityBase.GetEntityList().GetPlayerByIndex(index);
        }
    }
}
