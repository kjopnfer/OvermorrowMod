using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.StormTalon
{
    public class StormTalon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Talon");
            Tooltip.SetDefault("Leaves a trail of electric sparks");
        }

        public override void SetDefaults()
        {
            //item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item1;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 33;
            item.scale = 1.1f;
            item.useAnimation = 30;
            item.useTime = 30;
            item.width = 58;
            item.height = 58;
            item.shoot = ModContent.ProjectileType<StormTalonProjectile>();
            item.shootSpeed = 4f;
            item.knockBack = 3.9f;
            item.melee = true;
            item.value = Item.sellPrice(gold: 1);
            item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player)
        {
            // Ensures no more than one spear can be thrown out, use this when using autoReuse
            return player.ownedProjectileCounts[item.shoot] < 1;
        }
    }
}