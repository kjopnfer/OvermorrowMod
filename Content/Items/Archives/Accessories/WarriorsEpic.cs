using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Items;
using OvermorrowMod.Common.Tooltips;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Buffs;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items.Accessories;
using OvermorrowMod.Core.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories
{
    public class WarriorsEpic : OvermorrowAccessory, ITooltipEntities
    {
        public List<TooltipEntity> TooltipObjects()
        {
            var title = Language.GetTextValue(LocalizationPath.Buffs + "WarriorsResolve" + ".DisplayName");
            var line = Language.GetTextValue(LocalizationPath.TooltipEntities + "WarriorsResolve" + ".Description.Line0");
            var line2 = Language.GetTextValue(LocalizationPath.TooltipEntities + "WarriorsResolve" + ".Description.Line1");
            var line3 = Language.GetTextValue(LocalizationPath.TooltipEntities + "WarriorsResolve" + ".Description.Line2");

            return new List<TooltipEntity>() {
                new BuffTooltip(ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "WarriorsResolve").Value,
                    title,
                    [line, line2, line3],
                    10,
                    BuffTooltipType.Buff),
            };
        }

        public override string Texture => AssetDirectory.ArchiveItems + Name;

        protected override void SafeSetDefaults()
        {
            Item.width = 38;
            Item.height = 42;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public int WarriorsEpicCooldown { get; private set; } = 0;

        /// <summary>
        /// Determines whether we have activated the Heal after killing an enemy.
        /// </summary>
        public bool WarriorsResolveTriggered { get; private set; } = false;
        protected override void UpdateAccessoryEffects(Player player)
        {
            if (WarriorsEpicCooldown > 0) WarriorsEpicCooldown--;

            if (!player.HasBuff<WarriorsResolve>())
                WarriorsResolveTriggered = false;

            if (player.HasBuff<WarriorsResolve>() && !WarriorsResolveTriggered)
            {
                player.GetCritChance(DamageClass.Melee) += 100; // 100% crit chance bonus
            }
        }

        protected override void SetAccessoryEffects(AccessoryDefinition definition)
        {
            definition.AddDeathsDoorEffect(
                condition: (player) =>
                {
                    return GetInstance<WarriorsEpic>(player).WarriorsEpicCooldown <= 0;
                },
                effect: (player) =>
                {
                    player.AddBuff(ModContent.BuffType<WarriorsResolve>(), ModUtils.SecondsToTicks(10));
                    GetInstance<WarriorsEpic>(player).WarriorsEpicCooldown = ModUtils.SecondsToTicks(10);
                }
            );

            // Heal when killing an enemy while Warriors Resolve is active but prevent healing multiple times
            definition.AddExecuteEffect(
                condition: (player, killedNPC) =>
                {
                    return player.HasBuff<WarriorsResolve>() && !GetInstance<WarriorsEpic>(player).WarriorsResolveTriggered;
                },
                effect: (player, killedNPC) =>
                {
                    player.Heal(40);
                    GetInstance<WarriorsEpic>(player).WarriorsResolveTriggered = true;
                }
            );

            definition.AddVigorEffect(
                condition: (player) => true, // Vigor keyword already checks for 80%+ health
                effect: (player) =>
                {
                    player.GetDamage(DamageClass.Melee) += 0.15f;
                }
            );
        }

        public static void DrawEffects(Player player, PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            var modPlayer = player.GetModPlayer<OldAccessoryPlayer>();
            if (!player.HasBuff<WarriorsResolve>() || modPlayer.WarriorsResolveTriggered || Main.gamePaused)
                return;

            int delay = (int)(Main.rand.NextFloat(0.5f, 0.7f) * 15);
            if (Main.GameUpdateCount % delay != 0)
                return;

            Texture2D texture = ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/trace_05").Value;
            int widthRange = player.width + 8;
            int heightRange = player.height - 4;
            int stepSize = 2;
            int maxXSteps = widthRange / stepSize;
            int maxYSteps = heightRange / stepSize;

            for (int i = 0; i < 2; i++)
            {
                Vector2 offset = new Vector2(
                    Main.rand.Next(-maxXSteps, maxXSteps + 1) * stepSize,
                    Main.rand.Next(-maxYSteps, 1) * stepSize
                );

                var aura = new Spark(texture, ModUtils.SecondsToTicks(Main.rand.NextFloat(0.7f, 1f)), false)
                {
                    endColor = Color.DarkRed,
                    slowModifier = 0.98f,
                    squashHeight = false
                };

                float scale = Main.rand.NextFloat(0.1f, 0.2f);
                Vector2 velocity = -Vector2.UnitY * Main.rand.Next(3, 4);

                ParticleManager.CreateParticleDirect(aura, player.Bottom + offset, velocity, Color.Red, 1f, scale, 0f, useAdditiveBlending: true);
            }
        }
    }
}