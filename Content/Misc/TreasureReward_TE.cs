using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Content.Misc
{
    /// <summary>
    /// A treasure room's reward spot, at the air tile above the floor. Spawns a <see cref="TreasureChest"/> while a player
    /// is near and the reward is unclaimed, and saves the rolled offers so reloading does not reroll them.
    /// </summary>
    public class TreasureReward_TE : ModTileEntity
    {
        public bool Claimed;
        public int[] Offers;

        private const int SpawnRangeTiles = 60;
        private const int DropHeightTiles = 3;

        private int chestIndex = -1;

        public override bool IsTileValidForEntity(int x, int y) => true;

        public override void SaveData(TagCompound tag)
        {
            tag["Claimed"] = Claimed;
            if (Offers != null) tag["Offers"] = Offers;
        }

        public override void LoadData(TagCompound tag)
        {
            Claimed = tag.GetBool("Claimed");
            Offers = tag.ContainsKey("Offers") ? tag.GetIntArray("Offers") : null;
        }

        public override void Update()
        {
            if (Claimed) return;
            if (chestIndex >= 0 && Main.npc[chestIndex].active && Main.npc[chestIndex].ModNPC is TreasureChest chest && chest.RewardID == ID) return;
            chestIndex = -1;

            Vector2 spot = Position.ToWorldCoordinates(8, 16);
            if (!PlayerWithin(spot, SpawnRangeTiles * 16)) return;

            int index = NPC.NewNPC(new EntitySource_WorldEvent(), (int)spot.X, (int)spot.Y - DropHeightTiles * 16, ModContent.NPCType<TreasureChest>());
            if (index < Main.maxNPCs && Main.npc[index].ModNPC is TreasureChest spawned)
            {
                spawned.RewardID = ID;
                chestIndex = index;
            }
        }

        private static bool PlayerWithin(Vector2 point, float distance)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && Vector2.Distance(player.Center, point) <= distance) return true;
            }
            return false;
        }
    }
}
