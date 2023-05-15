using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;
using Terraria.Audio;

namespace OvermorrowMod.Common.Base
{
    public abstract class GlowmaskHoldout : ModProjectile
    {
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        public virtual Vector2 PositionOffset => new Vector2(15, 0);

        public abstract int ParentItem { get; }

        public virtual void SafeSetDefaults() { }

        public sealed override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1;
            SafeSetDefaults();
        }
        public Player player => Main.player[Projectile.owner];
        public override void AI()
        {
            if (Main.myPlayer != player.whoAmI) return;
            if (player.HeldItem.type != ParentItem) Projectile.Kill();
        }

        public override string Texture => ItemLoader.GetItem(ParentItem).Texture;
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 positionOffset = PositionOffset.RotatedBy(Projectile.Center.DirectionTo(Main.MouseWorld).ToRotation());
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + positionOffset;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 1);
            texture = (Texture2D)ModContent.Request<Texture2D>(GlowTexture);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 1);
            return false;
        }
    }
}