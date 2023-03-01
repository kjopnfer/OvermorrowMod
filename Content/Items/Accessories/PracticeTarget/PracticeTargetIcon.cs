using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Players;
using OvermorrowMod.Common.VanillaOverrides.Bow;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories.PracticeTarget
{
    public class PracticeTargetIcon : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
        }

        private Vector2 initialPosition;
        public ref float AICounter => ref Projectile.ai[0];
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (AICounter == 0) initialPosition = player.Center + new Vector2(-4, -32);
            
            AICounter++;

            Projectile.Center = Vector2.Lerp(player.Center + new Vector2(-4, -32), player.Center + new Vector2(-4, -32) - Vector2.UnitY * 32, (float)(Math.Sin(AICounter / 15f)) / 2 + 0.5f);

            //if (AICounter == 5) Projectile.velocity = Vector2.Zero;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Item[ModContent.ItemType<PracticeTarget>()].Value;

            Vector2 scale = new Vector2(MathHelper.Lerp(0.5f, 1f, Utils.Clamp(AICounter, 0, 10) / 10f), 1f);
            float alpha = MathHelper.Lerp(0, 0.75f, (float)(Math.Sin(AICounter / 15f)) / 2 + 0.5f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, texture.Size() / 2f, scale, SpriteEffects.None, 1);

            return base.PreDraw(ref lightColor);
        }
    }
}