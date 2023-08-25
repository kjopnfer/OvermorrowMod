using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.VanillaOverrides.Melee;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee
{
    public class AmethystSlicer : ModDagger<AmethystSlicer_Held, AmethystSlicer_Thrown>
    {
        public override List<DaggerAttack> AttackList => new List<DaggerAttack>() { DaggerAttack.Slash };
        public override void SafeSetDefaults()
        {
            Item.damage = 17;
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

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Amethyst, 12)
                .AddIngredient<Knife>(1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class AmethystSlicer_Held : HeldDagger
    {
        public override int ParentItem => ModContent.ItemType<AmethystSlicer>();

        public override int ThrownProjectile => ModContent.ProjectileType<AmethystSlicer_Thrown>();
        public override void SetWeaponDrawing(ref Vector2 spritePositionOffset, ref Vector2 dualWieldOffset, ref float rotationOffset, ref float scaleFactor)
        {
            scaleFactor = DualWieldFlag == 1 ? 0.8f : 1f;

            switch (ComboIndex)
            {
                case (int)DaggerAttack.Throw:
                    spritePositionOffset = new Vector2(6, 0).RotatedBy(Projectile.rotation);
                    rotationOffset = MathHelper.ToRadians(-235 * player.direction);
                    break;
                case (int)DaggerAttack.Slash:
                    dualWieldOffset = DualWieldFlag == 1 ? new Vector2(4, -4) : Vector2.Zero;
                    spritePositionOffset = new Vector2(-12 + dualWieldOffset.X, (22 + dualWieldOffset.Y) * player.direction).RotatedBy(Projectile.rotation);
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
                    //hitbox.Width = 60;

                    hitboxOffset = new Vector2(25, -5 * player.direction).RotatedBy(Projectile.rotation);
                    hitbox.X = (int)(player.Center.X - (hitbox.Width / 2f) + hitboxOffset.X);
                    hitbox.Y = (int)(player.Center.Y - (hitbox.Height / 2f) + hitboxOffset.Y);
                    break;
            }
        }
    }

    public class AmethystSlicer_Thrown : ThrownDagger
    {
        public override int ParentItem => ModContent.ItemType<AmethystSlicer>();
        public override void AI()
        {
            base.AI();
        }

        public override void OnThrownDaggerHit()
        {
            base.OnThrownDaggerHit();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}