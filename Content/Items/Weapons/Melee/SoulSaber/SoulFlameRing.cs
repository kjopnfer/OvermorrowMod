using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Buffs.Debuffs;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.SoulSaber
{
    public class SoulFlameRing : ModProjectile
    {
        public override string Texture => AssetDirectory.Textures + "FlameRing";

        public override bool? CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SoulFlame Ring");
        }

        public override void SetDefaults()
        {
            Projectile.height = 285;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 540;
            Projectile.alpha = 200;
        }

        public override void AI()
        {
            Projectile.rotation += 0.09f;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && Projectile.Distance(npc.Center) < 150)
                {
                    npc.AddBuff(ModContent.BuffType<SoulFlame>(), 60 * 10);
                }
            }

            Projectile.velocity.X *= 0.995f;
            Projectile.velocity.Y *= 0.995f;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "FlameRing").Value;
            int num154 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            int y2 = num154 * Projectile.frame;
            Rectangle drawRectangle = new Rectangle(0, y2, TextureAssets.Projectile[Projectile.type].Value.Width, num154);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            float scale = MathHelper.Lerp(0.95f, 1.05f, (float)Math.Sin(Projectile.ai[0]++ / 20f));
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, drawRectangle, Color.Lerp(Color.Transparent, Color.Blue, Utils.Clamp(Projectile.timeLeft, 0, 60) / 60f), Projectile.rotation, new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2), scale, SpriteEffects.None, 0);

            scale = MathHelper.Lerp(0.95f, 1.05f, (float)Math.Sin(Projectile.ai[0] / 20f));
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, drawRectangle, Color.Lerp(Color.Transparent, new Color(35, 200, 254), Utils.Clamp(Projectile.timeLeft, 0, 60) / 60f), Projectile.rotation * 1.1f, new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2), scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}