using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.TreeGuns
{
    public class BrambleCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bramble Blaster");
        }
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.damage = 7;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 48;
            Item.height = 40;
            Item.useTime = 34;
            Item.useAnimation = 34;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.scale = 0.8f;
            Item.noMelee = true;
            Item.knockBack = 0.1f;
            Item.UseSound = SoundID.Item17;
            Item.shoot = ProjectileID.Seed;
            Item.shootSpeed = 7f;
            Item.value = Item.sellPrice(0, 0, 15, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numberProjectiles = Main.rand.Next(2, 3);
            for (int i = 0; i < numberProjectiles; i++)
            {

                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(30f));
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 0.5f, player.whoAmI);
            }
            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
