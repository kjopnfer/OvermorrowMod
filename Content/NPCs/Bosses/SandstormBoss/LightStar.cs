using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Interfaces;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    public class LightStar : ModProjectile, ITrailEntity
    {
        public Color TrailColor(float progress) => Color.Lerp(new Color(253, 254, 255), new Color(244, 188, 91), progress) * progress;
        public float TrailSize(float progress) => 24;
        public Type TrailType() => typeof(TorchTrail);

        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.LostSoulFriendly;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Searing Star");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 60 * 6;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        private ref float AICounter => ref Projectile.ai[0];
        private ref float InitialRotation => ref Projectile.ai[1];

        public override void AI()
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(0.5f * Projectile.ai[1]));

            if (Main.rand.NextBool(10))
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X + Projectile.velocity.X, Projectile.position.Y + Projectile.velocity.Y), Projectile.width, Projectile.height, DustID.RainbowMk2, Projectile.velocity.X, Projectile.velocity.Y, 100, new Color(253, 254, 255), 0.5f);
                Main.dust[dust].noGravity = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;

            Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), new Color(244, 188, 91), Projectile.rotation, drawOrigin, Projectile.scale * 0.4f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), new Color(244, 188, 91), Projectile.rotation, drawOrigin, Projectile.scale * 0.4f, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), new Color(244, 188, 91), Projectile.rotation + MathHelper.PiOver2, drawOrigin, new Vector2(0.15f, 1f), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), new Color(244, 188, 91), Projectile.rotation, drawOrigin, new Vector2(0.15f, 1f), SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

}