using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace OvermorrowMod.NPCs.SpiderBoss
{

    public class BloodEgg : ModNPC
    {

        int RandomEggVelo = Main.rand.Next(-3, 4);


        public override void SetDefaults()
        {
            npc.width = 18;
            npc.height = 18;
            npc.damage = 0;
            npc.aiStyle = 0;
            npc.dontTakeDamage = false;
            npc.defense = 0;
            npc.lifeMax = 50;
            npc.alpha = 20;
            npc.scale = 1.5f;
            npc.value = 25f;
            npc.noGravity = false;
            npc.lavaImmune = true;
            npc.buffImmune[BuffID.OnFire] = true;
        }

        int direction = 0;
        int pscale = 0;

        public override void AI()
        {
            RandomEggVelo = Main.rand.Next(-3, 4);
            npc.spriteDirection = 1;
            pscale++;
            if(pscale > 0 && pscale < 25)
            {
                npc.scale = npc.scale - 0.005f;
            }

            if(pscale > 25 && pscale < 50)
            {
                npc.scale = npc.scale + 0.005f;
            }

            if(pscale > 50 && pscale < 75)
            {
                npc.scale = npc.scale - 0.005f;
            }

            if(pscale > 75 && pscale < 100)
            {
                npc.scale = npc.scale + 0.005f;
            }

            if(pscale > 100 && pscale < 125)
            {
                npc.scale = npc.scale - 0.005f;
            }

            if(pscale > 125 && pscale < 150)
            {
                npc.scale = npc.scale + 0.005f;
            }

            if(pscale > 150 && pscale < 175)
            {
                npc.scale = npc.scale - 0.005f;
            }

            if(pscale > 175 && pscale < 200)
            {
                npc.scale = npc.scale + 0.005f;
            }

            if(pscale > 200 && pscale < 225)
            {
                npc.scale = npc.scale - 0.005f;
            }

            if(pscale > 225 && pscale < 250)
            {
                npc.scale = npc.scale + 0.005f;
            }

            if(pscale > 250 && pscale < 275)
            {
                npc.scale = npc.scale - 0.005f;
            }

            if(pscale > 275 && pscale < 300)
            {
                npc.scale = npc.scale + 0.005f;
            }

            direction++;
            if(direction == 300)
            {
                npc.life -= 1000;
                NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 20, 164);
                Vector2 value1 = new Vector2(0f, 0f);
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, value1.X - RandomEggVelo, value1.Y, mod.ProjectileType("Eggbottom"), npc.damage, 1f);
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, value1.X + RandomEggVelo, value1.Y - 3f, mod.ProjectileType("Eggtop"), npc.damage, 1f);
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spider Egg");
        }
    }
}

