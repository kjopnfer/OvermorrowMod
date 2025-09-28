using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Archives.Accessories;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Particles;
using System.Diagnostics.Metrics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;

namespace OvermorrowMod.Core.Globals
{
    public class GlobalItems : GlobalItem
    {
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (item.ModItem is IBowModifier bowModifier)
            {
                player.GetModPlayer<BowPlayer>().AddBowModifier(bowModifier);
            }

            if (item.ModItem is IBowDrawEffects drawEffects)
            {
                player.GetModPlayer<BowPlayer>().AddBowDrawEffect(drawEffects);
            }

            if (item.ModItem is IGunModifier gunModifier)
            {
                player.GetModPlayer<GunPlayer>().AddGunModifier(gunModifier);
            }
        }
    }
}