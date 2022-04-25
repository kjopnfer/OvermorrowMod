using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Projectiles.Accessory
{
    class StoredDamage : ModProjectile, ITrailEntity
    {
        public Color TrailColor(float progress) => Color.White;
        public float TrailSize(float progress) => 3;
        public Type TrailType()
        {
            return typeof(SoulTrail);
        }

        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.LostSoulFriendly;
        public float distance;
        public float movement;
        public float delta;
        public float angle;
        public bool spawned = true;

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            // projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2;
        }

        public override void AI()
        {
            if (spawned)
            {
                angle = Main.rand.Next(360);
                spawned = false;

                distance = 0;
                movement = 20;
                delta = 3;
            }
            Player player = Main.player[Projectile.owner];

            // Vector2 anchor = player.Center + new Vector2(0, -100);
            Vector2 anchor = player.Center;

            if (distance < 125)
            {
                Projectile.position = anchor + new Vector2(125 - distance, 0).RotatedBy(angle);
                distance += Math.Abs(movement);
                movement -= delta;

                Lighting.AddLight(Projectile.position, 0.25f, 0.4f, 0.9f);

                Projectile.timeLeft = 30;

            }
        }
    }
}