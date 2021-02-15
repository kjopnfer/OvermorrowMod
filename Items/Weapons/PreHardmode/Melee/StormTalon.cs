using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Melee
{
    public class StormTalon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Talon");
        }

        public override void SetDefaults()
        {
            item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 30;
            item.useAnimation = 18;
            item.useTime = 24;
            item.width = 58;
            item.height = 58;
            item.shoot = ModContent.ProjectileType<StormTalonProjectile>();
            item.shootSpeed = 3.75f;
            item.knockBack = 3.9f;
            item.melee = true;
            item.value = Item.sellPrice(gold: 1);
        }

        public override bool CanUseItem(Player player)
        {
            // Ensures no more than one spear can be thrown out, use this when using autoReuse
            return player.ownedProjectileCounts[item.shoot] < 1;
        }
    }
}