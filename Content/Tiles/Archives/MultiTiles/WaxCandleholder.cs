using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Particles;
using SteelSeries.GameSense;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class WaxCandleholder : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;

        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;

            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, TileObjectData.newTile.Height).ToArray();

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, TileObjectData.newTile.Height - 1);

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            AddMapEntry(new Color(159, 131, 65));
            TileObjectData.addTile(Type);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.9f;
            g = 0.675f;
            b = 0f;
        }

        public static void CreateEmberParticle(Vector2 position, Vector2 velocity, float scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.StardustTowerMark).Value;

            var emberParticle = new Circle(texture, 0f, useSineFade: true);
            emberParticle.endColor = Color.DarkRed;
            velocity = velocity.RotatedBy(Main.rand.NextFloat(MathHelper.ToRadians(-5), MathHelper.ToRadians(5)));
            //velocity = velocity.RotatedBy(MathHelper.ToRadians(-5));

            ParticleManager.CreateParticleDirect(emberParticle, position, velocity, Color.DarkOrange, 1f, scale, 0f, ParticleDrawLayer.BehindNPCs, useAdditiveBlending: true);
            ParticleManager.CreateParticleDirect(emberParticle, position, velocity, Color.White * 0.45f, 1f, scale, 0f, ParticleDrawLayer.BehindNPCs, useAdditiveBlending: true);
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX == 0 && tile.TileFrameY == 0 && !Main.gamePaused)
            {
                float scale = 0.15f;
                Vector2 velocity = -Vector2.UnitY * 0.55f;

                // Create particles at different positions
                CreateEmberParticle(new Vector2(i + 1, j + 0.5f) * 16, velocity, scale);
                CreateEmberParticle(new Vector2(i + 0.55f, j + 0.75f) * 16, velocity, scale);
                CreateEmberParticle(new Vector2(i + 1.5f, j + 0.5f) * 16, velocity, scale);
            }

            base.DrawEffects(i, j, spriteBatch, ref drawData);
        }
    }
}