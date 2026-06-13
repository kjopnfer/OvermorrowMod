using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Particles;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class WaxCandelabra : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;

        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;

            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, TileObjectData.newTile.Height).ToArray();

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, TileObjectData.newTile.Height - 1);

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            AddMapEntry(new Color(159, 131, 65));
            LocalizedText name = CreateMapEntryName();

            TileObjectData.addTile(Type);

            ArchiveLights.LightTileTypes.Add(Type);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float brightness = ArchiveLights.GetBrightness(i, j);
            r = 0.9f * brightness;
            g = 0.675f * brightness;
            b = 0f;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            float brightness = ArchiveLights.GetBrightness(i, j);
            if (tile.TileFrameX == 0 && tile.TileFrameY == 0 && !Main.gamePaused && brightness > 0.05f)
            {
                float scale = 0.1f * brightness;
                Vector2 velocity = -Vector2.UnitY * 0.4f;

                WaxCandleholder.CreateEmberParticle(new Vector2(i + 1, j + 0.5f) * 16, velocity, scale);
                WaxCandleholder.CreateEmberParticle(new Vector2(i + 0.55f, j + 0.75f) * 16, velocity, scale);
                WaxCandleholder.CreateEmberParticle(new Vector2(i + 1.5f, j + 0.5f) * 16, velocity, scale);
            }

            base.NearbyEffects(i, j, closer);
        }
    }
}