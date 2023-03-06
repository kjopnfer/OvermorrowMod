using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories.BearTrap
{
    public class PlacedBearTrap : ModProjectile
    {
        public override bool ShouldUpdatePosition() => false;
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1200;
        }

        private bool CheckOnGround()
        {
            Tile leftTile = Framing.GetTileSafely(Projectile.Hitbox.BottomLeft());
            Tile rightTile = Framing.GetTileSafely(Projectile.Hitbox.BottomRight());
            if (leftTile.HasTile && Main.tileSolid[leftTile.TileType] && rightTile.HasTile && Main.tileSolid[rightTile.TileType])
            {
                return true;
            }

            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = 360;

            while (!CheckOnGround()) Projectile.Center += Vector2.UnitY;
        }

        public override void AI()
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            int frame = 0;
            Rectangle drawRectangle = new Rectangle(0, texture.Height / 4 * frame, texture.Width, texture.Height / 4);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, drawRectangle, Color.White, Projectile.rotation, drawRectangle.Size() / 2f, 1f, SpriteEffects.None, 1);

            return false;
        }
    }
}