using Microsoft.Xna.Framework.Graphics;
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
        /*public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            r = 1f;
            g = 1f;
            b = 1f;
            a = 1f;
            fullBright = true;
            base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        }*/
    }
}