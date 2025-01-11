using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using OvermorrowMod.Common;
using System;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Particles;
using Terraria.GameContent;
using OvermorrowMod.Common.Utilities;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class FlowerSummon : ModProjectile
    {
        public override string Texture => AssetDirectory.ArchiveProjectiles + Name;
        public override void SetDefaults()
        {
            Projectile.timeLeft = 300;
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
        }

        int spriteVariant = 1;
        int projectileWidth = 42;
        int projectileHeight = 80;
        public override void OnSpawn(IEntitySource source)
        {
            spriteVariant = Main.rand.Next(0, 3);
        }

        public ref float AICounter => ref Projectile.ai[0];
        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;

            if (AICounter < 30) AICounter++;

            base.AI();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox)) return true;

            float _ = float.NaN;
            Vector2 endPosition = Projectile.Bottom + new Vector2(0, -projectileHeight).RotatedBy(Projectile.rotation);
            //Dust.NewDust(Projectile.Bottom, 1, 1, DustID.RedTorch);
            //Dust.NewDust(endPosition, 1, 1, DustID.Torch);

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, endPosition, projectileWidth * Projectile.scale, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            float textureWidth = texture.Width / 3f;
            Rectangle drawRectangle = new Rectangle((int)textureWidth * spriteVariant, 0, (int)textureWidth, texture.Height);

            float drawHeight = MathHelper.Lerp(0, 1f, EasingUtils.EaseOutBack(AICounter / 30f));
            float drawWidth = MathHelper.Lerp(0.5f, 1f, AICounter / 30f);
            Vector2 scale = new Vector2(drawWidth, drawHeight);
            Main.spriteBatch.Draw(texture, Projectile.Bottom - Main.screenPosition, drawRectangle, lightColor, Projectile.rotation, new Vector2(drawRectangle.Width / 2f, texture.Height), scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}