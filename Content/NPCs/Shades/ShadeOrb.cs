using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Shades
{
    public class ShadeOrb : ModNPC
    {
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CheckActive() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("???");
        }

        public override void SetDefaults()
        {
            npc.width = 32;
            npc.height = 32;
            npc.defense = 9;
            npc.lifeMax = 500;
            npc.aiStyle = -1;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath53;
            npc.value = 60f;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
        }

        public ref float AICounter => ref npc.ai[0];
        public ref float SpawnFlag => ref npc.ai[1];
        public override void AI()
        {
            SpawnFlag = -1;
            foreach (Player player in Main.player)
            {
                if (player.active && npc.Distance(player.Center) < 2000) SpawnFlag = 1;
            }

            if (SpawnFlag == 1)
            {
                if (++AICounter % 300 == 0)
                {
                    Vector2 SpawnPosition = npc.Center;
                    NPC.NewNPC((int)SpawnPosition.X, (int)SpawnPosition.Y, ModContent.NPCType<Shade>());
                }
            }
        }
    }
}