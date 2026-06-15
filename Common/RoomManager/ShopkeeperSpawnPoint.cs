using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.NPCs.Archives.Shop;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Common.RoomManager
{
    /// <summary>
    /// Spawns a single persistent NPC (the shopkeeper) once a player comes near, then stays put.
    /// Unlike <see cref="NPCSpawnPoint"/> it never despawns or respawns it.
    /// </summary>
    public class ShopkeeperSpawnPoint : ModTileEntity
    {
        public int NPCType { get; set; }

        /// <summary>Horizontal facing for the spawned shopkeeper (-1 left, 1 right).</summary>
        public int Facing { get; set; } = 1;

        private bool spawned;
        private readonly float SpawnDistance = ModUtils.TilesToPixels(120);

        public override bool IsTileValidForEntity(int x, int y) => true;

        public override void Update()
        {
            if (spawned || NPCType <= 0) return;

            Vector2 worldPos = Position.ToWorldCoordinates(8, 16);
            foreach (var player in Main.player)
            {
                if (!player.active || player.dead) continue;
                if (Vector2.Distance(worldPos, player.Center) > SpawnDistance) continue;

                NPC npc = NPC.NewNPCDirect(null, worldPos, NPCType);
                npc.Bottom = worldPos;
                if (npc.ModNPC is CartShopkeeper cart) cart.Facing = Facing;
                spawned = true;
                break;
            }
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["NPCType"] = NPCType;
            tag["Facing"] = Facing;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            if (tag.ContainsKey("NPCType")) NPCType = tag.GetInt("NPCType");
            if (tag.ContainsKey("Facing")) Facing = tag.GetInt("Facing");
        }
    }
}
