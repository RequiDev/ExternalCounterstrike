using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalCounterstrike.CSGO
{
    internal static class BaseClient
    {
        private static object lockObj = new object();
        private static List<BasePlayer> playerList;
        public static void Update()
        {
            lock(lockObj)
            {
                playerList = new List<BasePlayer>();
                for(int i = 0; i < 64/*change to maxplayers later*/; i++)
                {
                    var entityAddress = ExternalCounterstrike.ClientDll.BaseAddress.ToInt32() + 0x04A4BA64/*sigscan this and maybe change reading method*/ + (i * 0x10);
                    var entity = ExternalCounterstrike.Memory.Read<int>(entityAddress);

                    if (entity == 0) continue;
                    var player = new BasePlayer(entity);
                    player.Update();
                    playerList.Add(player);
                }
            }
        }

        public static void ClearCache()
        {
            playerList = new List<BasePlayer>();
        }

        public static List<BasePlayer> PlayerList
        {
            get
            {
                return playerList;
            }
        }

        public static BasePlayer LocalPlayer
        {
            get
            {
                return playerList.FirstOrDefault(player => player.GetIndex() == (EngineClient.LocalPlayerIndex + 1));
            }
        }
    }
}
