using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.VanillaOverrides.Gun
{
    public abstract class ModGun<HeldProjectile> : ModItem where HeldProjectile : HeldGun
    {
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<HeldProjectile>()] <= 0;
        public override bool CanConsumeAmmo(Item ammo, Player player) => false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Test Held Gun");
        }

        public virtual void SafeSetDefaults() { }
        public sealed override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<HeldProjectile>();
            Item.noUseGraphic = true;
            Item.useAmmo = AmmoID.Bullet;

            SafeSetDefaults();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = Item.shoot;
        }
    }

    public class TestGun_Held : HeldGun
    {
        public override int ConvertBullet => ItemID.MusketBall;
        public override int BulletType => ProjectileID.VenomBullet;
        public override int ParentItem => ModContent.GetInstance<TestGun>().Type;
        public override Vector2 PositionOffset => new Vector2(18, -5);
        public override float ProjectileScale => 0.75f;

        public override void OnReloadEnd()
        {
            for (int i = 0; i < 6; i++)
            {
                int gore = Gore.NewGore(null, Projectile.Center, new Vector2(player.direction * -0.01f, 0f), Mod.Find<ModGore>("BulletCasing").Type, 0.75f);
                Main.gore[gore].sticky = true;
            }
        }

        public override void OnReloadEventSuccess()
        {
            reloadTime = 0;

            /*for (int i = 0; i < 6; i++)
            {
                int gore = Gore.NewGore(null, Projectile.Center, new Vector2(player.direction * -0.01f, 0f), Mod.Find<ModGore>("BulletCasing").Type, 0.75f);
                Main.gore[gore].sticky = true;
            }*/
        }

        public override void OnReloadStart()
        {
            
        }
    }

    public class TestGun : ModGun<TestGun_Held>
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Gamer Gun");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 69;
            Item.width = 32;
            Item.height = 74;
            Item.autoReuse = true;
            Item.shootSpeed = 10f;
            Item.rare = ItemRarityID.Lime;
            Item.useTime = 22;
            Item.useAnimation = 22;
        }
    }
}