using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace OvermorrowMod.Content.Items.Weapons.Melee

{
	public class GraniteYoyo : ModItem
    {
		public override string Texture => "OvermorrowMod/Content/Items/Weapons/Melee/GraniteYoyo/GraniteYoyo";
		public override void SetStaticDefaults()
        {
			ItemID.Sets.Yoyo[Item.type] = true;
			ItemID.Sets.GamepadExtraRange[Item.type] = 15;
			ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
		}
        public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.width = 28;
			Item.height = 24;
			Item.useAnimation = 25;
			Item.knockBack = 2f;
			Item.useTime = 25;
			Item.shootSpeed = 16f;
			Item.damage = 9;
			Item.DamageType = DamageClass.Melee;
			Item.channel = true;
			Item.rare = ItemRarityID.Blue;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<GraniteYoyoProj>();
		}
		public static int ChannelTimer;
        public override void HoldItem(Player player)
        {	
			if (player.itemTime == 0)
			{
				for (int i = 0; Main.projectile.Length > i; i++)
				{
					if (Main.projectile[i].type == ModContent.ProjectileType<GraniteYoyoOrbit>())
						Main.projectile[i].Kill();
				}
			}
		}
        public override void UpdateInventory(Player player)
        {
			if (player.channel)
				ChannelTimer++;
			else
				ChannelTimer = 0;
		}
    }
}