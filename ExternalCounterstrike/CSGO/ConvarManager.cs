using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalCounterstrike.CSGO
{
    internal class ConvarManager
    {
        private int m_pICVar = 0;
        private Dictionary<string, int> ConVars = new Dictionary<string, int>();

        public ConvarManager(int pCvar)
        {
            m_pICVar = pCvar;

            int hashMapEntry;
            if (m_pICVar != 0)
            {
                //bucket table
                var shortCuts = ExternalCounterstrike.Memory.Read<int>(m_pICVar + 52); //m_pCVarList
                hashMapEntry = ExternalCounterstrike.Memory.Read<int>(shortCuts); //ptr to list

                //walk list
                while (hashMapEntry != 0)
                {
                    var pConVar = ExternalCounterstrike.Memory.Read<int>(hashMapEntry + 4); //entry
                    var pConVarName = ExternalCounterstrike.Memory.ReadString(ExternalCounterstrike.Memory.Read<int>(pConVar + 12));

                    if (!ConVars.ContainsValue(pConVar))
                        ConVars.Add(pConVarName.ToLower(), pConVar);

                    hashMapEntry = ExternalCounterstrike.Memory.Read<int>(hashMapEntry + 4);
                }
            }
        }

        public ConVar FindFast(string str)
        {
            if (ConVars.ContainsKey(str.ToLower()))
                return new ConVar(ConVars[str.ToLower()]);

            return new ConVar(0);
        }

        public int Find(string str)
        {
            int hashMapEntry;

            if (m_pICVar != 0)
            {
                //bucket table
                var shortCuts = ExternalCounterstrike.Memory.Read<int>(m_pICVar + 52); //m_pCVarList
                hashMapEntry = ExternalCounterstrike.Memory.Read<int>(shortCuts); //ptr to list

                //walk list
                while (hashMapEntry != 0)
                {
                    var pConVar = ExternalCounterstrike.Memory.Read<int>(hashMapEntry + 4); //entry
                    var pConVarName = ExternalCounterstrike.Memory.ReadString(ExternalCounterstrike.Memory.Read<int>(pConVar + 12));
                    if (pConVarName.ToLower() == str.ToLower())
                    {
                        //found the nigger
                        return pConVar;
                    }
                    hashMapEntry = ExternalCounterstrike.Memory.Read<int>(hashMapEntry + 4);
                }
            }

            return 0;
        }
    }
}
