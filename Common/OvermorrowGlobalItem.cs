using OvermorrowMod.Content.Items.Consumable;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public class OvermorrowGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ModContent.ItemType<ReforgeStone>())
            {
                for (int lines = 0; lines < tooltips.Count; lines++)
                {
                    if (tooltips[lines].Name == "Damage") tooltips.RemoveAt(lines);
                    if (tooltips[lines].Name == "CritChance") tooltips.RemoveAt(lines);
                    if (tooltips[lines].Name == "Speed") tooltips.RemoveAt(lines);
                    if (tooltips[lines].Name == "Knockback") tooltips.RemoveAt(lines);
                }
            }

            base.ModifyTooltips(item, tooltips);
        }

        public override bool ConsumeItem(Item item, Player player)
        {
            if (Main.mouseItem.type == ModContent.ItemType<ReforgeStone>())
            {
                return false;
            }

            return base.ConsumeItem(item, player);
        }

        public override void RightClick(Item item, Player player)
        {
            if (Main.mouseItem.type == ModContent.ItemType<ReforgeStone>())
            {
                Main.NewText("AHHHHHHH");
            }

            base.RightClick(item, player);
        }

        public override bool CanRightClick(Item item)
        {
            if (Main.mouseItem.type == ModContent.ItemType<ReforgeStone>())
            {
                // Check if the prefix is even valid for the item
                if (item.DamageType == DamageClass.Melee)
                {
                    // The item has a knockback prefix but it doesn't even have knockback
                    if (item.knockBack == 0 && Array.IndexOf(ReforgeStone.knockbackPrefixes, Main.mouseItem.prefix) > -1)
                    {
                        Main.NewText("NO KNOCKBACK");
                    }
                    else if (item.crit <= 0 && Array.IndexOf(ReforgeStone.critPrefixes, Main.mouseItem.prefix) > -1)  // The item has a crit prefix but it doesn't even have critical strike chance
                    {
                        Main.NewText("NO CRIT");
                    }
                    else
                    {
                        item.SetDefaults(item.type); // Reset the item to prevent stacking
                        item.Prefix(0);
                        item.Prefix(Main.mouseItem.prefix); // FUCKING WHY DOES IT NOT GIVE YO UTHE RIGHT PREFIX   
                        Main.NewText("item prefix is " + item.prefix);
                        Main.NewText("held item prefix is " + Main.mouseItem.prefix);
                    }
                }    
            }

            return base.CanRightClick(item);
        }

        public override void UpdateInventory(Item item, Player player)
        {
            /*if (Main.mouseItem.type == ModContent.ItemType<ReforgeStone>())
            {
                if (Main.mouseRight)
                {
                    if (Main.HoverItem.DamageType == DamageClass.Melee)
                    {
                        Main.NewText("i am RIGHT CLICKING while being HLEd OVER AN ITEM");

                     

                        if (Main.HoverItem.type == item.type)
                        {
                            item.Prefix(Main.mouseItem.prefix);
                        }

                        Main.mouseItem.TurnToAir();
                    }
                }
            }*/

            base.UpdateInventory(item, player);
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (player.GetModPlayer<OvermorrowModPlayer>().SerpentTooth)
            {
                damage.Flat += 5;
            }

            if (player.GetModPlayer<OvermorrowModPlayer>().PredatorTalisman)
            {
                damage.Flat += 3;
            }

            if (player.GetModPlayer<OvermorrowModPlayer>().EruditeDamage)
            {
                damage.Flat += 2;
            }

            if (player.GetModPlayer<OvermorrowModPlayer>().moonBuff)
            {
                damage.Flat += 4;
            }
        }

        public override void AddRecipes()
        {
            Recipe.Create(ItemID.MagicMirror)
                .AddIngredient(ItemID.RecallPotion, 7)
                .Register();
        }
    }
}