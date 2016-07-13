using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalCounterstrike.CSGO.Models
{
    /*
    class ConVar
    {
    public:
	    char pad_0x0000[0x4]; //0x0000
	    ConVar* m_pNext; //0x0004 
	    __int32 m_bRegistered; //0x0008 change to bol
	    char* m_pszName; //0x000C 
	    char* m_pszDescription; //0x0010 
	    __int32 m_nFlags; //0x0014 
	    char pad_0x0018[0x4]; //0x0018
	    ConVar* m_pParent; //0x001C 
	    char* m_pszDefaultValue; //0x0020 
	    char* m_pszValue; //0x0024 
	    __int32 m_nSize; //0x0028 
	    float m_flValue; //0x002C 
	    __int32 m_nValue; //0x0030 
	    __int32 m_bHasMin; //0x0034  change to bool
	    float m_fMinVal; //0x0038 
	    __int32 m_bHasMax; //0x003C  change to bool
	    float m_fMaxVal; //0x0040 

    };//Size=0x0044
    */
    internal class ConVar
    {
        private static int _address;
        public ConVar(int ptr)
        {
            _address = ptr;
        }

        public int Pointer
        {
            get
            {
                return _address;
            }
        }

        public ConVar GetNext()
        {
            return new ConVar(ExternalCounterstrike.Memory.Read<int>(_address + 0x4));
        }

        public bool IsRegistered
        {
            get
            {
                return ExternalCounterstrike.Memory.Read<bool>(_address + 0x8);
            }
        }

        public string Name
        {
            get
            {
                return ExternalCounterstrike.Memory.ReadString(ExternalCounterstrike.Memory.Read<int>(_address + 0xC));
            }
        }

        public string Description
        {
            get
            {
                return ExternalCounterstrike.Memory.ReadString(ExternalCounterstrike.Memory.Read<int>(_address + 0x10));
            }
        }

        public int Flags
        {
            get
            {
                return ExternalCounterstrike.Memory.Read<int>(_address + 0x14);
            }
        }

        public ConVar GetParent()
        {
            return new ConVar(ExternalCounterstrike.Memory.Read<int>(_address + 0x1C));
        }
        public string DefaultValue
        {
            get
            {
                return ExternalCounterstrike.Memory.ReadString(ExternalCounterstrike.Memory.Read<int>(_address + 0x20));
            }
        }

        public string GetString()
        {
            return ExternalCounterstrike.Memory.ReadString(ExternalCounterstrike.Memory.Read<int>(_address + 0x24));
        }
        public int GetSize()
        {
            return ExternalCounterstrike.Memory.Read<int>(_address + 0x28);
        }
        public float GetFloat()
        {
            return ExternalCounterstrike.Memory.Read<float>(_address + 0x2C);
        }

        public int GetInt()
        {
            return ExternalCounterstrike.Memory.Read<int>(_address + 0x30);
        }

        public bool HasMin()
        {
            return ExternalCounterstrike.Memory.Read<bool>(_address + 0x34);
        }

        public float GetMinValue()
        {
            return ExternalCounterstrike.Memory.Read<float>(_address + 0x38);
        }
        public bool HasMax()
        {
            return ExternalCounterstrike.Memory.Read<bool>(_address + 0x3C);
        }

        public float GetMaxValue()
        {
            return ExternalCounterstrike.Memory.Read<float>(_address + 0x40);
        }

        public void SetValue(string val)
        {
            ExternalCounterstrike.Memory.WriteString(ExternalCounterstrike.Memory.Read<int>(_address + 0x24), val);
        }

        public void SetValue(float val)
        {
            ExternalCounterstrike.Memory.Write<float>(_address + 0x2C, val);
        }
        public void SetValue(int val)
        {
            ExternalCounterstrike.Memory.Write<float>(_address + 0x30, val);
        }
    }
}
