using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Tooltips;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Interfaces;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories.GuideLantern
{
    public class WarriorsEpic : ModItem, ITooltipEntities
    {
        public List<TooltipEntity> TooltipObjects()
        {
            var title = Language.GetTextValue(LocalizationPath.Buffs + "WarriorsResolve" + ".DisplayName");
            var line = Language.GetTextValue(LocalizationPath.TooltipEntities + "WarriorsResolve" + ".Description.Line0");
            var line2 = Language.GetTextValue(LocalizationPath.TooltipEntities + "WarriorsResolve" + ".Description.Line1");
            var line3 = Language.GetTextValue(LocalizationPath.TooltipEntities + "WarriorsResolve" + ".Description.Line2");

            return new List<TooltipEntity>() {
                new BuffTooltip(ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "Default").Value,
                    title,
                    [line, line2, line3],
                    10,
                    BuffTooltipType.Buff),
            };
        }

        public override string Texture => AssetDirectory.ArchiveItems + Name;

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 38;
            Item.height = 42;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, 1, 0, 0);
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AccessoryPlayer>().WarriorsEpic = true;
            /*if (player.statLife >= player.statLifeMax2 * 0.8f)
            {
                player.GetDamage(DamageClass.Melee) += 0.15f; // 15% damage bonus if at or above 80% health
            }*/
        }
    }
}