using OvermorrowMod.Common.DrawLayers;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Players
{
    public partial class OvermorrowModPlayer : ModPlayer
    {
        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            /*if (!Player.GetModPlayer<OvermorrowModPlayer>().iorichGuardianShield)
            {
                PlayerDrawLayerLoader.Layers.OfType<ShieldDrawLayer>().FirstOrDefault()?.Hide();
            }*/
        }
    }
}