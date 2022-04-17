using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.BloodStaff
{
    public class BloodStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodburst Staff");
            Tooltip.SetDefault("Shoots a blood ball that explodes into bouncing balls upon hitting an enemy");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Orange;
            Item.mana = 10;
            Item.UseSound = SoundID.Item21;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 17;
            Item.useTurn = false;
            Item.useAnimation = 24;
            Item.useTime = 24;
            Item.width = 56;
            Item.height = 56;
            Item.shoot = ModContent.ProjectileType<BloodSplit>();
            Item.shootSpeed = 15f;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.sellPrice(gold: 1, silver: 75);
        }
    }
}