using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;

namespace OvermorrowMod.Content.Dungeons.Archive.Cells
{
    public class SubworldDoorRoom : DoorRoom
    {
        private readonly string _target;

        public SubworldDoorRoom(string target) => _target = target;

        protected override void PlaceDoorTile(int x, int y)
        {
            var door = TileUtils.PlaceTileWithEntity<SubworldDoor, SubworldDoor_TE>(x, y);
            if (door != null) door.TargetSubworld = _target;
        }
    }
}
