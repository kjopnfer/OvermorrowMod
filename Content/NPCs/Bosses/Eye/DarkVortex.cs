using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    public class DarkVortex : ModProjectile
    {
        public override bool? CanDamage() => true;
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Vortex");
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

            Projectile.rotation += 0.06f;
        }

        public static void DrawAll(SpriteBatch spriteBatch)
        {
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<DarkVortex>() && projectile.active)
                {
                    Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/DarkVortex").Value;

                    float mult = (0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.1f);
                    float scale = projectile.scale * 1.55f * mult;
                    Color color = Color.White;

                    spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, color, projectile.rotation, texture.Size() / 2, scale, SpriteEffects.None, 0);

                    texture = ModContent.Request<Texture2D>("OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/VortexCenter").Value;
                    scale = projectile.scale * 1.55f * mult;
                    spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, color, projectile.rotation, texture.Size() / 2, scale, SpriteEffects.None, 0);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/VortexShadow").Value;

            float mult = (0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.1f);
            float scale = Projectile.scale * 1.55f * mult;
            Color color = Color.White;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, texture.Size() / 2, scale, SpriteEffects.None, 0);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            return false;
        }
    }
}
