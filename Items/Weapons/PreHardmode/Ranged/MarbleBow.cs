using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Ranged
{
    public class MarbleBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spellbolt Bow");
            Tooltip.SetDefault("Right click to consume mana and empower your arrows");
        }

        public override void SetDefaults()
        {
            //item.autoReuse = true;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item5;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 25;
            item.useAnimation = 25;
            item.useTime = 25;
            item.width = 30;
            item.height = 60;
            item.shoot = AmmoID.Arrow;
            item.shootSpeed = 8f;
            item.knockBack = 10f;
            item.ranged = true;
            item.value = Item.sellPrice(gold: 1);
            item.useAmmo = AmmoID.Arrow;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6, 0);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.useAnimation = 25;
                item.useTime = 25;
                item.autoReuse = true;
                item.mana = 10;
                item.damage = 28;
                item.shootSpeed = 14f;
                item.knockBack = 10f;
            }
            else
            {
                item.useAnimation = 19;
                item.useTime = 19;
                item.autoReuse = false;
                item.mana = 0;
                item.damage = 25;
                item.shootSpeed = 8f;
                item.knockBack = 4f;
            }
            return base.CanUseItem(player);
        }
    }
}