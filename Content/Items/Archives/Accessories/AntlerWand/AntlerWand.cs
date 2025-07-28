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
    public class AntlerWand : OvermorrowAccessory, ITooltipEntities
    {
        public List<TooltipEntity> TooltipObjects()
        {
            var title = Language.GetTextValue(LocalizationPath.TooltipEntities + "FaeOrb" + ".DisplayName");
            var line = Language.GetTextValue(LocalizationPath.TooltipEntities + "FaeOrb" + ".Description.Line0");
            var line2 = Language.GetTextValue(LocalizationPath.TooltipEntities + "FaeOrb" + ".Description.Line1");

            return [
                new ProjectileTooltip(ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "FaeOrb").Value,
                    title,
                    [line, line2],
                    20f,
                    ProjectileTooltipType.Projectile,
                    DamageClass.Summon),
            ];
        }

        public override string Texture => AssetDirectory.ArchiveItems + Name;
        protected override void SafeSetDefaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, gold: 1, silver: 50, copper: 0);
        }

        public int WandDelay { get; private set; } = ModUtils.SecondsToTicks(2);
        protected override void UpdateAccessoryEffects(Player player)
        {
            player.maxMinions += 2;
            player.statLifeMax2 -= 60;

            if (WandDelay > 0)
                WandDelay--;
            else
            {
                WandDelay = ModUtils.SecondsToTicks(5);

                // if near any enemies:
                if (ModUtils.HasNearbyNPCs(player.Center, ModUtils.TilesToPixels(48)))
                {
                    int activeMinions = player.GetActiveMinionCount();
                    if (activeMinions > 0)
                    {
                        SpawnWandOrbs(player, activeMinions);
                    }
                }
            }
        }

        private void SpawnWandOrbs(Player player, int numberOfOrbs)
        {
            float angleStep = MathHelper.TwoPi / numberOfOrbs;
            float radius = 80f;

            for (int i = 0; i < numberOfOrbs; i++)
            {
                float angle = angleStep * i;
                Vector2 spawnOffset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
                Vector2 spawnPosition = player.Center;

                Vector2 velocity = spawnOffset.SafeNormalize(Vector2.Zero) * 8f;

                Projectile.NewProjectile(player.GetSource_Accessory(Item), spawnPosition, velocity, ModContent.ProjectileType<WandOrb>(), 20, 2f, player.whoAmI);
            }
        }

        protected override void SetAccessoryEffects(AccessoryDefinition definition)
        {
        }
    }
}