using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Tooltips;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Interfaces;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class WhitePage : ModItem, ITooltipEntities
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
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 38;
            Item.height = 36;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, gold: 1, silver: 50, copper: 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AccessoryPlayer>().WhitePage = true;
        }

        public static void TryApplyStellarCorona(Projectile projectile, NPC target)
        {
            Player player = Main.player[projectile.owner];
            var accessoryPlayer = player.GetModPlayer<AccessoryPlayer>();

            if (!accessoryPlayer.WhitePage || projectile.DamageType != DamageClass.Magic)
                return;

            // 20% chance
            if (Main.rand.NextFloat() > 0.20f)
                return;

            int coronaType = ModContent.ProjectileType<StellarCorona>();
            int prominenceType = ModContent.ProjectileType<StellarProminence>();

            // Prevent it from spawning itself somehow
            if (projectile.type == coronaType || projectile.type == prominenceType)
                return;

            // Check if any active StellarCorona exists owned by this player
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.type == coronaType && proj.owner == player.whoAmI)
                    return; // Already has a StellarCorona active
            }

            Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, coronaType, 20, 0f, player.whoAmI, 0f, target.whoAmI);
        }
    }
}