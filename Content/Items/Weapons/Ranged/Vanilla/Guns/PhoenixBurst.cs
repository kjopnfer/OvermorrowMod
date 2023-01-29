using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla.Guns
{
    public class PhoenixBurst : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phoenix Burst");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
        }

        public ref float AICounter => ref Projectile.ai[0];
        public override void AI()
        {
            AICounter++;
            Projectile.scale = MathHelper.Lerp(0.5f, 0, Projectile.timeLeft / 120f);
            Projectile.rotation += 0.08f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "smoke_10").Value;
            float alpha = MathHelper.Lerp(1f, 0f, Utils.Clamp(AICounter, 0f, 80f) / 80f);
            float scale = MathHelper.Lerp(2f, 0f, Utils.Clamp(AICounter, 0f, 80f) / 80f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Red * alpha, Projectile.rotation, texture.Size() / 2f, scale, SpriteEffects.None, 1);

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "light_03").Value;
            scale = MathHelper.Lerp(2f, 0f, Utils.Clamp(AICounter, 0f, 80f) / 80f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.OrangeRed * alpha, Projectile.rotation, texture.Size() / 2f, scale + 0.5f, SpriteEffects.None, 1);


            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "twirl_01").Value;
            alpha = MathHelper.Lerp(1f, 0f, Utils.Clamp(AICounter, 0f, 80f) / 80f);
            scale = MathHelper.Lerp(0.75f, 0f, Utils.Clamp(AICounter, 0f, 60f) / 60f);

            for (int i = 0; i < 8; i++)
            {
                Color color = Color.Lerp(Color.Red, Color.Orange, i / 8f);
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color * alpha, (Projectile.rotation * 2 + MathHelper.ToRadians(10 * i)) * -1, texture.Size() / 2f, scale, SpriteEffects.FlipHorizontally, 1);
            }

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "twirl_01").Value;
            scale = MathHelper.Lerp(1.25f, 0f, Utils.Clamp(AICounter, 0f, 60f) / 60f);
            for (int i = 0; i < 8; i++)
            {
                Color color = Color.Lerp(Color.Red, Color.Orange, i / 8f);
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color * alpha, Projectile.rotation * 3 + MathHelper.ToRadians(25 * i) + MathHelper.ToRadians(240), texture.Size() / 2f, scale, SpriteEffects.FlipHorizontally, 1);
            }

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "twirl_01").Value;
            scale = MathHelper.Lerp(1.75f, 0f, Utils.Clamp(AICounter, 0f, 60f) / 60f);
            for (int i = 0; i < 8; i++)
            {
                Color color = Color.Lerp(Color.Red, Color.Orange, i / 8f);
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color * alpha, (Projectile.rotation * 4 + MathHelper.ToRadians(5 * i) + MathHelper.ToRadians(240)) * -1, texture.Size() / 2f, scale, SpriteEffects.None, 1);
            }

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            /*Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "light_03").Value;
            float alpha = MathHelper.Lerp(1f, 0f, AICounter / 80f);
            float scale = MathHelper.Lerp(0, 1.5f, AICounter / 80f);
            Color color = Color.Lerp(Color.DarkOrange, Color.DarkRed, AICounter / 80f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color * alpha, Projectile.rotation, texture.Size() / 2f, scale, SpriteEffects.None, 1);

            Main.spriteBatch.Reload(BlendState.Additive);

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "twirl_03").Value;
            alpha = MathHelper.SmoothStep(1f, 0f, AICounter / 80f);
            scale = MathHelper.Lerp(0, 1f, AICounter / 60f);

            for (int i = 1; i <= 4; i++)
            {
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation + (MathHelper.PiOver2 * i), texture.Size() / 2f, scale, SpriteEffects.None, 1);
            }

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            alpha = MathHelper.Lerp(1f, 0f, AICounter / 60f);
            scale = MathHelper.Lerp(0, 2f, AICounter / 100f);
            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "flame_01").Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.DarkOrange * alpha, Projectile.rotation, texture.Size() / 2f, scale, SpriteEffects.None, 1);*/

            return false;
        }
    }
}