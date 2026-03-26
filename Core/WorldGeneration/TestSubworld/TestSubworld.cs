using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Core.WorldGeneration.TestSubworld
{
    public class TestSubworld : Subworld
    {
        public override int Width => 1600;
        public override int Height => 800;

        public override List<GenPass> Tasks =>
        [
            new TestGenPass("Loading", 1)
        ];

        public override void OnLoad()
        {
        }

        public override void Update()
        {
            // Subworlds don't call TileEntity.Update() automatically
            foreach (KeyValuePair<int, TileEntity> pair in TileEntity.ByID)
            {
                pair.Value.Update();
            }
        }

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
