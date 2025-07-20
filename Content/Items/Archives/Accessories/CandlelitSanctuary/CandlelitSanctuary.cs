
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Items;
using OvermorrowMod.Common.Tooltips;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items.Accessories;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class CandlelitSanctuary : OvermorrowAccessory, ITooltipEntities
    {
        public List<TooltipEntity> TooltipObjects()
        {
            var title = Language.GetTextValue(LocalizationPath.TooltipEntities + "CandleBurst" + ".DisplayName");
            var line = Language.GetTextValue(LocalizationPath.TooltipEntities + "CandleBurst" + ".Description.Line0");
            var line2 = Language.GetTextValue(LocalizationPath.TooltipEntities + "CandleBurst" + ".Description.Line1");

            return new List<TooltipEntity>() {
                new ProjectileTooltip(ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "CandleBurst").Value,
                    title,
                    [line, line2],
                    15f,
                    ProjectileTooltipType.Projectile,
                    DamageClass.Magic),
            };
        }

        public override string Texture => AssetDirectory.ArchiveItems + Name;

        public int CandleCharges { get; private set; } = 0;
        private int CandleCounter = 0;
        protected override void SafeSetDefaults()
        {
            Item.width = 30;
            Item.height = 40;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(0, 5, 0, 0);
        }
        public override void ResetVariables()
        {
            CandleCharges = 0;
            CandleCounter = 0;
        }

        protected override void UpdateAccessoryEffects(Player player)
        {
            if (CandleCharges > 0)
                Lighting.AddLight(player.Center, new Vector3(0.8f, 0.5f, 1f) * 1.2f);

            if (CandleCharges < 3)
            {
                CandleCounter++;
                if (CandleCounter >= ModUtils.SecondsToTicks(15))
                {
                    CandleCharges++;
                    CandleCounter = 0;

                    Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<CandleGain>(), 0, 0, player.whoAmI);
                }
            }
        }

        protected override void SetAccessoryEffects(AccessoryDefinition definition)
        {
            definition.AddRetaliateEffect(
                condition: (player, attacker, hurtInfo) =>
                {
                    return GetInstance<CandlelitSanctuary>(player).CandleCharges > 0;
                },
                effect: (player, attacker, hurtInfo) =>
                {
                    if (GetInstance<CandlelitSanctuary>(player).CandleCharges > 0)
                    {
                        var damageReduction = 15 * GetInstance<CandlelitSanctuary>(player).CandleCharges;
                        hurtInfo.Damage = Math.Max(1, hurtInfo.Damage - damageReduction);

                        GetInstance<CandlelitSanctuary>(player).CandleCounter = 0;

                        var item = ItemLoader.GetItem(ModContent.ItemType<CandlelitSanctuary>()).Item;
                        Projectile.NewProjectile(player.GetSource_Accessory_OnHurt(item, hurtInfo.DamageSource), player.Center, Vector2.Zero, ModContent.ProjectileType<CandleBurst>(), damageReduction, 6f, player.whoAmI);

                        GetInstance<CandlelitSanctuary>(player).CandleCharges = 0;
                    }
                }
            );
        }
    }
}