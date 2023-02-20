using Microsoft.Xna.Framework;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Content.Items.Weapons.Ranged.GraniteLauncher;
using OvermorrowMod.Core;
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
                player.setBonus = "Critical hits with Revolvers rebound to the nearest enemy";
                player.GetModPlayer<GunPlayer>().CowBoySet = true;
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

        /// <summary>
        /// Determines whether or not the item is equipped in vanity based on if the 'Social' tooltip is displayed
        /// </summary>
        private bool CheckInVanity(List<TooltipLine> tooltips)
        {
            for (int lines = 0; lines < tooltips.Count; lines++)
            {
                if (tooltips[lines].Name == "Social") return true;
            }

            return false;
        }

      
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            //if (player.CheckArmorEquipped(ItemID.CowboyHat)) Main.NewText("a" + player.armor[0].ToString());
        }

        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            if (player.CheckArmorEquipped(ItemID.CowboyJacket) && item.DamageType == DamageClass.Ranged)
            {
                if (item.GetWeaponType() == GunType.Revolver)
                {

                    crit += 1f;
                    //Main.NewText("a");
                }
                else
                {
                    crit += 0.1f;
                    //Main.NewText("b");
                }
                //Main.NewText("e" + player.armor[1].ToString());
            }
        }

        public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback)
        {
            //Main.NewText("i" + player.armor[2].ToString());
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (CheckInVanity(tooltips)) return;

            switch (item.type)
            {
                case ItemID.CowboyHat:
                    tooltips.Insert(3, new TooltipLine(Mod, "Tooltip", "Increased ranged critical strike damage by 5%"));
                    break;
                case ItemID.CowboyJacket:
                    tooltips.Insert(3, new TooltipLine(Mod, "Tooltip", "Increased ranged critical strike chance by 10%\nRevolvers gain an additional 5% critical strike chance"));
                    break;
                case ItemID.CowboyPants:
                    tooltips.Insert(3, new TooltipLine(Mod, "Tooltip", "Increased movement speed by 10%"));
                    break;
            }
        }
    }
}