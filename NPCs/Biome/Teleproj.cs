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
            projectile.timeLeft = 500;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acorn");
        }

		public override void AI() 
		{

            if(Vector2.Distance(projectile.Center, Main.player[projectile.owner].Center) < 50f)
            {
                projectile.Kill();
            }

                    NPC npc = Main.npc[(int)projectile.ai[0]];
                    
                        if(!hashit)
                        {
                            projectile.timeLeft = 500;
                        }
                        else
                        {
                            if (npc.CanBeChasedBy())
                            {
                                npc.Center = projectile.Center;
                                projectile.Kill();
                            }
                            else
                            {
                                NPC npc2 = Main.npc[(int)projectile.ai[0]];
                                npc2.Center = projectile.Center;
                                projectile.Kill();
                            }
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