using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Weapons.Bows;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items;
using OvermorrowMod.Core.Items.Bows;
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