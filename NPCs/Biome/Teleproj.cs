using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Biome
{
    public class Teleproj : ModProjectile
    {

        bool hashit = false;

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Teleport");
        }

		public override void AI() 
		{

            
            NPC npc = Main.npc[(int)projectile.ai[1]];

            if (hashit)
			{
                npc.Center = projectile.Center;
                projectile.Kill();
                Main.PlaySound(SoundID.Item, npc.position, 9);
            } 
                    
            

            float num116 = 16f;
            for (int num117 = 0; (float)num117 < 16; num117++)
            {
                Vector2 spinningpoint7 = Vector2.UnitX * 0f;
                spinningpoint7 += -Vector2.UnitY.RotatedBy((float)num117 * ((float)Math.PI * 2f / num116)) * new Vector2(1f, 4f);
                spinningpoint7 = spinningpoint7.RotatedBy(projectile.velocity.ToRotation());
                Vector2 position = projectile.Center;
                Dust dust = Terraria.Dust.NewDustPerfect(position, 10, new Vector2(0f, 0f), 0, new Color(), 1f);
                dust.noLight = true;
                dust.noGravity = true;
                dust.position = projectile.Center + spinningpoint7;
            }



            projectile.velocity.X *= 0.97f;
            projectile.velocity.Y += 0.3f;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);
        }
        
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            hashit = true;
            return false;
        }
    }
}