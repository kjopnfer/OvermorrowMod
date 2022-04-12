using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Core;
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

        public override string Texture => "Terraria/Projectile_" + ProjectileID.LostSoulFriendly;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Searing Star");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 16;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.aiStyle = -1;
            projectile.timeLeft = 60 * 6;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
        }

        private ref float AICounter => ref projectile.ai[0];
        private ref float InitialRotation => ref projectile.ai[1];

        public override void AI()
        {
            projectile.velocity = projectile.velocity.RotatedBy(MathHelper.ToRadians(0.5f * projectile.ai[1]));

            if (Main.rand.NextBool(10))
            {
                int dust = Dust.NewDust(new Vector2(projectile.position.X + projectile.velocity.X, projectile.position.Y + projectile.velocity.Y), projectile.width, projectile.height, DustID.RainbowMk2, projectile.velocity.X, projectile.velocity.Y, 100, new Color(253, 254, 255), 0.5f);
                Main.dust[dust].noGravity = true;
            }

            projectile.rotation = projectile.velocity.ToRotation();
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture(AssetDirectory.Textures + "Spotlight");

            Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), new Color(244, 188, 91), projectile.rotation, drawOrigin, projectile.scale * 0.4f, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), new Color(244, 188, 91), projectile.rotation, drawOrigin, projectile.scale * 0.4f, SpriteEffects.None, 0);

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), new Color(244, 188, 91), projectile.rotation + MathHelper.PiOver2, drawOrigin, new Vector2(0.15f, 1f), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), new Color(244, 188, 91), projectile.rotation, drawOrigin, new Vector2(0.15f, 1f), SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

}