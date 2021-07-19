using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Biome
{
    public class TestNPC : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Clamper");
            Main.npcFrameCount[npc.type] = 4;
        }

        int Random = Main.rand.Next(1, 1000);
        int timer = 0;
        int timer2 = 0;

        public override void SetDefaults()
        {
            npc.width = 34;
            npc.height = 53;
            npc.damage = 14;
            npc.defense = 6;
            npc.lifeMax = 120;
            npc.HitSound = SoundID.NPCHit50;
            npc.DeathSound = SoundID.NPCDeath53;
            npc.value = 60f;
            npc.knockBackResist = 0.5f;
            npc.noGravity = true;
        }

        public override void AI()
        {
            
            timer++;
            if(timer == 1)
            {
				int p = Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 10, mod.ProjectileType("TestHook"), npc.damage, 0.0f, Main.myPlayer, 0.0f, (float)npc.whoAmI);
				Main.projectile[p].hostile = true;
				npc.netUpdate = true;
            }

            /*if(timer > 15)
            {
                timer = 0;
            }*/


        }

        public override void FindFrame(int frameHeight)
        {
            npc.spriteDirection = -npc.direction;
            npc.frameCounter++;

            if (npc.frameCounter % 6 == 5f) // Ticks per frame
            {
                npc.frame.Y += frameHeight;
            }
            if (npc.frame.Y >= frameHeight * 4) // 4 is max # of frames
            {
                npc.frame.Y = 0; // Reset back to default
            }
        }
    }
}