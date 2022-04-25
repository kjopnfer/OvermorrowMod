using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.GameContent;
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
            Projectile.width = 113;
            Projectile.height = 64;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 540;
        }

        public ref float AICounter => ref Projectile.ai[0];
        public ref float Direction => ref Projectile.ai[1];
        public override void AI()
        {
            if (AICounter++ % 10 == 0)
            {
                //Main.NewText("flip " + Direction);
                Projectile.direction = (int)Direction * -1;
            }
        }

        public override bool PreDraw(ref Color drawColor)
        {
            Main.spriteBatch.Reload(BlendState.Additive);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var spriteEffects = Direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Color flashColor = Color.Lerp(Color.Orange, Color.Yellow, (float)Math.Sin(AICounter / 10f)) * 0.75f;
            Color color = Color.Lerp(Color.Transparent, flashColor, Utils.Clamp(AICounter, 0, 60) / 60f);

            if (Projectile.timeLeft < 60) color = Color.Lerp(Color.Transparent, flashColor, Utils.Clamp(Projectile.timeLeft, 0, 60) / 60f);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, texture.Size() / 2, Projectile.scale, spriteEffects, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            return false;
        }
    }
}