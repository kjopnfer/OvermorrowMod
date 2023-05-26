using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Players;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Interfaces;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories
{
    public class SickeningSnack : ModItem, ITooltipObject
    {
        public List<TooltipObject> TooltipObjects()
        {
            return new List<TooltipObject>() {
                new BuffTooltip(ModContent.Request<Texture2D>(AssetDirectory.UI + "Tooltips/WhiteHat").Value,
                    "Fungal Infection",
                    " - Reduces armor by [c/ff5555:2] for each debuff on the target\n - Spawns mushroom spores while active",
                    3f,
                    BuffTooltipType.Debuff),
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sickening Snack");
            Tooltip.SetDefault("Ranged attacks have a {Increase:33%} chance to inflict <Debuff:Fungal Infection>\n" +
                "Increase debuff durations on the target by {Increase:2} seconds on Ranged hits.\n" +
                "If the target has less than 50% health, increase debuff durations by {Increase:4} seconds instead.\n" +
                "Debuff durations can only be increased up to a maximum of 16 seconds.");
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 30;
            Item.height = 30;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().SickeningSnack = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GlowingMushroom, 20)
                .AddIngredient<StaleBread>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}