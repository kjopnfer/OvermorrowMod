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
            // DisplayName.SetDefault("???");
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.defense = 9;
            NPC.lifeMax = 500;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath53;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public ref float AICounter => ref NPC.ai[0];
        public ref float SpawnFlag => ref NPC.ai[1];
        public override void AI()
        {
            SpawnFlag = -1;
            foreach (Player player in Main.player)
            {
                if (player.active && NPC.Distance(player.Center) < 2000) SpawnFlag = 1;
            }

            if (SpawnFlag == 1)
            {
                if (++AICounter % 300 == 0)
                {
                    Vector2 SpawnPosition = NPC.Center;
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)SpawnPosition.X, (int)SpawnPosition.Y, ModContent.NPCType<Shade>());
                }
            }
        }
    }
}