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
            Projectile.timeLeft = 720;
            Projectile.localAI[0] = -30;
            Projectile.localAI[1] = -60;

            while (!CheckOnGround()) Projectile.Center += Vector2.UnitY;
        }

        public ref float AICounter => ref Projectile.ai[0];
        public override void AI()
        {
            AICounter++;

            if (AICounter > 38)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (npc.friendly || !npc.active || !npc.Hitbox.Intersects(Projectile.Hitbox)) continue;

                    Main.NewText("trap");                    
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (AICounter > 38)
            {
                Main.spriteBatch.Reload(BlendState.Additive);
                Texture2D glowTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_05").Value;
                Main.spriteBatch.Draw(glowTexture, Projectile.Center - Main.screenPosition, null, Color.Orange * 0.55f, 0f, glowTexture.Size() / 2, 0.35f, SpriteEffects.None, 1f);
                Main.spriteBatch.Reload(BlendState.AlphaBlend);
            }

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            int frame = (int)MathHelper.Lerp(3, 0, Utils.Clamp(AICounter - 30, 0, 8) / 8f);

            Rectangle drawRectangle = new Rectangle(0, texture.Height / 4 * frame, texture.Width, texture.Height / 4);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, drawRectangle, lightColor, Projectile.rotation, drawRectangle.Size() / 2f, 1f, SpriteEffects.None, 1);

            Main.spriteBatch.Reload(BlendState.Additive);

            if (AICounter > 38)
            {
                texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "PulseCircle").Value;

                if (Projectile.localAI[0]++ == 60f) Projectile.localAI[0] = 0;
                if (Projectile.localAI[1]++ == 60f) Projectile.localAI[1] = 0;

                for (int i = 0; i < 2; i++)
                {
                    float progress = ModUtils.EaseOutQuad(Utils.Clamp(Projectile.localAI[i], 0, 90f) / 90f);
                    Color color = Color.Lerp(Color.Orange, Color.Transparent, progress);
                    float scale = MathHelper.Lerp(0, 0.25f, progress);

                    // The original is kinda faded out this just makes it thicker
                    Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color * 0.75f, 0f, texture.Size() / 2, scale, SpriteEffects.None, 1f);
                }

                Main.spriteBatch.Reload(BlendState.AlphaBlend);
            }

            return false;
        }
    }
}