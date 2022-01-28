using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
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
