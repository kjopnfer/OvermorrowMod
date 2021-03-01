using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Boss;
using OvermorrowMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
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
            item.mana = 12;
            item.UseSound = SoundID.Item21;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 17;
            item.useTurn = true;
            item.useAnimation = 24;
            item.useTime = 24;
            item.width = 56;
            item.height = 56;
            item.shoot = ModContent.ProjectileType<SplittingBlood_Magic>();
            item.shootSpeed = 15f;
            item.knockBack = 3f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1, silver:75);
        }
    }
}