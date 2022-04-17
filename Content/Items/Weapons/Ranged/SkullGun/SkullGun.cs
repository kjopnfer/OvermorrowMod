using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.SkullGun
{
    public class SkullGun : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit Six-Shooter");
            Tooltip.SetDefault("Converts Musket Balls into Fire Bullets\n'Snooze dart sold separately'");
        }

        public override void SetDefaults()
        {
            item.damage = 19;
            item.DamageType = DamageClass.Ranged;
            item.width = 40;
            item.height = 25;
            item.useTime = 7;
            item.autoReuse = true;
            item.useAnimation = 42;
            item.reuseDelay = 75;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = 10000;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item40;
            item.shoot = ProjectileType<SpiritShot>();
            item.autoReuse = false;
            item.shootSpeed = 20.5f;
            item.useAmmo = AmmoID.Bullet;
        }

        public override bool ConsumeAmmo(Player player)
        {
            return !(player.itemAnimation < item.useAnimation - 2);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 15f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(45));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;

            if (type == ProjectileID.Bullet)
            {
                type = ProjectileType<SpiritShot>();
            }

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe1 = new ModRecipe(mod);
            recipe1.AddIngredient(ItemID.Handgun, 1);
            recipe1.AddIngredient(ItemID.HellstoneBar, 20);
            recipe1.AddIngredient(ItemID.Bone, 15);
            recipe1.AddTile(TileID.Anvils);
            recipe1.SetResult(this);
            recipe1.AddRecipe();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
