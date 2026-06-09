using OvermorrowMod.Core.WorldGeneration.TestSubworld;
using System.Collections.Generic;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Content.Dungeons.Inkwell
{
    public class InkwellSubworld : TestSubworld
    {
        protected override string TitleCardText => "The Inkwell";

        public override List<GenPass> Tasks =>
        [
            new TestGenPass("Loading", 1, null, null, () => new InkwellContent())
        ];
    }
}
