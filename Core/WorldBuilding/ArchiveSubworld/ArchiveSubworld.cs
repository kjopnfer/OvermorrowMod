using SubworldLibrary;
using System.Collections.Generic;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Core.WorldBuilding.ArchiveSubworld
{
    public class ArchiveSubworld : Subworld
    {
        public override int Width => 536;

        public override int Height => 241;

        public override List<GenPass> Tasks => new()
        {
            new SetupGenPass("Loading", 1)
        };

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
