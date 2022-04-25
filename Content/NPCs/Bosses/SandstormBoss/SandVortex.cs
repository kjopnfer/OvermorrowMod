using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    public class SandVortex : ModProjectile
    {
        public override bool? CanDamage() => true;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand Vortex");
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 720;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (Projectile.timeLeft <= 180)
            {
                Projectile.scale = MathHelper.Lerp(0, 1, MathHelper.Clamp(Projectile.timeLeft, 0, 180) / 180f);
            }
            else
            {
                Projectile.scale = MathHelper.Lerp(0, 1, MathHelper.Clamp(Projectile.ai[0]++, 0, 180) / 180f);
            }


            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && Projectile.Distance(player.Center) < 200 && Collision.CanHit(Projectile.Center, 1, 1, player.Center, 1, 1))
                {
                    float PullStrength = Projectile.ai[0] >= 180 ? .25f : .165f;
                    float Direction = (Projectile.Center - player.Center).ToRotation();
                    float HorizontalPull = (float)Math.Cos(Direction) * PullStrength;
                    float VerticalPull = (float)Math.Sin(Direction) * (2 * PullStrength);

                    player.velocity += new Vector2(HorizontalPull, VerticalPull);
                }
            }

            for (int _ = 0; _ < 10; _++)
            {
                Vector2 RandomPosition = Projectile.Center + new Vector2(Main.rand.Next(150, 250), 0).RotatedByRandom(MathHelper.TwoPi);
                Vector2 Direction = Vector2.Normalize(Projectile.Center - RandomPosition);

                int DustSpeed = Projectile.ai[0] > 180 ? 20 : 10;

                int dust = Dust.NewDust(RandomPosition, 2, 2, DustID.Sand, Direction.X * DustSpeed, Direction.Y * DustSpeed);
                Main.dust[dust].noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.rotation += 0.06f;

            Texture2D texture = ModContent.Request<Texture2D>("OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/SandVortex2").Value;

            float mult = (0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.1f);
            float scale = Projectile.scale * 1.55f * mult;
            Color color = new Color(212, 148, 88);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0);

            texture = ModContent.Request<Texture2D>("OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/VortexCenter").Value;
            scale = Projectile.scale * 1.25f * mult;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0);

            scale = Projectile.scale * 1.1f * mult;
            color = new Color(83, 41, 7);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0);


            return false;
        }
    }
}
