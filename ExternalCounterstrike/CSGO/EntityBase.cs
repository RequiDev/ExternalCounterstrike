using ExternalCounterstrike.CSGO.Models;
using ExternalCounterstrike.MemorySystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExternalCounterstrike.CSGO
{
    class EntityBase
    {
        private static MemoryScanner Memory => ExternalCounterstrike.Memory;
        private static readonly object lockObj = new object();
        private static EntityList currentList;
        private static int entityAddress;

        public static void ClearCache()
        {
            currentList = null;
        }

        public static void Update(bool withEnts = true)
        {
            if (entityAddress == 0)
                entityAddress = SignatureManager.GetEntityList();

            currentList = new EntityList();

            var players = new List<BasePlayer>();
            var entities = new List<BaseEntity>();
            var entityList = Memory.ReadMemory(entityAddress, 4096 * 0x10); //lol
            for (int i = 0; i < 64/*BaseClient.GlobalVars.maxClients*/; i++)
            {
                var entity = BitConverter.ToInt32(entityList, i * 0x10);

                if (entity == 0) continue;
                var player = new BasePlayer(entity);
                players.Add(player);
            }
            currentList.Players = players;
            if (withEnts)
            {
                for (int i = 64/*BaseClient.GlobalVars.maxClients*/; i < 4096; i++)
                {
                    var entity = BitConverter.ToInt32(entityList, i * 0x10);

                    if (entity == 0) continue;
                    var ent = new BaseEntity(entity);
                    entities.Add(ent);
                }
                currentList.Entities = entities;
            }
        }

        public static EntityList GetEntityList()
        {
            if (currentList == null)
                Update();
            return currentList;
        }
    }
}
