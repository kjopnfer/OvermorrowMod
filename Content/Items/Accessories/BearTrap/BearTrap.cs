using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Players;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Interfaces;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories.BearTrap
{
    public class BearTrap : ModItem, ITooltipObject
    {
        public List<TooltipObject> TooltipObjects()
        {
            return new List<TooltipObject>() {
                new ProjectileTooltip(ModContent.Request<Texture2D>(AssetDirectory.UI + "Tooltips/WhiteHat").Value,
                    "Bear Trap",
                    " + Deals damage to an enemy that walks over when armed\n + Locks any ground enemy that activated this trap in place",
                    1f,
                    ProjectileTooltipType.Trap),
            };
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Trapper's Armaments");
            /* Tooltip.SetDefault("Gain a <Projectile:Bear Trap> after dealing at least 70 Ranged damage in a single hit\n" +
                "Place [?] to place a <Projectile:Bear Trap>\n" +
                "You can have only up to 3 traps active at a time\n" +
                "[?] This item requires a keybind to use!"); */
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string hotkey = "?";

            foreach (string key in OvermorrowModFile.BearTrapKey.GetAssignedKeys())
            {
                hotkey = key;
            }

            foreach (TooltipLine line in tooltips)
            {
                if (line.Mod == "Terraria" && line.Name == "Tooltip1")
                {
                    line.Text = "Press <Key:" + hotkey + "> to place a <Projectile:Bear Trap>";
                }

                if (line.Mod == "Terraria" && line.Name == "Tooltip3")
                {
                    if (hotkey == "?") line.Text = "<Key:?> This item requires a keybind to use";
                    else line.Text = "";
                }
            }
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 32;
            Item.height = 28;
            Item.damage = 32;
            Item.knockBack = 0f;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().BearTrap = true;
            player.GetModPlayer<OvermorrowModPlayer>().BearTrapHide = hideVisual;
        }
    }
}