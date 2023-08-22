using Microsoft.Xna.Framework;
using OvermorrowMod.Common.VanillaOverrides.Melee;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee
{
    public class Windrunner : ModDagger<Windrunner_Held, Windrunner_Thrown>
    {
        public override void SafeSetDefaults()
        {
            Item.damage = 23;
            Item.knockBack = 2f;
            Item.useAnimation = 12;
            Item.useTime = 12;
            Item.width = 32;
            Item.height = 32;
            Item.crit = 20;
            Item.shootSpeed = 2.1f;

            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(0, 0, 0, 10);
        }
    }

    public class Windrunner_Held : HeldDagger
    {
        public override int ParentItem => ModContent.ItemType<Windrunner>();
        public override int ThrownProjectile => ModContent.ProjectileType<Windrunner_Thrown>();

        public override void SetWeaponDrawing(ref Vector2 spritePositionOffset, ref Vector2 dualWieldOffset, ref float rotationOffset, ref float scaleFactor)
        {
            scaleFactor = DualWieldFlag == 1 ? 0.8f : 1f;

            switch (ComboIndex)
            {
                case -1: // The throwing index
                    spritePositionOffset = new Vector2(6, 0).RotatedBy(Projectile.rotation);
                    rotationOffset = MathHelper.ToRadians(-145 * player.direction);
                    break;
                case 0:
                    dualWieldOffset = DualWieldFlag == 1 ? new Vector2(4, -4) : Vector2.Zero;
                    spritePositionOffset = new Vector2(-12 + dualWieldOffset.X, (22 + dualWieldOffset.Y) * player.direction).RotatedBy(Projectile.rotation);
                    rotationOffset = MathHelper.ToRadians(135 * player.direction);
                    break;
                case 1:
                    dualWieldOffset = DualWieldFlag == 1 ? new Vector2(4, -4) : Vector2.Zero;
                    spritePositionOffset = new Vector2(12 + dualWieldOffset.X, (2 + dualWieldOffset.Y) * player.direction).RotatedBy(Projectile.rotation);
                    rotationOffset = MathHelper.ToRadians(45 * player.direction);
                    break;
            }
        }

        public override void SetDamageHitbox(Vector2 positionOffset, ref Vector2 hitboxOffset, ref Rectangle hitbox)
        {
            hitbox.Width = 35;
            hitbox.Height = 35;

            switch (ComboIndex)
            {
                case 1:
                    hitboxOffset = positionOffset.RotatedBy(Projectile.rotation);

                    hitbox.X = (int)(player.Center.X - (hitbox.Width / 2f) + hitboxOffset.X);
                    hitbox.Y = (int)(player.Center.Y - (hitbox.Height / 2f) + hitboxOffset.Y);
                    break;
                default:
                    hitboxOffset = new Vector2(25, -5 * player.direction).RotatedBy(Projectile.rotation);
                    hitbox.X = (int)(player.Center.X - (hitbox.Width / 2f) + hitboxOffset.X);
                    hitbox.Y = (int)(player.Center.Y - (hitbox.Height / 2f) + hitboxOffset.Y);
                    break;
            }
        }
    }

    public class Windrunner_Thrown : ThrownDagger
    {
        public override int ParentItem => ModContent.ItemType<Windrunner>();
    }
}