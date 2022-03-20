using OvermorrowMod.Content.Buffs.Pets;
using OvermorrowMod.Content.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Dev
{
    public class AtomHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Atomic Heart");
        }
        public override void SetDefaults()
        {

            item.damage = 0;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.width = 22;
            item.height = 22;
            item.useAnimation = 20;
            item.useTime = 20;
            item.rare = ItemRarityID.Expert;
            item.noMelee = true;
            item.value = Item.sellPrice(0, 3, 50, 0);
            item.shoot = ModContent.ProjectileType<Atom>();
            item.buffType = ModContent.BuffType<AtomBuff>();
        }

        public override bool CanUseItem(Player player)
        {
            // The equip slot
            return player.miscEquips[1].IsAir;
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }
    }
}