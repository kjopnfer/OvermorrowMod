using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class SandStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sandstorm Staff");
            Tooltip.SetDefault("Shoots sand towards the mouse cursor to batter your enemies\n'It gets everywhere'");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.mana = 10;
            item.UseSound = SoundID.Item8;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 12;
            item.useTurn = false;
            item.useAnimation = 20;
            item.useTime = 20;
            item.width = 54;
            item.height = 54;
            item.shoot = ModContent.ProjectileType<SandBolt>();
            item.shootSpeed = 14f;
            item.knockBack = 0.5f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1, silver: 75);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < 3; i++)
            {
                // The target for the projectile to move towards
                Vector2 target = Main.MouseWorld;
                position += Vector2.Normalize(new Vector2(speedX, speedY));
                float speed = (float)(3.0 + (double)(Main.rand.NextFloat() * 6.0));
                Vector2 start = Vector2.UnitY.RotatedByRandom(6.32);
                Projectile.NewProjectile(position.X, position.Y, start.X * speed, start.Y * speed, type, damage, knockBack, player.whoAmI, target.X, target.Y);
            }

            return false;
        }
    }
}