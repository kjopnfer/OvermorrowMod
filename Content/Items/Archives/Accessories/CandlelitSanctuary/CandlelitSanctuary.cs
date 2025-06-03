
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Tooltips;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Interfaces;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives
{
    /// <summary>
    /// AbsorbTeamDamageAccessory mimics the unique effect of the Paladin's Shield item.
    /// This example showcases some advanced interplay between accessories, buffs, and ModPlayer hooks.
    /// Of particular note is how this accessory gives other players a buff and how a player might act on another player being hit.
    /// </summary>
    [AutoloadEquip(EquipType.Shield)]
    public class CandlelitSanctuary : ModItem, ITooltipEntities
    {
        public List<TooltipEntity> TooltipObjects()
        {
            var title = Language.GetTextValue(LocalizationPath.TooltipEntities + "CandleBurst" + ".DisplayName");
            var line = Language.GetTextValue(LocalizationPath.TooltipEntities + "CandleBurst" + ".Description.Line0");
            var line2 = Language.GetTextValue(LocalizationPath.TooltipEntities + "CandleBurst" + ".Description.Line1");

            return new List<TooltipEntity>() {
                new ProjectileTooltip(ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "Default").Value,
                    title,
                    [line, line2],
                    15f,
                    ProjectileTooltipType.Projectile,
                    DamageClass.Magic),
            };
        }

        public override string Texture => AssetDirectory.ArchiveItems + Name;
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 40;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(0, 5, 0, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            int charges = player.GetModPlayer<AccessoryPlayer>().CandleCharges;
            if (charges > 0)
                Lighting.AddLight(player.Center, new Vector3(0.8f, 0.5f, 1f) * 1.2f);
        }
    }
}