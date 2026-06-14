using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Projectiles.Archives;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Utilities;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class Fireplace : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;

        public override bool CanExplode(int i, int j) => false;

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool RightClick(int i, int j)
        {
            Point bl = TileUtils.GetCornerOfMultiTile(i, j, TileUtils.CornerType.BottomLeft);
            var te = FireplaceRest_TE.GetOrCreate(bl.X, bl.Y);
            if (te != null && te.TryRest(Main.LocalPlayer))
            {
                Player player = Main.LocalPlayer;
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<RestLight>(), 0, 0f, player.whoAmI);
                return true;
            }

            return base.RightClick(i, j);
        }

        public override void MouseOver(int i, int j)
        {
            Point bl = TileUtils.GetCornerOfMultiTile(i, j, TileUtils.CornerType.BottomLeft);
            if (FireplaceRest_TE.IsRested(bl.X, bl.Y)) return;

            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ItemID.LesserHealingPotion;
        }

        public override void SetStaticDefaults()
        {
            AnimationFrameHeight = 54;

            // Properties
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileWaterDeath[Type] = false;
            Main.tileLavaDeath[Type] = false;

            //TileID.Sets.Campfire[Type] = true;

            AdjTiles = new int[] { TileID.Campfire };

            TileObjectData.newTile.Width = 6;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, TileObjectData.newTile.Height).ToArray();

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, 2);

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            //TileObjectData.newTile.StyleLineSkip = 6; // This needs to be added to work for modded tiles.
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(254, 121, 2));

            ArchiveLights.LightTileTypes.Add(Type);
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            // HasCampfire is a gameplay effect, so we don't run the code if closer is true.
            if (closer)
            {
                return;
            }

            if (Main.tile[i, j].TileFrameY < 36)
            {
                //Main.SceneMetrics.HasCampfire = true;
            }
        }


        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 5)
            {
                frameCounter = 0;
                frame++;
                if (frame > 5)
                {
                    frame = 0;
                }
            }
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.gamePaused || !Main.instance.IsActive)
            {
                return;
            }

            /*if (!Lighting.UpdateEveryFrame || new FastRandom(Main.TileFrameSeed).WithModifier(i, j).Next(4) == 0)
            {
                Tile tile = Main.tile[i, j];
                // Only emit dust from the top tiles, and only if toggled on. This logic limits dust spawning under different conditions.
                if (tile.TileFrameY == 0 && Main.rand.NextBool(3) && ((Main.drawToScreen && Main.rand.NextBool(4)) || !Main.drawToScreen))
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(i * 16 + 2, j * 16 - 4), 4, 8, DustID.Smoke, 0f, 0f, 100);
                    if (tile.TileFrameX == 0)
                        dust.position.X += Main.rand.Next(8);

                    if (tile.TileFrameX == 36)
                        dust.position.X -= Main.rand.Next(8);

                    dust.alpha += Main.rand.Next(100);
                    dust.velocity *= 0.2f;
                    dust.velocity.Y -= 0.5f + Main.rand.Next(10) * 0.1f;
                    dust.fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;
                }
            }*/
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameY < 36)
            {
                float pulse = Main.rand.Next(28, 42) * 0.005f;
                pulse += (270 - Main.mouseTextColor) / 700f;

                float brightness = ArchiveLights.GetBrightness(i, j);
                r = (1.2f + pulse) * brightness; // Increase red for a more vibrant orange
                g = (0.55f + pulse) * brightness; // Slightly lower green for a deeper orange
                b = 0; // Keep blue at 0 for a pure orange hue
            }

        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            var tile = Main.tile[i, j];

            if (!TileDrawing.IsVisible(tile))
            {
                return;
            }



            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }

            int width = 16;
            int offsetY = 0;
            int height = 16;
            short frameX = tile.TileFrameX;
            short frameY = tile.TileFrameY;
            int addFrX = 0;
            int addFrY = 0;

            TileLoader.SetDrawPositions(i, j, ref width, ref offsetY, ref height, ref frameX, ref frameY); // calculates the draw offsets
            TileLoader.SetAnimationFrame(Type, i, j, ref addFrX, ref addFrY); // calculates the animation offsets

            Rectangle drawRectangle = new Rectangle(tile.TileFrameX, tile.TileFrameY + addFrY, 16, 16);

            // The flame is manually drawn separate from the tile texture so that it can be drawn at full brightness.
            Color flameColor = Color.White * ArchiveLights.GetBrightness(i, j);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Flame").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + offsetY) + zero, drawRectangle, flameColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                DrawRestPrompt(i, j, spriteBatch, zero);
            }
        }

        private static void DrawRestPrompt(int i, int j, SpriteBatch spriteBatch, Vector2 zero)
        {
            Point bl = TileUtils.GetCornerOfMultiTile(i, j, TileUtils.CornerType.BottomLeft);
            if (FireplaceRest_TE.IsRested(bl.X, bl.Y)) return;

            const float OuterRadius = 200f;
            const float InnerRadius = 120f;

            Vector2 fireplaceCenter = new Vector2(i * 16 + 48, j * 16 + 24);
            float dist = Vector2.Distance(Main.LocalPlayer.Center, fireplaceCenter);
            float proximity = MathHelper.Clamp((OuterRadius - dist) / (OuterRadius - InnerRadius), 0f, 1f);
            if (proximity <= 0f) return;

            Texture2D icon = TextureAssets.Item[ItemID.LesserHealingPotion].Value;
            float bob = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 4f;
            float pulse = 0.85f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.15f;
            float alpha = proximity * pulse;

            Vector2 drawPos = new Vector2(i * 16 + 48, j * 16 - 14 + bob);
            Vector2 screenPos = drawPos - Main.screenPosition + zero;
            spriteBatch.Draw(icon, screenPos, null, Color.White * alpha, 0f, icon.Size() / 2f, 1f, SpriteEffects.None, 0f);
        }
    }
}