using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Items.Weapons.Ranged.GraniteLauncher;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.VanillaOverrides
{
    public class VanillaItemOverrides : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            switch (item.type)
            {
                case ItemID.CowboyHat:
                    item.defense = 4;
                    item.vanity = false;
                    break;
                case ItemID.CowboyJacket:
                    item.defense = 7;
                    item.vanity = false;
                    break;
                case ItemID.CowboyPants:
                    item.defense = 5;
                    item.vanity = false;
                    break;
            }
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == "CowBoySet")
            {
                player.setBonus = "\nCritical hits with Revolvers rebound to the nearest enemy";
            }

            base.UpdateArmorSet(player, set);
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == ItemID.CowboyHat && body.type == ItemID.CowboyJacket && legs.type == ItemID.CowboyPants) return "CowBoySet";

            return base.IsArmorSet(head, body, legs);
        }

        public override void UpdateVanity(Item item, Player player)
        {
            base.UpdateVanity(item, player);
        }

        public override void UpdateEquip(Item item, Player player)
        {
            switch (item.type)
            {
                case ItemID.CowboyHat:
                    break;
                case ItemID.CowboyJacket:
                    break;
                case ItemID.CowboyPants:
                    player.moveSpeed += .1f;
                    break;
            }
        }

        private bool CheckInVanity(List<TooltipLine> tooltips)
        {
            for (int lines = 0; lines < tooltips.Count; lines++)
            {
                if (tooltips[lines].Name == "Social") return true;
            }

            return false;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {

            if (CheckInVanity(tooltips)) return;

            switch (item.type)
            {
                case ItemID.CowboyHat:
                    tooltips.Add(new TooltipLine(Mod, "Tooltip", "Increased critical strike damage by 5%\n"));
                    break;
                case ItemID.CowboyJacket:
                    tooltips.Add(new TooltipLine(Mod, "Tooltip", "Increased critical strike chance for Revolvers by 10%"));
                    break;
                case ItemID.CowboyPants:
                    tooltips.Add(new TooltipLine(Mod, "Tooltip", "Increased movement speed by 10%"));
                    break;
            }

            /*for (int lines = 0; lines < tooltips.Count; lines++)
            {
                if (tooltips[lines].Name == "Speed")
                {
                    tooltips.RemoveAt(lines);
                    tooltips.Insert(lines, new TooltipLine(Mod, "Charge",
                        string.Format("Takes {0} {1} to fully charge",
                        (BowsToOverride[i].ChargeTime - (float)trajectoryPlayer.bowTimingReduce) / 60f,
                        ((float)BowsToOverride[i].ChargeTime / 60f == 1f) ? "second" : "seconds")));
                }
            }*/
        }
    }
}