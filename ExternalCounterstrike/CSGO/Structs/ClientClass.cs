using ExternalCounterstrike.CSGO.Enums;

namespace ExternalCounterstrike.CSGO.Structs
{
    internal struct ClientClass
    {
        private int unk01;
        private int unk02;
        private int pClassName;
        private int pRecvTable;
        private int pNextClass;
        private ClientClassId classID;

        public string GetClassName()
        {
            return ExternalCounterstrike.Memory.ReadString(pClassName);
        }

        public ClientClassId GetClassId()
        {
            return classID;
        }

        public int GetRecvTable()
        {
            return pRecvTable;
        }

        public int GetNextClass()
        {
            return pNextClass;
        }
    }
}
