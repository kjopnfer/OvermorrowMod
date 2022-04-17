using Microsoft.Xna.Framework;
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

            Item.damage = 0;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.width = 22;
            Item.height = 22;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.rare = ItemRarityID.Expert;
            Item.noMelee = true;
            Item.value = Item.sellPrice(0, 3, 50, 0);
            Item.shoot = ModContent.ProjectileType<Atom>();
            Item.buffType = ModContent.BuffType<AtomBuff>();
        }

        public override bool CanUseItem(Player player)
        {
            // The equip slot
            return player.miscEquips[1].IsAir;
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600, true);
            }
        }
    }
}