using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanAim.CSGO
{
    internal class BaseClient
    {
        private static List<BaseEntity> playerList;
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
                return playerList.FirstOrDefault(player => player.GetId() == EngineClient.LocalPlayerIndex);
            }
        }
    }
}
