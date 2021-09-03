using OvermorrowMod.Projectiles.Melee.ThunderClap;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Melee
{
	public class ThunderClap : ModItem
	{
        public override void SetStaticDefaults()
        {
			DisplayName.SetDefault("ThunderClap");
			Tooltip.SetDefault("Throws a flail that shocks nearby enemies\n'Begone with the thunder clap'");
        }
        public override void SetDefaults()
		{
			item.width = 22;
			item.height = 20;
			item.rare = ItemRarityID.Orange;
			item.noMelee = true;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.useAnimation = 40;
			item.useTime = 40;
			item.knockBack = 4f;
			item.damage = 24;
			item.noUseGraphic = true;
			item.shoot = ModContent.ProjectileType<ThunderClapHead>();
			item.shootSpeed = 15.1f;
			item.UseSound = SoundID.Item1;
			item.melee = true;
			item.crit = 9;
			item.channel = true;
			item.value = Item.sellPrice(silver: 5);
		}
	}
}