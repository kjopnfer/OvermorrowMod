using OvermorrowMod.Common;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid;

namespace OvermorrowMod.Content.Dungeons.Archive.Cells
{
    /// <summary>
    /// The room the player spawns in.
    /// </summary>
    public class StartingRoom : DoorRoom
    {
        public override RoomType Type => RoomType.Start;

        public override bool IsSpawnRoom => true;

        protected override string AsepritePath => AssetDirectory.GrandArchives + "StartingRoom.aseprite";

        public override void PlaceFurniture(FurnitureContext ctx)
        {
            int paintX = ctx.Origin.X - DungeonGrid.HorizontalPadding;
            int paintY = ctx.Origin.Y - DungeonGrid.VerticalPadding;
            TexGen.PaintAsepriteLayer(SheetLayer.Objects, AsepritePath, paintX, paintY, ctx.Palette.Objects);
        }
    }
}
