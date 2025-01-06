using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Particles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs
{
    public class ChairBolt : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 100;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            Projectile.friendly = false;
            //Projectile.hostile = true;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.5f, 0.5f, 0.5f));

            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(5));

            // Optionally, rotate the projectile to match its velocity direction
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[ProjectileID.PaperAirplaneA].Value;
            float particleScale = 0.1f;

            if (!Main.gamePaused)
            {
                int randomIterations = Main.rand.Next(5, 9);
                Vector2 drawOffset = new Vector2(-4, -4).RotatedBy(Projectile.rotation);

                for (int i = 0; i < randomIterations; i++)
                {
                }
            }

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                float trailProgress = ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Color textureColor = Color.Lerp(Color.DeepPink, Color.Yellow, k / (float)Projectile.oldPos.Length);
                int trailSize = (int)MathHelper.SmoothStep(2, 9, trailProgress);

                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition;
                Color afterImageColor = (textureColor * 0.8f) * trailProgress;
               
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, drawPos, new Rectangle(0, 0, trailSize, 13), afterImageColor, Projectile.oldRot[k], texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            }

            Color silhouetteColor = new Color(3, 252, 232);
            Vector2 silhoutteOffset = new Vector2(-4, -4).RotatedBy(Projectile.rotation);

            float mult = (0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.1f);
            float silhouetteScale = MathHelper.Lerp(1f, 1.5f, (float)Math.Sin(Math.Abs(Projectile.ai[0]) / 10f));
            //Main.spriteBatch.Draw(texture, Projectile.Center + silhoutteOffset - Main.screenPosition, null, silhouetteColor * 0.8f, Projectile.rotation, Vector2.Zero, silhouetteScale, SpriteEffects.None, 0);
            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, textureColor * Projectile.Opacity, Projectile.rotation, Vector2.Zero, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}