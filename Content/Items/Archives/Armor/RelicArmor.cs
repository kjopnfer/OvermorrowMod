using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Tooltips;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Loot;
using OvermorrowMod.Core.Loot.Pools;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Armor
{
    [AutoloadEquip(EquipType.Body)]
    [Loot<ArchivePool>(ItemType.Generic, Rarity.Rare)]
    public class RelicArmor : ModItem, ITooltipEntities
    {
        public override string Texture => AssetDirectory.ArchiveItems + Name;

        public List<TooltipEntity> TooltipObjects()
        {
            var title = Language.GetTextValue(LocalizationPath.TooltipEntities + "RelicBolt.DisplayName");
            var line0 = Language.GetTextValue(LocalizationPath.TooltipEntities + "RelicBolt.Description.Line0");
            var line1 = Language.GetTextValue(LocalizationPath.TooltipEntities + "RelicBolt.Description.Line1");

            return [
                new ProjectileTooltip(ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "RelicBolt").Value,
                    title,
                    [line0, line1],
                    0f,
                    ProjectileTooltipType.Projectile,
                    DamageClass.Generic),
            ];
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 28;
            Item.defense = 8;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, gold: 3);
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Generic) += 6;
            player.GetModPlayer<RelicArmorPlayer>().relicArmorEquipped = true;
        }
    }
}
