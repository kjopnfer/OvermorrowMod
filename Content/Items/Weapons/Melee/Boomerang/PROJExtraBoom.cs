using System;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;
using Terraria.Audio;


namespace OvermorrowMod.Content.Items.Weapons.Melee.Boomerang
{
    public class PROJExtraBoom : ModProjectile
    {
        private int timer = 0;
        private bool ComingBack = false;
        private int flametimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("ExtraBoom-erang");

        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 32;
            Projectile.timeLeft = 100;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }
        public override void Kill(int timeLeft)
        {
            int test = Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileID.Grenade, Projectile.damage*2, 10f, 0);
            Main.projectile[test].timeLeft = 1;
        }

        public override void AI()
        {
            timer++;

            Projectile.rotation += .55f;

            if (Projectile.timeLeft < 65)
            {
                Projectile.timeLeft = 10;
                ComingBack = true;
            }

            if (Projectile.timeLeft > 98)
            {
                Projectile.tileCollide = false;
            }
            else if (!ComingBack)
            {
                Projectile.tileCollide = true;
            }

            if (ComingBack)
            {
                flametimer++;
                float BetweenKill = Vector2.Distance(Main.player[Projectile.owner].Center, Projectile.Center);
                Projectile.tileCollide = false;
                Vector2 position = Projectile.Center;
                Vector2 targetPosition = Main.player[Projectile.owner].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                Projectile.velocity = direction * 18;
                if (BetweenKill < 22)
                {
                    Projectile.Kill();
                }
            }
        }
    }
}
