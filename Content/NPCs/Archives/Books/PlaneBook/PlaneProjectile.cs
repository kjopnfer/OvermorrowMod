using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs
{
    public class PlaneProjectile : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            Projectile.CloneDefaults(ProjectileID.PaperAirplaneA);
            Projectile.friendly = false;
            //Projectile.hostile = true;
            Projectile.Opacity = 0.75f;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Rectangle hitbox = new Rectangle((int)Projectile.Center.X + (14 * Projectile.direction), (int)Projectile.Center.Y, Projectile.width, Projectile.height);
            foreach (Player target in Main.player)
            {
                if (!target.active) continue;
                if (target.Hitbox.Intersects(hitbox))
                {
                    //target.Hurt(PlayerDeathReason.LegacyDefault(), Projectile.damage, 0, false, false);
                    //target.velocity = new Vector2(26 * NPC.direction, -4);
                    if (!target.noKnockback) target.velocity = Projectile.velocity * 1.3f;
                }
            }

            Lighting.AddLight(Projectile.Center, new Vector3(0.5f, 0.5f, 0.5f));
            base.AI();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[ProjectileID.PaperAirplaneA].Value;
            Color textureColor = Color.Gray;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 offset = new Vector2(-12, Projectile.direction == -1 ? -24 : 12);
                Vector2 drawPos = offset + Projectile.oldPos[k] + texture.Size() / 2f - Main.screenPosition;
                Color afterImageColor = (textureColor * 0.8f) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, drawPos, new Rectangle(0, 0, 6, 2), afterImageColor, Projectile.oldRot[k], texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
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