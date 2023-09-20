using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
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
        public override bool? CanCutTiles() => false;
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

        private enum AIState
        {
            Setup = 0,
            Active = 1,
            Triggered = 2
        }

        public ref float AICase => ref Projectile.ai[0];
        public ref float AICounter => ref Projectile.ai[1];

        private NPC trappedNPC;
        public override void AI()
        {
            AICounter++;

            switch (AICase)
            {
                case (int)AIState.Setup:
                    if (AICounter >= 90)
                    {
                        AICase = (int)AIState.Active;
                    }
                    break;
                case (int)AIState.Active:
                    foreach (NPC npc in Main.npc)
                    {
                        if (npc.friendly || !npc.active || !npc.Hitbox.Intersects(Projectile.Hitbox)) continue;

                        trappedNPC = npc;
                        trappedNPC.StrikeNPC(new NPC.HitInfo { Damage = 69 });

                        AICase = (int)AIState.Triggered;
                        Projectile.timeLeft = 180;

                        break;
                    }
                    break;
                case (int)AIState.Triggered:
                    if (trappedNPC.active && trappedNPC.aiStyle == 3)
                    {
                        trappedNPC.GetGlobalNPC<OvermorrowGlobalNPC>().BearTrapped = true;
                    }

                    break;
            }
        }

        public override void Kill(int timeLeft)
        {
            if (trappedNPC != null && trappedNPC.active)
            {
                trappedNPC.GetGlobalNPC<OvermorrowGlobalNPC>().BearTrapped = false;
            }

            base.Kill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (AICase == (int)AIState.Active)
            {
                Main.spriteBatch.Reload(BlendState.Additive);
                Texture2D glowTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_05").Value;
                Main.spriteBatch.Draw(glowTexture, Projectile.Center - Main.screenPosition, null, Color.Orange * 0.55f, 0f, glowTexture.Size() / 2, 0.35f, SpriteEffects.None, 1f);
                Main.spriteBatch.Reload(BlendState.AlphaBlend);
            }

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            int frame = (int)MathHelper.Lerp(3, 0, Utils.Clamp(AICounter - 30, 0, 60) / 60f);
            if (AICase == (int)AIState.Triggered) frame = 3;

            Rectangle drawRectangle = new Rectangle(0, texture.Height / 4 * frame, texture.Width, texture.Height / 4);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, drawRectangle, lightColor, Projectile.rotation, drawRectangle.Size() / 2f, 1f, SpriteEffects.None, 1);

            Main.spriteBatch.Reload(BlendState.Additive);

            if (AICase == (int)AIState.Active)
            {
                texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "PulseCircle").Value;

                if (Projectile.localAI[0]++ == 60f) Projectile.localAI[0] = 0;
                if (Projectile.localAI[1]++ == 60f) Projectile.localAI[1] = 0;

                for (int i = 0; i < 2; i++)
                {
                    float progress = ModUtils.EaseOutQuad(Utils.Clamp(Projectile.localAI[i], 0, 90f) / 90f);
                    Color color = Color.Lerp(Color.Orange, Color.Transparent, progress);
                    float scale = MathHelper.Lerp(0, 0.25f, progress);

                    Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color * 0.75f, 0f, texture.Size() / 2, scale, SpriteEffects.None, 1f);
                }

            }

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            return false;
        }
    }
}