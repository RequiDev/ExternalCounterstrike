namespace ExternalCounterstrike.MemorySystem
{
    internal static class NetvarManager
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
            int current = ExternalCounterstrike.Memory.Read<int>(ExternalCounterstrike.Memory.Read<int>(subTable + 0x28));
            while (true)
            {
                string entryName = ExternalCounterstrike.Memory.ReadString(ExternalCounterstrike.Memory.Read<int>(current));

                if (entryName == "")
                    break;

                if (entryName.Length < 1)
                    break;

                if (entryName.Length > 3)
                {
                    int offset = ExternalCounterstrike.Memory.Read<int>(current + 0x2C);
                    if (entryName.Equals(searchFor))
                        return offset;
                }

                int subSubTable = ExternalCounterstrike.Memory.Read<int>(current + 0x28);

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
                string entryName = ExternalCounterstrike.Memory.ReadString(ExternalCounterstrike.Memory.Read<int>(current));

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

                int subSubTable = ExternalCounterstrike.Memory.Read<int>(current + 0x28);

                if (subSubTable > 0)
                {
                    int a = SearchInSubSubTable(current, searchFor);
                    if (a > 0)
                        return a;
                }

                int offset = ExternalCounterstrike.Memory.Read<int>(current + 0x2C);
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

            string className = ExternalCounterstrike.Memory.ReadString(ExternalCounterstrike.Memory.Read<int>(baseClass));

            if (className == "baseclass")
            {
                return SearchInBaseClass(ExternalCounterstrike.Memory.Read<int>(ExternalCounterstrike.Memory.Read<int>(baseClass + 0x28)), searchFor);
            }

            return 0;
        }
        private static int SearchInCSLocalData(int csLocalData, string searchFor)
        {
            int a = SearchInSubtable(csLocalData + 0x28, searchFor);
            if (a > 0)
                return a;

            string className = ExternalCounterstrike.Memory.ReadString(ExternalCounterstrike.Memory.Read<int>(csLocalData));

            if (className == "cslocaldata")
            {
                return SearchInBaseClass(ExternalCounterstrike.Memory.Read<int>(ExternalCounterstrike.Memory.Read<int>(csLocalData + 0x28)), searchFor);
            }

            return 0;
        }
        private static int SearchInLocalData(int localData, string searchFor)
        {
            int a = SearchInSubtable(localData + 0x28, searchFor);

            if (a > 0)
                return a;

            string className = ExternalCounterstrike.Memory.ReadString(ExternalCounterstrike.Memory.Read<int>(localData));

            if (className == "localdata")
            {
                return SearchInBaseClass(ExternalCounterstrike.Memory.Read<int>(ExternalCounterstrike.Memory.Read<int>(localData + 0x28)), searchFor);
            }

            return 0;
        }
        private static int SearchInTableFor(int table, string searchFor)
        {
            int current = ExternalCounterstrike.Memory.Read<int>(ExternalCounterstrike.Memory.Read<int>(table + 0xC));
            while (true)
            {
                if (ExternalCounterstrike.Memory.Read<int>(current) < 1)
                    break;

                string entryName = ExternalCounterstrike.Memory.ReadString(ExternalCounterstrike.Memory.Read<int>(current));

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

                int offset = ExternalCounterstrike.Memory.Read<int>(current + 0x2C);
                if (entryName.Equals(searchFor))
                    return offset;
                current += 0x3C;

            }

            return 0;
        }
        private static int GetTable(string wantedTable)
        {
            int clientClass = ClientClassesHead;
            int current = clientClass;

            while (true)
            {
                string className = ExternalCounterstrike.Memory.ReadString((ExternalCounterstrike.Memory.Read<int>((current + 0x8))));
                string tableName = ExternalCounterstrike.Memory.ReadString(ExternalCounterstrike.Memory.Read<int>(ExternalCounterstrike.Memory.Read<int>(current + 0xC) + 0xC));

                if (className.Equals(wantedTable) || tableName.Equals(wantedTable))
                    return current;

                current = ExternalCounterstrike.Memory.Read<int>(current + 0x10);
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
