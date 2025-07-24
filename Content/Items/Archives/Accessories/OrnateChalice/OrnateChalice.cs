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
    public class OrnateChalice : OvermorrowAccessory, ITooltipEntities
    {
        public List<TooltipEntity> TooltipObjects()
        {
            string GetText(string key) => Language.GetTextValue(LocalizationPath.TooltipEntities + key);

            var chaliceManaTitle = GetText("ChaliceMana.DisplayName");
            var chaliceManaLines = new[]
            {
                GetText("ChaliceMana.Description.Line0"),
            };

            var chaliceHealthTitle = GetText("ChaliceHealth.DisplayName");
            var chaliceHealthLines = new[]
            {
                GetText("ChaliceHealth.Description.Line0"),
            };

            var chaliceManaTexture = ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "Shadowbrand").Value;
            var chaliceHealthTexture = ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "Shadowbrand").Value;

            return new List<TooltipEntity>
            {
                new BuffTooltip(chaliceHealthTexture, chaliceHealthTitle, chaliceHealthLines, 5, BuffTooltipType.Buff),
                new BuffTooltip(chaliceManaTexture, chaliceManaTitle, chaliceManaLines, 5, BuffTooltipType.Buff)
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

        public int ChaliceCounter { get; private set; } = 0;
        protected override void UpdateAccessoryEffects(Player player)
        {
            if (ChaliceCounter > 0) ChaliceCounter--;

            player.AddHealthRegenPerSecond(1);
            player.AddManaRegenPerSecond(1);
        }

        protected override void SetAccessoryEffects(AccessoryDefinition definition)
        {
            definition.AddDeathsDoorEffect(
                condition: (player) => GetInstance<OrnateChalice>(player).ChaliceCounter <= 0,
                effect: (player) =>
                {
                    player.AddBuff(ModContent.BuffType<Buffs.ChaliceHealth>(), ModUtils.SecondsToTicks(5));
                    GetInstance<OrnateChalice>(player).ChaliceCounter = ModUtils.SecondsToTicks(40);

                    int projectileCount = Main.rand.Next(6, 10);
                    for (int i = 0; i < projectileCount; i++)
                    {
                        float randomAngle = Main.rand.NextFloat(0f, MathHelper.TwoPi);

                        Vector2 velocity = new Vector2((float)Math.Cos(randomAngle), (float)Math.Sin(randomAngle));
                        float speed = Main.rand.NextFloat(2f, 6f);
                        velocity *= speed;

                        Projectile.NewProjectile(null, player.Center, velocity, ModContent.ProjectileType<ChaliceHealth>(), 0, 0, player.whoAmI);
                    }
                }
            );

            definition.AddMindDownEffect(
                condition: (player) => GetInstance<OrnateChalice>(player).ChaliceCounter <= 0,
                effect: (player) =>
                {
                    player.AddBuff(ModContent.BuffType<Buffs.ChaliceMana>(), ModUtils.SecondsToTicks(5));
                    GetInstance<OrnateChalice>(player).ChaliceCounter = ModUtils.SecondsToTicks(40);

                    int projectileCount = Main.rand.Next(6, 10);
                    for (int i = 0; i < projectileCount; i++)
                    {
                        float randomAngle = Main.rand.NextFloat(0f, MathHelper.TwoPi);

                        Vector2 velocity = new Vector2((float)Math.Cos(randomAngle), (float)Math.Sin(randomAngle));
                        float speed = Main.rand.NextFloat(2f, 6f);
                        velocity *= speed;

                        Projectile.NewProjectile(null, player.Center, velocity, ModContent.ProjectileType<ChaliceMana>(), 0, 0, player.whoAmI);
                    }
                }
            );
        }
    }
}