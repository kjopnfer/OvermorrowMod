using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.PreHardmode.Melee
{
    public class ScarTissue : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Makes enemies bleed \nBleeding ignores defence");
        }

        public override void SetDefaults()
        {
            item.damage = 20;
            item.melee = true;
            item.width = 50;
            item.height = 50;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 0.5f;
            item.value = 10000;
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item71;
            item.autoReuse = false;
        }


        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            Projectile.NewProjectile(target.Center.X, target.Center.Y, target.velocity.X, target.velocity.Y, mod.ProjectileType("KeyAmmo"), 5, 0, player.whoAmI);
        }
    }
}