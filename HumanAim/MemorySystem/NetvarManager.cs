namespace HumanAim.MemorySystem
{
    internal class NetvarManager
    {
        private static int _clientClassesHead = 0;
        private static int ClientClassesHead
        {
            get
            {
                if(_clientClassesHead == 0)
                {
                    _clientClassesHead = SignatureManager.GetClientClassesHead();
                }
                return _clientClassesHead;
            }
        }
        private static int SearchInSubSubTable(int subTable, string searchFor)
        {
            int current = HumanAim.Memory.Read<int>(HumanAim.Memory.Read<int>(subTable + 0x28));
            while (true)
            {
                string entryName = HumanAim.Memory.ReadString(HumanAim.Memory.Read<int>(current));

                if (entryName == "")
                    break;

                if (entryName.Length < 1)
                    break;

                if (entryName.Length > 3)
                {
                    int offset = HumanAim.Memory.Read<int>(current + 0x2C);
                    if (entryName.Equals(searchFor))
                        return offset;
                }

                int subSubTable = HumanAim.Memory.Read<int>(current + 0x28);

                if (subSubTable > 0)
                {
                    int a = SearchInSubSubTable(current, searchFor);
                    if (a > 0)
                        return a;
                }
                current += 0x3C;
            }

            return 0;

        }
        private static int SearchInSubtable(int subTable, string searchFor)
        {
            int current = subTable;
            while (true)
            {
                string entryName = HumanAim.Memory.ReadString(HumanAim.Memory.Read<int>(current));

                if (entryName == "")
                    break;

                if (entryName.Length < 1)
                    break;

                if (entryName == "baseclass")
                {
                    int a = SearchInBaseClass(current, searchFor);
                    if (a > 0)
                        return a;
                }

                if (entryName == "cslocaldata")
                {
                    int a = SearchInCSLocalData(current, searchFor);
                    if (a > 0)
                        return a;
                }

                if (entryName == "localdata")
                {
                    int a = SearchInLocalData(current, searchFor);
                    if (a > 0)
                        return a;
                }

                int subSubTable = HumanAim.Memory.Read<int>(current + 0x28);

                if (subSubTable > 0)
                {
                    int a = SearchInSubSubTable(current, searchFor);
                    if (a > 0)
                        return a;
                }

                int offset = HumanAim.Memory.Read<int>(current + 0x2C);
                if (entryName == searchFor)
                    return offset;

                current += 0x3C;
            }

            return 0;
        }
        private static int SearchInBaseClass(int baseClass, string searchFor)
        {
            int a = SearchInSubtable(baseClass + 0x3C, searchFor);
            if (a > 0)
                return a;

            string className = HumanAim.Memory.ReadString(HumanAim.Memory.Read<int>(baseClass));

            if (className == "baseclass")
            {
                return SearchInBaseClass(HumanAim.Memory.Read<int>(HumanAim.Memory.Read<int>(baseClass + 0x28)), searchFor);
            }

            return 0;
        }
        private static int SearchInCSLocalData(int csLocalData, string searchFor)
        {
            int a = SearchInSubtable(csLocalData + 0x28, searchFor);
            if (a > 0)
                return a;

            string className = HumanAim.Memory.ReadString(HumanAim.Memory.Read<int>(csLocalData));

            if (className == "cslocaldata")
            {
                return SearchInBaseClass(HumanAim.Memory.Read<int>(HumanAim.Memory.Read<int>(csLocalData + 0x28)), searchFor);
            }

            return 0;
        }
        private static int SearchInLocalData(int localData, string searchFor)
        {
            int a = SearchInSubtable(localData + 0x28, searchFor);

            if (a > 0)
                return a;

            string className = HumanAim.Memory.ReadString(HumanAim.Memory.Read<int>(localData));

            if (className == "localdata")
            {
                return SearchInBaseClass(HumanAim.Memory.Read<int>(HumanAim.Memory.Read<int>(localData + 0x28)), searchFor);
            }

            return 0;
        }
        private static int SearchInTableFor(int table, string searchFor)
        {
            int current = HumanAim.Memory.Read<int>(HumanAim.Memory.Read<int>(table + 0xC));
            while (true)
            {
                if (HumanAim.Memory.Read<int>(current) < 1)
                    break;

                string entryName = HumanAim.Memory.ReadString(HumanAim.Memory.Read<int>(current));

                if (entryName.Length < 1)
                    break;

                if (entryName == "baseclass")
                {
                    return SearchInBaseClass(current, searchFor);
                }

                if (entryName == "cslocaldata")
                {
                    return SearchInCSLocalData(current, searchFor);
                }

                if (entryName == "localdata")
                {
                    return SearchInLocalData(current, searchFor);
                }

                int offset = HumanAim.Memory.Read<int>(current + 0x2C);
                if (entryName.Equals(searchFor))
                    return offset;
                current += 0x3C;

            }

            return 0;
        }
        private static int GetTable(string wantedTable)
        {
            int clientClass = (HumanAim.ClientDll.BaseAddress.ToInt32() + ClientClassesHead);
            int current = clientClass;

            while (true)
            {
                string className = HumanAim.Memory.ReadString((HumanAim.Memory.Read<int>((current + 0x8))));
                string tableName = HumanAim.Memory.ReadString(HumanAim.Memory.Read<int>(HumanAim.Memory.Read<int>(current + 0xC) + 0xC));

                if (className.Equals(wantedTable) || tableName.Equals(wantedTable))
                    return current;

                current = HumanAim.Memory.Read<int>(current + 0x10);
                if (current < 1)
                    break;
            }

            return 0;
        }

        public static int GetOffset(string table, string entry)
        {
            int tableAddress = GetTable(table);
            int offset = SearchInTableFor(tableAddress, entry);
            return offset;
        }
    }
}
