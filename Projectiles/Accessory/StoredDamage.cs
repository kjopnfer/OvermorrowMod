using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Effects.Prim;
using OvermorrowMod.Effects.Prim.Trails;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Accessory
{
    class StoredDamage : ModProjectile, ITrailEntity
    {
        public Type TrailType()
        {
            return typeof(SoulTrail);
        }

        public override string Texture => "Terraria/Projectile_" + ProjectileID.LostSoulFriendly;
        public float distance;
        public float movement;
        public float delta;
        public float angle;
        public bool spawned = true;

        public override void SetDefaults() 
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            // projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.timeLeft = 2;
        }

        public override void AI()
        {
            if (spawned) {
                angle = Main.rand.Next(360);
                spawned = false;

                distance = 0;
                movement = 20;
                delta = 3;
            }
            Player player = Main.player[projectile.owner];

            // Vector2 anchor = player.Center + new Vector2(0, -100);
            Vector2 anchor = player.Center;

            if (distance < 125) {
                projectile.position = anchor + new Vector2(125 - distance, 0).RotatedBy(angle);
                distance += Math.Abs(movement);
                movement -= delta;

                Lighting.AddLight(projectile.position, 0.25f,0.4f,0.9f);

                projectile.timeLeft = 30;
                
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return base.PreDraw(spriteBatch, lightColor);
        }
    }
}