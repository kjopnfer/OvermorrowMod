using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells
{
    public class SubworldDoorRoom : DoorRoom
    {
        protected override void PlaceDoorTile(int x, int y) => TileUtils.PlaceTileWithEntity<SubworldDoor, SubworldDoor_TE>(x, y);
    }
}
