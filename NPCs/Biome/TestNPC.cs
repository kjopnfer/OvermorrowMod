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
        }

        int Random = Main.rand.Next(1, 1000);
        int timer = 0;
        int timer2 = 0;

        public override void SetDefaults()
        {
            npc.width = 46;
            npc.height = 76;
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
				int proj = Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 10, mod.ProjectileType("TestHook"), npc.damage, 0.0f, Main.myPlayer, 0.0f, (float)npc.whoAmI);
				npc.netUpdate = true;
            }

            /*if(timer > 15)
            {
                timer = 0;
            }*/


        }
    }
}