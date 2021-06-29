using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Piercing;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Items.Other;

namespace OvermorrowMod
{
    public class OvermorrowGlobalItem : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            if (item.type == ItemID.ChainKnife)
            {
                item.damage = 5;
                item.value = Item.sellPrice(gold: 2, silver: 75);
            }
        }
        public override void ModifyWeaponDamage(Item item, Player player, ref float add, ref float mult, ref float flat)
        {
            if (player.GetModPlayer<OvermorrowModPlayer>().EruditeDamage)
            {
                flat = 2;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe Mirror = new ModRecipe(mod);
            Mirror.AddIngredient(2350, 7);
            Mirror.SetResult(ItemID.MagicMirror);
            Mirror.AddRecipe();
        }
        
        public override bool Shoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (item.type == ItemID.ChainKnife)
            {
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<ChainKnifeProjectile>(), damage, 0f, player.whoAmI);
                return false;
            }
            return base.Shoot(item, player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }
    }
}