using ExternalCounterstrike.CSGO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalCounterstrike.CSGO
{
    class EntityList
    {
        public List<BasePlayer> Players;
        public List<BaseEntity> Entities;

        public BasePlayer GetPlayerByIndex(int index)
        {
            return Players.FirstOrDefault(player => player.GetIndex() == index);
        }

        public BaseEntity GetEntityByIndex(int index)
        {
            return Entities.FirstOrDefault(ent => ent.GetIndex() == index);
        }

        public BasePlayer GetLocalPlayer()
        {
            return Players.FirstOrDefault(player => player.GetIndex() == (EngineClient.LocalPlayerIndex + 1));
        }
    }
}
