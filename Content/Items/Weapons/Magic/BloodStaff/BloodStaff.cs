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
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.mana = 10;
            item.UseSound = SoundID.Item21;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 17;
            item.useTurn = false;
            item.useAnimation = 24;
            item.useTime = 24;
            item.width = 56;
            item.height = 56;
            item.shoot = ModContent.ProjectileType<SplittingBlood>();
            item.shootSpeed = 15f;
            item.knockBack = 3f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1, silver: 75);
        }
    }
}