using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Ranged
{
    public class JungleSapper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sap Spurter");
            Tooltip.SetDefault("Uses gel as ammo");
        }
        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Blue;
            item.width = 32;
            item.height = 32;
            item.damage = 16;
            item.ranged = true;
            item.noMelee = true;
            item.useTime = 26;
            item.useAnimation = 26;
            item.UseSound = SoundID.Item17;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.scale = 0.8f;
            item.channel = true;
            item.knockBack = 0.1f;
            item.shoot = ModContent.ProjectileType<SapSpurt>();
            item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 20f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            return true;
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
