using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace OvermorrowMod.NPCs.Inferno
{
    public class PandorasBox : ModProjectile
    {
        int RandomNPC = Main.rand.Next(0, 5);

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 18;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.magic = true;
            projectile.tileCollide = true;
            projectile.penetrate = 5;
            projectile.timeLeft = 80;
            projectile.light = 0.5f;
            projectile.extraUpdates = 1;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pandora's Box");
        }
        public override void Kill(int timeLeft)
        {
            RandomNPC = Main.rand.Next(0, 5);
            if(RandomNPC == 0)
            {
            NPC.NewNPC((int)projectile.Center.X + 0, (int)projectile.Center.Y + 0, mod.NPCType("FlameThrower"));
            }
            if(RandomNPC == 1)
            {
            NPC.NewNPC((int)projectile.Center.X + 0, (int)projectile.Center.Y + 0, mod.NPCType("InfernoBomber"));
            }
            if(RandomNPC == 2)
            {
            NPC.NewNPC((int)projectile.Center.X + 0, (int)projectile.Center.Y + 0, mod.NPCType("InfernoMage"));
            }
            if(RandomNPC == 3)
            {
            NPC.NewNPC((int)projectile.Center.X + 0, (int)projectile.Center.Y + 0, mod.NPCType("InfernoSupporter"));
            }
            if(RandomNPC == 4)
            {
            NPC.NewNPC((int)projectile.Center.X + 0, (int)projectile.Center.Y + 0, mod.NPCType("InfernoWarrior"));
            }

            Vector2 value1 = new Vector2(0f, 0f);
            Projectile.NewProjectile(projectile.position.X, projectile.position.Y, value1.X, value1.Y, mod.ProjectileType("PandorasBoxOpen"), projectile.damage - 10, 1f, projectile.owner, 0f);

            Vector2 position = projectile.Center;
            Main.PlaySound(SoundID.Item71, (int)position.X, (int)position.Y);
            int radius = 3;     //this is the explosion radius, the highter is the value the bigger is the explosion

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {

                    if (Math.Sqrt(x * x + y * y) <= radius + 0.5)   //this make so the explosion radius is a circle
                    {
                        Dust.NewDust(position, 5, 5, 162, 0.0f, 0.0f, 120, new Color(), 0.5f);  //this is the dust that will spawn after the explosion
                    }
                }
            }
        }
    }
}
