using Microsoft.Xna.Framework;
using OvermorrowMod.Common.RoomManager;
using OvermorrowMod.Content.NPCs.Archives;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Spawners
{
    public class HauntedChandelierSpawner : NPCSpawnPoint
    {
        public override int NPCType { get; set; } = ModContent.NPCType<HauntedChandelier>();
        protected override Vector2 SpawnOffset { get; set; } = new Vector2(0, 40);
    }
}