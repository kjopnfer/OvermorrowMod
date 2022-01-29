using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.SandstormBoss
{
    public class SandVortex : ModProjectile
    {
        public override bool CanDamage() => true;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand Vortex");
        }

        public override void SetDefaults()
        {
            projectile.width = 64;
            projectile.height = 64;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 720;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (projectile.timeLeft <= 180)
            {
                projectile.scale = MathHelper.Lerp(0, 1, MathHelper.Clamp(projectile.timeLeft, 0, 180) / 180f);
            }
            else
            {
                projectile.scale = MathHelper.Lerp(0, 1, MathHelper.Clamp(projectile.ai[0]++, 0, 180) / 180f);
            }


            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && projectile.Distance(player.Center) < 200)
                {
                    float PullStrength = projectile.ai[0] >= 180 ? .25f : .165f;
                    float Direction = (projectile.Center - player.Center).ToRotation();
                    float HorizontalPull = (float)Math.Cos(Direction) * PullStrength;
                    float VerticalPull = (float)Math.Sin(Direction) * (2 * PullStrength);

                    player.velocity += new Vector2(HorizontalPull, VerticalPull);
                }
            }

            for (int _ = 0; _ < 10; _++)
            {
                Vector2 RandomPosition = projectile.Center + new Vector2(Main.rand.Next(150, 250), 0).RotatedByRandom(MathHelper.TwoPi);
                Vector2 Direction = Vector2.Normalize(projectile.Center - RandomPosition);

                int DustSpeed = projectile.ai[0] > 180 ? 20 : 10;

                int dust = Dust.NewDust(RandomPosition, 2, 2, DustID.Sand, Direction.X * DustSpeed, Direction.Y * DustSpeed);
                Main.dust[dust].noGravity = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            projectile.rotation += 0.06f;

            Texture2D texture = ModContent.GetTexture("OvermorrowMod/NPCs/Bosses/SandstormBoss/SandVortex2");

            float mult = (0.55f + (float)Math.Sin(Main.GlobalTime) * 0.1f);
            float scale = projectile.scale * 1.55f * mult;
            Color color = new Color(212, 148, 88);

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, color, projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0f);

            texture = ModContent.GetTexture("OvermorrowMod/NPCs/Bosses/SandstormBoss/VortexCenter");
            scale = projectile.scale * 1.25f * mult;
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, color, projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0f);

            scale = projectile.scale * 1.1f * mult;
            color = new Color(83, 41, 7);
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, color, projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0f);


            return false;
        }
    }
}
