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
    public class ArcanistsHammer : OvermorrowAccessory, ITooltipEntities
    {
        public List<TooltipEntity> TooltipObjects()
        {
            var title = Language.GetTextValue(LocalizationPath.TooltipEntities + "ArcanistsHammer" + ".DisplayName");
            var line = Language.GetTextValue(LocalizationPath.TooltipEntities + "ArcanistsHammer" + ".Description.Line0");
            var line2 = Language.GetTextValue(LocalizationPath.TooltipEntities + "ArcanistsHammer" + ".Description.Line1");

            return new List<TooltipEntity>() {
                new ProjectileTooltip(ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "ArcanistsHammer").Value,
                    title,
                    [line, line2],
                    0f,
                    ProjectileTooltipType.Projectile,
                    DamageClass.Melee),
            };
        }

        public override string Texture => AssetDirectory.ArchiveItems + Name;
        protected override void SafeSetDefaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, gold: 1, silver: 50, copper: 0);
            Item.defense = 4;
        }

        public int HammerDelay { get; private set; } = 0;
        protected override void UpdateAccessoryEffects(Player player)
        {
            if (HammerDelay > 0) HammerDelay--;
        }

        protected override void SetAccessoryEffects(AccessoryDefinition definition)
        {
            definition.AddStrikeEffect(
                condition: (player, npc, hitInfo, damage) =>
                {
                    if (Main.rand.NextFloat() > 0.20f)
                        return false;

                    float distance = Vector2.Distance(player.Center, npc.Center);
                    return GetInstance<ArcanistsHammer>(player).HammerDelay <= 0 && distance <= ModUtils.TilesToPixels(8);
                },
                effect: (player, npc, hitInfo, damage) =>
                {
                    //GetInstance<ArcanistsHammer>(player).HammerDelay = ModUtils.SecondsToTicks(5);
                    GetInstance<ArcanistsHammer>(player).HammerDelay = ModUtils.SecondsToTicks(2);

                    var item = ItemLoader.GetItem(ModContent.ItemType<HauntedAthame>()).Item;
                    Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, Vector2.Zero, ModContent.ProjectileType<HammerSwing>(), 50, 2f, player.whoAmI);
                }
            );
        }
    }
}