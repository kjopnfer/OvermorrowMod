using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.ScarTissue
{
    public class ScarTissue : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Makes enemies bleed \nBleeding ignores defence");
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Melee;
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 0.5f;
            Item.value = 10000;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = false;
        }


        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            Projectile.NewProjectile(player.GetProjectileSource_OnHit(target, ProjectileSourceID.None), target.Center.X, target.Center.Y, target.velocity.X, target.velocity.Y, ModContent.ProjectileType<Scar>(), 5, 0, player.whoAmI);
        }
    }
}