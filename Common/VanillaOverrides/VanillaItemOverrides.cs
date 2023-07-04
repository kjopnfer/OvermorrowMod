using Microsoft.Xna.Framework;
using OvermorrowMod.Common.VanillaOverrides.Gun;
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

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            //if (player.CheckArmorEquipped(ItemID.CowboyHat)) Main.NewText("a" + player.armor[0].ToString());
        }

        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            if (player.CheckArmorEquipped(ItemID.CowboyJacket) && item.DamageType == DamageClass.Ranged)
            {
                if (item.GetWeaponType() == GunType.Revolver)
                    crit += 10f;
                else
                    crit += 5f;
            }
        }

        public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback)
        {
            //Main.NewText("i" + player.armor[2].ToString());
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            
        }
    }
}