using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Weapons.Bows;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items;
using OvermorrowMod.Core.Items.Bows;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Weapons
{
    public class PatronusBow_Held : HeldBow
    {
        public override string Texture => AssetDirectory.ArchiveProjectiles + "PatronusBow";
        public override int ParentItem => ModContent.ItemType<PatronusBow>();
        public override BowStats GetBaseBowStats()
        {
            return new BowStats
            {
                ChargeSpeed = 1.2f,
                MaxChargeTime = 45f,
                ShootDelay = 20f,
                MaxSpeed = 14f,
                DamageMultiplier = 1.1f,
                StringColor = Color.White,
                PositionOffset = new Vector2(12, 0),
                StringPositions = (new Vector2(-4, 18), new Vector2(-4, -16))
            };
        }

        protected override Texture2D GetCustomArrowTexture(Texture2D defaultTexture, bool isPowerShot)
        {
            return ModContent.Request<Texture2D>(AssetDirectory.ArchiveProjectiles + "PatronusArrow").Value;
        }

        protected override int GetArrowTypeForShot(int defaultArrowType, bool isPowerShot)
        {
            if (isPowerShot)
            {
                //return ModContent.ProjectileType<Patronus>();
            }

            return ModContent.ProjectileType<WispArrow>();
        }

        protected override void OnArrowFired(Projectile arrow, bool isPowerShot)
        {
            if (isPowerShot)
            {
                //Vector2 direction = Vector2.Normalize(arrow.velocity);
                //arrow.velocity = direction * 8f;
            }

            base.OnArrowFired(arrow, isPowerShot);
        }
    }

    public class PatronusBow : ModBow<PatronusBow_Held>, IWeaponClassification
    {
        public override string Texture => AssetDirectory.ArchiveItems + Name;

        public WeaponType WeaponType => WeaponType.Bow;

        public override void SafeSetDefaults()
        {
            Item.damage = 32;
            Item.width = 24;
            Item.height = 72;
            Item.autoReuse = true;
            Item.shootSpeed = 10f;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 25;
            Item.useAnimation = 25;
        }
    }
}