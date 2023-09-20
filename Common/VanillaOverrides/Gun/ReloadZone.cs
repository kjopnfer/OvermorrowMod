using OvermorrowMod.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.VanillaOverrides.Gun
{
    public class ReloadZone
    {
        public int StartPercentage = 0;
        public int EndPercentage = 0;

        public bool HasClicked { get; set; } = false;
        
        public ReloadZone(int StartPercentage, int EndPercentage)
        {
            this.StartPercentage = StartPercentage;
            this.EndPercentage = EndPercentage;
        }
    }
}