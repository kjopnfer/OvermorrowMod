using OvermorrowMod.Common.DrawLayers;
using OvermorrowMod.Content.Buffs.Summon;
using OvermorrowMod.Content.Items.Weapons.Summoner.IorichWand;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public partial class OvermorrowModPlayer : ModPlayer
    {
        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (Player.HeldItem.type != ModContent.ItemType<IorichWand>() || !Player.HasBuff<IorichGuardian>())
            {
                PlayerDrawLayerLoader.Layers.OfType<GuardianBarDrawLayer>().FirstOrDefault()?.Hide();
            }
            if (!Player.GetModPlayer<OvermorrowModPlayer>().iorichGuardianShield)
            {
                PlayerDrawLayerLoader.Layers.OfType<ShieldDrawLayer>().FirstOrDefault()?.Hide();
            }
        }
    }
}