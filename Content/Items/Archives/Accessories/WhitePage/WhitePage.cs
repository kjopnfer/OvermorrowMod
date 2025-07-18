using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Items;
using OvermorrowMod.Common.Tooltips;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items.Accessories;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class WhitePage : OvermorrowAccessory, ITooltipEntities
    {
        public List<TooltipEntity> TooltipObjects()
        {
            var title = Language.GetTextValue(LocalizationPath.TooltipEntities + "StellarCorona" + ".DisplayName");
            var line = Language.GetTextValue(LocalizationPath.TooltipEntities + "StellarCorona" + ".Description.Line0");
            var line2 = Language.GetTextValue(LocalizationPath.TooltipEntities + "StellarCorona" + ".Description.Line1");
            var line3 = Language.GetTextValue(LocalizationPath.TooltipEntities + "StellarCorona" + ".Description.Line2");
            var line4 = Language.GetTextValue(LocalizationPath.TooltipEntities + "StellarCorona" + ".Description.Line3");

            return new List<TooltipEntity>() {
                new ProjectileTooltip(ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "StellarCorona").Value,
                    title,
                    [line, line2, line3, line4],
                    20f,
                    ProjectileTooltipType.Projectile,
                    DamageClass.Magic),
            };
        }

        public override string Texture => AssetDirectory.ArchiveItems + Name;
        protected override void SafeSetDefaults()
        {
            Item.width = 38;
            Item.height = 36;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, gold: 1, silver: 50, copper: 0);
        }

        protected override void SetAccessoryEffects(AccessoryDefinition definition)
        {
            definition.AddProjectileStrikeEffect(
                condition: (player, projectile, target, hit, damageDone) =>
                {
                    if (projectile.DamageType != DamageClass.Magic)
                        return false;

                    // 20% chance to apply Stellar Corona
                    if (Main.rand.NextFloat() > 0.20f)
                        return false;

                    int coronaType = ModContent.ProjectileType<StellarCorona>();
                    int prominenceType = ModContent.ProjectileType<StellarProminence>();

                    // Prevent it from spawning itself somehow
                    if (projectile.type == coronaType || projectile.type == prominenceType)
                        return false;

                    // Check if any active StellarCorona exists owned by this player
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];
                        if (proj.active && proj.type == coronaType && proj.owner == player.whoAmI)
                            return false; // Already has a StellarCorona active
                    }

                    return true;
                },
                effect: (player, projectile, target, hit, damageDone) =>
                {
                    int coronaType = ModContent.ProjectileType<StellarCorona>();

                    Projectile.NewProjectile(
                        projectile.GetSource_OnHit(target),
                        target.Center,
                        Vector2.Zero,
                        coronaType,
                        20,
                        0f,
                        player.whoAmI,
                        0f,
                        target.whoAmI
                    );
                }
            );
        }

        public static bool HasStellarCorona(NPC npc)
        {
            int coronaType = ModContent.ProjectileType<StellarCorona>();

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];

                if (proj.active && proj.type == coronaType && (int)proj.ai[1] == npc.whoAmI)
                {
                    return true; // NPC has a StellarCorona targeting it
                }
            }

            return false; // No matching StellarCorona found
        }
    }
}