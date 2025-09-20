using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles
{
    public class DarknessBlock : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileSolidTop[Type] = false;


            AddMapEntry(Color.Black);
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.gamePaused) return;

            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameY == 0 && Main.rand.NextBool(20))
            {
                Texture2D texture = OvermorrowModFile.Instance.SpotlightTexture.Value;
                int widthRange = 16 + 8;
                int heightRange = 16 - 4;
                int stepSize = 2;
                int maxXSteps = widthRange / stepSize;
                int maxYSteps = heightRange / stepSize;

                if (Main.rand.NextBool())
                {
                    Vector2 offset = new Vector2(
                        Main.rand.Next(-maxXSteps, maxXSteps + 1) * stepSize,
                        Main.rand.Next(-maxYSteps, 1) * stepSize
                    );

                    var aura = new Spark(texture, ModUtils.SecondsToTicks(Main.rand.NextFloat(1f, 4f)), false)
                    {
                        endColor = Color.Black,
                        slowModifier = 0.98f,
                        squashHeight = false,
                    };

                    float scale = Main.rand.NextFloat(0.1f, 0.25f);
                    Vector2 velocity = -Vector2.UnitY * Main.rand.Next(3, 8);

                    ParticleManager.CreateParticleDirect(aura, new Vector2(i * 16, j * 16 + 32) + offset, velocity, Color.Black, 1f, scale, 0f, ParticleDrawLayer.BehindProjectiles, useAdditiveBlending: false);
                }
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;

            //spriteBatch.Draw(TextureAssets.MagicPixel.Value, drawPos, new Rectangle(0, 0, 16, 16), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            return false;
        }
    }
}