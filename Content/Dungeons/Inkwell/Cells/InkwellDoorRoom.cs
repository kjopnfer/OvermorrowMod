using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Dungeons.Archive.Cells;
using OvermorrowMod.Content.Tiles.Archives;

namespace OvermorrowMod.Content.Dungeons.Inkwell.Cells
{
    public class InkwellDoorRoom : DoorRoom
    {
        private readonly string _target;

        public InkwellDoorRoom(string target) => _target = target;

        protected override void PlaceDoorTile(int x, int y)
        {
            var door = TileUtils.PlaceTileWithEntity<InkwellDoor, InkwellDoor_TE>(x, y);
            if (door != null) door.TargetSubworld = _target;
        }
    }
}
