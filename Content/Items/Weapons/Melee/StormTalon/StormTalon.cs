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
            //Item.autoReuse = true;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 33;
            Item.scale = 1.1f;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.width = 58;
            Item.height = 58;
            Item.shoot = ModContent.ProjectileType<StormTalonProjectile>();
            Item.shootSpeed = 4f;
            Item.knockBack = 3.9f;
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.sellPrice(gold: 1);
            Item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player)
        {
            // Ensures no more than one spear can be thrown out, use this when using autoReuse
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
    }
}