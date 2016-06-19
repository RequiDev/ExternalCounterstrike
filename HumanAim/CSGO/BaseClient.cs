using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanAim.CSGO
{
    internal class BaseClient
    {
        private static object lockObj = new object();
        private static List<BaseEntity> playerList;
        public static void Update()
        {
            lock(lockObj)
            {
                playerList = new List<BaseEntity>();
                for(int i = 0; i < 64/*change to maxplayers*/; i++)
                {
                    var entityAddress = HumanAim.ClientDll.BaseAddress.ToInt32() + 0x04A4BA64 + (i * 0x10);
                    var entity = HumanAim.Memory.Read<int>(entityAddress);

                    if (entity == 0) continue;
                    var player = new BaseEntity(entity);
                    player.Update();
                    playerList.Add(player);
                }
            }
        }

        public static void ClearCache()
        {
            playerList = new List<BaseEntity>();
        }

        public static List<BaseEntity> PlayerList
        {
            get
            {
                return playerList;
            }
        }

        public static BaseEntity LocalPlayer
        {
            get
            {
                return playerList.FirstOrDefault(player => player.GetIndex() == (EngineClient.LocalPlayerIndex + 1));
            }
        }
    }
}
