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
            Item.damage = 19;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 25;
            Item.useTime = 7;
            Item.autoReuse = true;
            Item.useAnimation = 42;
            Item.reuseDelay = 75;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item40;
            Item.shoot = ProjectileType<SpiritShot>();
            Item.autoReuse = false;
            Item.shootSpeed = 20.5f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override bool CanConsumeAmmo(Player player)
        {
            return player.itemAnimation > Item.useAnimation - 2;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 15f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(45));

            if (type == ProjectileID.Bullet)
            {
                type = ProjectileType<SpiritShot>();
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Handgun, 1)
                .AddIngredient(ItemID.HellstoneBar, 20)
                .AddIngredient(ItemID.Bone, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
