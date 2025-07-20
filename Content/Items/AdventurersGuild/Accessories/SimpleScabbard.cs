using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Items;
using OvermorrowMod.Common.Tooltips;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Buffs;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items.Accessories;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.AdventurersGuild.Accessories
{
    [AutoloadEquip(EquipType.Waist)]
    public class SimpleScabbard : OvermorrowAccessory, ITooltipEntities
    {
        public List<TooltipEntity> TooltipObjects()
        {
            var path = LocalizationPath.TooltipEntities + "FocusedBlade";
            var title = Language.GetTextValue(path + ".DisplayName");
            var line = Language.GetTextValue(path + ".Description.Line0");
            var line2 = Language.GetTextValue(path + ".Description.Line1");

            return new List<TooltipEntity>() {
                new BuffTooltip(ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "CandleBurst").Value,
                    title,
                    [line, line2],
                    1,
                    BuffTooltipType.Buff),
            };
        }
        public override string Texture => AssetDirectory.GuildItems + Name;

        protected override void SafeSetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, gold: 1, silver: 50, copper: 0);
        }

        protected override void UpdateAccessoryEffects(Player player)
        {
            base.UpdateAccessoryEffects(player);
        }

        protected override void SetAccessoryEffects(AccessoryDefinition definition)
        {
            definition.AddRespiteEffect(
                condition: (player) => true,
                effect: (player) =>
                {
                    if (!player.HasBuff<FocusedBlade>())
                        player.Heal(20);

                    player.AddBuff(ModContent.BuffType<FocusedBlade>(), ModUtils.SecondsToTicks(1));
                }
            );

            // This needs explicit parameter type specification because idk
            definition.AddTrueMeleeEffect(
                condition: (Player player, Item item, Projectile projectile, NPC target, ref NPC.HitModifiers modifiers) => player.HasBuff<FocusedBlade>(),
                effect: (Player player, Item item, Projectile projectile, NPC target, ref NPC.HitModifiers modifiers) =>
                {
                    modifiers.SourceDamage *= 2f;
                    modifiers.SetCrit();

                    player.ClearBuff(ModContent.BuffType<FocusedBlade>());
                }
            );
        }
    }
}