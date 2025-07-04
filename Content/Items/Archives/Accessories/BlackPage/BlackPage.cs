using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Tooltips;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Buffs;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Interfaces;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class BlackPage : ModItem, ITooltipEntities
    {
        public List<TooltipEntity> TooltipObjects()
        {
            string GetText(string key) => Language.GetTextValue(LocalizationPath.TooltipEntities + key);

            var blackEchoesTitle = GetText("BlackEchoes.DisplayName");
            var blackEchoesLines = new[]
            {
                GetText("BlackEchoes.Description.Line0"),
                GetText("BlackEchoes.Description.Line1"),
                GetText("BlackEchoes.Description.Line2")
            };

            var shadowbrandTitle = GetText("Shadowbrand.DisplayName");
            var shadowbrandLines = new[]
            {
                GetText("Shadowbrand.Description.Line0"),
                GetText("Shadowbrand.Description.Line1")
            };

            var shadowbrandTexture = ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "Shadowbrand").Value;
            var blackEchoesTexture = ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "BlackEchoes").Value;

            return new List<TooltipEntity>
            {
                new BuffTooltip(shadowbrandTexture, shadowbrandTitle, shadowbrandLines, 10, BuffTooltipType.Debuff),
                new ProjectileTooltip(blackEchoesTexture, blackEchoesTitle, blackEchoesLines, 16f, ProjectileTooltipType.Projectile, DamageClass.Summon)
            };
        }

        public override string Texture => AssetDirectory.ArchiveItems + Name;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 30;
            Item.height = 40;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, gold: 1, silver: 50, copper: 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AccessoryPlayer>().BlackPage = true;
        }

        public static void TryApplyShadowBrand(Projectile projectile, NPC target)
        {
            Player player = Main.player[projectile.owner];
            var accessoryPlayer = player.GetModPlayer<AccessoryPlayer>();

            if (!accessoryPlayer.BlackPage || projectile.DamageType != DamageClass.SummonMeleeSpeed)
                return;

            // 20% chance
            //if (Main.rand.NextFloat() > 0.20f)
            //    return;

            var debuff = ModContent.BuffType<ShadowBrand>();
            //if (!target.HasBuff(debuff))
            Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<ShadowGrasp>(), 16, 0f, player.whoAmI, 0f, target.whoAmI);

            target.AddBuff(debuff, ModUtils.SecondsToTicks(10));
        }
    }
}