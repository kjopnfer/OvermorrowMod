using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Interfaces;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Bandits
{
    public class SlimeFlask : ModProjectile
    {
        public override bool? CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slime Flask");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.28f;
            if (Projectile.velocity.Y >= 5) Projectile.velocity.Y = 5;

            Projectile.rotation += 0.12f;
        }

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SlimeExplosion>(), 0, 0f, Main.myPlayer);
        }
    }

    public class SlimeExplosion : ModProjectile, ITileOverlay
    {
        private const int MAX_TIME = 300;
        private bool OnFire = false;
        public override string Texture => AssetDirectory.Textures + "Slime_" + Main.rand.Next(1, 4);
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SLIME EXPLOSION");
        }

        public override void SetDefaults()
        {
            Projectile.width = 224;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = MAX_TIME;
        }

        float counter = 0;
        public override void AI()
        {
            counter += (float)(Math.PI / 2f) / MAX_TIME;

            Projectile.scale = 1.5f;

            if (!OnFire)
            {
                foreach (Projectile projectile in Main.projectile)
                {
                    if (projectile.type != ModContent.ProjectileType<FlameArrow>() || !projectile.active) continue;

                    if (Projectile.Hitbox.Intersects(projectile.Hitbox))
                    {
                        OnFire = true;
                    }
                }
            }

            if (OnFire)
            {
                Lighting.AddLight(Projectile.Center, 2f, 0, 0);

                Vector2 RandomPosition = Projectile.Center + Vector2.UnitX * Main.rand.Next((int)(-Projectile.width / 2f), (int)(Projectile.width / 2f));
                if (Projectile.ai[0]++ % 2 == 0)
                    for (int i = 0; i < 2; i++)
                        Particle.CreateParticle(Particle.ParticleType<Flames>(), RandomPosition, -Vector2.UnitY * Main.rand.Next(6, 9), Color.Orange, 1f, Main.rand.NextFloat(0.45f, 0.55f), Main.rand.NextFloat(0, MathHelper.PiOver2), Main.rand.NextFloat(0.01f, 0.015f));

                int HEIGHT = 500;
                Rectangle hitRectangle = new Rectangle((int)Projectile.TopLeft.X, (int)Projectile.Top.Y - HEIGHT, Projectile.width, HEIGHT);

                foreach (Player player in Main.player)
                {
                    if (!player.active) continue;

                    if (player.Hitbox.Intersects(hitRectangle))
                    {
                        player.Hurt(PlayerDeathReason.LegacyDefault(), 25, 0, false, false, false, -1);
                        player.AddBuff(BuffID.OnFire, 180);
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;
        public void DrawOverTiles(SpriteBatch spriteBatch)
        {
            Color color = Color.White;
            color *= (float)Math.Cos(counter);
            spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        }
    }
}