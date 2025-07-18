using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Items;
using OvermorrowMod.Common.Tooltips;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Buffs;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items.Accessories;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class BlackPage : OvermorrowAccessory, ITooltipEntities
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

        protected override void SafeSetDefaults()
        {
            Item.width = 30;
            Item.height = 40;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, gold: 1, silver: 50, copper: 0);
        }
        protected override void SetAccessoryEffects(AccessoryDefinition definition)
        {
            definition.AddProjectileStrikeEffect(
                condition: (player, projectile, npc, hit, damageDone) =>
                {
                    // Must be a whip
                    if (projectile.DamageType != DamageClass.SummonMeleeSpeed)
                        return false;

                    // 20% chance
                    if (Main.rand.NextFloat() > 0.20f)
                        return false;

                    return true;
                },
                effect: (player, projectile, npc, hit, damageDone) =>
                {
                    var debuff = ModContent.BuffType<ShadowBrand>();
                    if (!npc.HasBuff(debuff))
                        Projectile.NewProjectile(projectile.GetSource_OnHit(npc), npc.Center, Vector2.Zero, ModContent.ProjectileType<ShadowGrasp>(), 16, 0f, player.whoAmI, 0f, npc.whoAmI);

                    npc.AddBuff(debuff, ModUtils.SecondsToTicks(10));

                    var modNPC = npc.GetGlobalNPC<GlobalNPCs>();
                    modNPC.ShadowBrandOwner = player;
                }
            );
        }
    }
}