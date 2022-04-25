using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.HammerThrow
{
    public class HammerThrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hammer Throw");
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {

            Item.width = 32;
            Item.height = 32;
            Item.damage = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.useTime = 51;
            Item.useAnimation = 51;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
            Item.knockBack = 0;
            Item.shoot = ModContent.ProjectileType<VeloIncrease>();
            Item.shootSpeed = 11f;
            Item.channel = true;
        }
    }
}