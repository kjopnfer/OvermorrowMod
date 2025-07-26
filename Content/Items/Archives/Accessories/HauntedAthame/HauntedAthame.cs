using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Items;
using OvermorrowMod.Common.Tooltips;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items.Accessories;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class HauntedAthame : OvermorrowAccessory, ITooltipEntities
    {
        public List<TooltipEntity> TooltipObjects()
        {
            var title = Language.GetTextValue(LocalizationPath.TooltipEntities + "TaintedAthame" + ".DisplayName");
            var line = Language.GetTextValue(LocalizationPath.TooltipEntities + "TaintedAthame" + ".Description.Line0");
            var line2 = Language.GetTextValue(LocalizationPath.TooltipEntities + "TaintedAthame" + ".Description.Line1");
            var line3 = Language.GetTextValue(LocalizationPath.TooltipEntities + "TaintedAthame" + ".Description.Line2");

            return new List<TooltipEntity>() {
                new ProjectileTooltip(ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "TaintedAthame").Value,
                    title,
                    [line, line2, line3],
                    50f,
                    ProjectileTooltipType.Projectile,
                    DamageClass.Melee),
            };
        }

        public override string Texture => AssetDirectory.ArchiveItems + Name;
        protected override void SafeSetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, gold: 1, silver: 50, copper: 0);
        }

        public int AthameDelay { get; private set; } = 0;
        protected override void UpdateAccessoryEffects(Player player)
        {
            if (AthameDelay > 0) AthameDelay--;
        }

        protected override void SetAccessoryEffects(AccessoryDefinition definition)
        {
            definition.AddRetaliateEffect(
                condition: (player, npc, info) =>
                {
                    return GetInstance<HauntedAthame>(player).AthameDelay <= 0;
                },
                effect: (player, npc, info) =>
                {
                    GetInstance<HauntedAthame>(player).AthameDelay = ModUtils.SecondsToTicks(5);

                    var item = ItemLoader.GetItem(ModContent.ItemType<HauntedAthame>()).Item;
                    Projectile.NewProjectile(player.GetSource_Accessory_OnHurt(item, info.DamageSource), player.Center, Vector2.Zero, ModContent.ProjectileType<TaintedAthame>(), 50, 2f, player.whoAmI);
                }
            );
        }
    }
}