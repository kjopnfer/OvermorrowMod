using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Items;
using OvermorrowMod.Common.Items.Guns;
using OvermorrowMod.Common.Tooltips;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items.Accessories;
using OvermorrowMod.Core.Items.Guns;
using OvermorrowMod.Core.Loot;
using OvermorrowMod.Core.Loot.Pools;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    [Loot<ArchivePool>(ItemType.Ranged, Rarity.Rare)]
    public class Spellshot : OvermorrowAccessory, IGunModifier, ITooltipEntities
    {
        public override string Texture => AssetDirectory.ArchiveItems + Name;

        public List<TooltipEntity> TooltipObjects()
        {
            var title = Language.GetTextValue(LocalizationPath.TooltipEntities + "ArcaneShot.DisplayName");
            var line0 = Language.GetTextValue(LocalizationPath.TooltipEntities + "ArcaneShot.Description.Line0");
            var line1 = Language.GetTextValue(LocalizationPath.TooltipEntities + "ArcaneShot.Description.Line1");

            return [
                new ProjectileTooltip(ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "ArcaneShot").Value,
                    title,
                    [line0, line1],
                    0f,
                    ProjectileTooltipType.Projectile,
                    DamageClass.Ranged),
            ];
        }

        protected override void SafeSetDefaults()
        {
            Item.width = 34;
            Item.height = 42;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, gold: 1, silver: 50, copper: 0);
        }

        protected override void SetAccessoryEffects(AccessoryDefinition definition) { }

        public void ModifyGunStats(GunStats stats, Player player) { }

        public void OnReloadSuccess(HeldGun gun, Player player, List<BulletObject> bullets)
        {
            gun.EnchantFinalRound(ModContent.ProjectileType<ArcaneBullet>(), AssetDirectory.ArchiveItems + "MagicBullet", Color.White, 2);
        }
    }
}
