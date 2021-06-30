using System;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;


namespace OvermorrowMod.NPCs.Bosses.EvilBoss.Sins
{
    public class Gluttony : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 100;
            projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            projectile.rotation += 0.5f;
        }



        public override bool OnTileCollide(Vector2 oldVelocity)
        {
                Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
                Main.PlaySound(SoundID.Item10, projectile.position);
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
            return false;
        }
        


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sin of Gluttony");
        }
    }
}