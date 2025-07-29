using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Items.Daggers;
using OvermorrowMod.Core.Items.Daggers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Weapons
{
    public class CarvingKnife : ModDagger<CarvingKnife_Held, CarvingKnife_Thrown>
    {
        public override string Texture => AssetDirectory.ArchiveItems + Name;
        public override void SafeSetDefaults()
        {
            Item.damage = 10;
            Item.knockBack = 2f;
            Item.useAnimation = 12;
            Item.useTime = 12;
            Item.width = 32;
            Item.height = 32;
            Item.crit = 20;
            Item.shootSpeed = 2.1f;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, 0, 0, 10);
        }
    }

    public class CarvingKnife_Held : HeldDagger
    {
        public override string Texture => AssetDirectory.ArchiveItems + "CarvingKnife";
        public override int ParentItem => ModContent.ItemType<CarvingKnife>();
        public override int ThrownProjectile => ModContent.ProjectileType<CarvingKnife_Thrown>();

        public override DaggerStats GetBaseDaggerStats()
        {
            return new DaggerBuilder()
                .WithComboSequence(DaggerAttack.Slash)
                .WithSlashTiming(12f, 8f, 6f)
                .WithSpeedMultiplier(1.1f)
                .WithDamageMultiplier(1f)
                .WithThrowVelocity(12f)
                .WithThrowDamageMultiplier(1.25f)
                .Build();
        }

        public override void SafeSetDefaults()
        {
            base.SafeSetDefaults();
        }

        public override void SetWeaponDrawing(ref Vector2 spritePositionOffset, ref Vector2 dualWieldOffset, ref float rotationOffset, ref float scaleFactor)
        {
            switch (ComboIndex)
            {
                case (int)DaggerAttack.Throw:
                    spritePositionOffset = new Vector2(32, -8 * player.direction).RotatedBy(Projectile.rotation);
                    rotationOffset = MathHelper.ToRadians(120 * player.direction);
                    break;
                case (int)DaggerAttack.Slash:
                    dualWieldOffset = DualWieldFlag == 1 ? new Vector2(6, -6) : Vector2.Zero;
                    spritePositionOffset = new Vector2(8 + dualWieldOffset.X, (16 + dualWieldOffset.Y) * player.direction).RotatedBy(Projectile.rotation);
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
                default:
                    hitboxOffset = new Vector2(25, -5 * player.direction).RotatedBy(Projectile.rotation);
                    hitbox.X = (int)(player.Center.X - (hitbox.Width / 2f) + hitboxOffset.X);
                    hitbox.Y = (int)(player.Center.Y - (hitbox.Height / 2f) + hitboxOffset.Y);
                    break;
            }
        }
    }

    public class CarvingKnife_Thrown : ThrownDagger
    {
        public override string Texture => AssetDirectory.ArchiveItems + "CarvingKnife";
        public override int ParentItem => ModContent.ItemType<CarvingKnife>();
        public override Color IdleColor => Color.Gray;
    }
}