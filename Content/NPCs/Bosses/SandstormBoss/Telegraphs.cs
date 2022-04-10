using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    public class PlatformTelegraph : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Telegraph");
        }

        public override void SetDefaults()
        {
            projectile.width = 113;
            projectile.height = 64;
            projectile.aiStyle = -1;
            projectile.timeLeft = 540;
        }

        public ref float AICounter => ref projectile.ai[0];
        public ref float Direction => ref projectile.ai[1];
        public override void AI()
        {
            if (AICounter++ % 10 == 0)
            {
                //Main.NewText("flip " + Direction);
                projectile.direction = (int)Direction * -1;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.Reload(BlendState.Additive);
            Texture2D texture = Main.projectileTexture[projectile.type];
            var spriteEffects = Direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Color flashColor = Color.Lerp(Color.Orange, Color.Yellow, (float)Math.Sin(AICounter / 10f)) * 0.75f;
            Color color = Color.Lerp(Color.Transparent, flashColor, Utils.Clamp(AICounter, 0, 60) / 60f);

            if (projectile.timeLeft < 60) color = Color.Lerp(Color.Transparent, flashColor, Utils.Clamp(projectile.timeLeft, 0, 60) / 60f);

            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, color, projectile.rotation, texture.Size() / 2, projectile.scale, spriteEffects, 0f);
            spriteBatch.Reload(BlendState.AlphaBlend);

            return false;
        }
    }
}