using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public class GuardianBarDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.Shield);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            throw new NotImplementedException();
        }
    }
}
