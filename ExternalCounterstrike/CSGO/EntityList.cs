using ExternalCounterstrike.MemorySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalCounterstrike.CSGO
{
    internal class EntityList
    {
        private static MemoryScanner Memory => ExternalCounterstrike.Memory;
        private static readonly object lockObj = new object();
        private static List<BasePlayer> players;

        public static void Update()
        {
            lock (lockObj)
            {
                players = new List<BasePlayer>();
                for (int i = 0; i < 64/*BaseClient.GlobalVars.maxClients*/; i++)
                {
                    var entityAddress = ExternalCounterstrike.ClientDll.BaseAddress.ToInt32() + 0x04A4BA64/*sigscan this and maybe change reading method*/ + (i * 0x10);
                    var entity = Memory.Read<int>(entityAddress);

                    if (entity == 0) continue;
                    var player = new BasePlayer(entity);
                    player.Update();
                    players.Add(player);
                }
            }
        }

        public static List<BasePlayer> Players
        {
            get
            {
                return players;
            }
        }

        public static BasePlayer GetPlayerByIndex(int index)
        {
            return players.FirstOrDefault(player => player.GetIndex() == index);
        }

        public static BasePlayer GetLocalPlayer()
        {
            return players.FirstOrDefault(player => player.GetIndex() == (EngineClient.LocalPlayerIndex + 1));
        }
    }
}
