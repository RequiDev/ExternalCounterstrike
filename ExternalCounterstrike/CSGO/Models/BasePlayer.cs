using ExternalCounterstrike.CSGO.Enums;
using System.Linq;
using System;
using ExternalCounterstrike.CSGO.Structs;
using System.Runtime.InteropServices;
using ExternalCounterstrike.MemorySystem;

namespace ExternalCounterstrike.CSGO.Models
{
    internal class BasePlayer : BaseEntity
    {
        private BaseBone[] cachedBones = null;

        public BasePlayer(int address) : base(address) { }

        public int GetHealth()
        {
            return BitConverter.ToInt32(readData, ExternalCounterstrike.NetVars["m_iHealth"]);
        }

        public Team GetTeam()
        {
            return (Team)BitConverter.ToInt32(readData, ExternalCounterstrike.NetVars["m_iTeamNum"]);
        }
        
        public Vector3D GetAimPunchAngle()
        {
            byte[] vecData = new byte[12];
            Buffer.BlockCopy(readData, ExternalCounterstrike.NetVars["m_vecAimPunch"], vecData, 0, 12);
            return MemoryScanner.GetStructure<Vector3D>(vecData);
        }
        public Vector3D GetViewPunchAngle()
        {
            byte[] vecData = new byte[12];
            Buffer.BlockCopy(readData, ExternalCounterstrike.NetVars["m_vecViewPunch"], vecData, 0, 12);
            return MemoryScanner.GetStructure<Vector3D>(vecData);
        }

        public Vector3D GetViewOffset()
        {
            byte[] vecData = new byte[12];
            Buffer.BlockCopy(readData, ExternalCounterstrike.NetVars["m_vecViewOffset"], vecData, 0, 12);
            return MemoryScanner.GetStructure<Vector3D>(vecData);
        }

        public Vector3D GetEyePos()
        {
            return GetPosition() + GetViewOffset();
        }

        public BaseWeapon GetWeapon()
        {
            var entIndex = BitConverter.ToInt32(readData, ExternalCounterstrike.NetVars["m_hActiveWeapon"]) & 0xFFF;
            var currentWeapon = EntityBase.GetEntityList().GetEntityByIndex(entIndex);
            return currentWeapon != null ? new BaseWeapon(currentWeapon.Address) : null;
        }

        public BaseBone[] GetBoneMatrix()
        {
            var boneMatrix = BitConverter.ToInt32(readData, ExternalCounterstrike.NetVars["m_dwBoneMatrix"]);
            if(cachedBones == null)
                cachedBones = Memory.ReadArray<BaseBone>(boneMatrix, 128);
            return cachedBones;
        }

        public Vector3D GetBonesPos(int boneId)
        {
            return GetBoneMatrix()[boneId];
        }

        public float FlashAlpha
        {
            get
            {
                return BitConverter.ToSingle(readData, ExternalCounterstrike.NetVars["m_flFlashAlpha"]);
            }
            set
            {
                Memory.Write(address + ExternalCounterstrike.NetVars["m_flFlashAlpha"], value);
            }
        }
    }
}