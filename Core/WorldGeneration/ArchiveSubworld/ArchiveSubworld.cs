using SubworldLibrary;
using System.Collections.Generic;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Core.WorldGeneration.ArchiveSubworld
{
    public class ArchiveSubworld : Subworld
    {

        public override int Width => 644;

        public override int Height => 241;

        public static int GetWidth() => new ArchiveSubworld().Width;
        public static int GetHeight() => new ArchiveSubworld().Height;


        public override List<GenPass> Tasks => new()
        {
            new SetupGenPass("Loading", 1)
        };

        public override void OnEnter()
        {
            // Create a popup message or title card or something

            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
