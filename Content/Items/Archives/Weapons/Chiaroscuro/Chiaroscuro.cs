using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Tooltips;
using OvermorrowMod.Content.Buffs;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Weapons
{
    public class Chiaroscuro : ModItem, ITooltipEntities, IWeaponClassification
    {
        public List<TooltipEntity> TooltipObjects()
        {
            var title = Language.GetTextValue(LocalizationPath.TooltipEntities + "ChiaroscuroShadow" + ".DisplayName");
            var line = Language.GetTextValue(LocalizationPath.TooltipEntities + "ChiaroscuroShadow" + ".Description.Line0");
            var line2 = Language.GetTextValue(LocalizationPath.TooltipEntities + "ChiaroscuroShadow" + ".Description.Line1");
            var line3 = Language.GetTextValue(LocalizationPath.TooltipEntities + "ChiaroscuroShadow" + ".Description.Line2");
            var line4 = Language.GetTextValue(LocalizationPath.TooltipEntities + "ChiaroscuroShadow" + ".Description.Line3");

            return new List<TooltipEntity>() {
                new ProjectileTooltip(ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "ChiaroscuroShadow").Value,
                    title,
                    [line, line2, line3, line4],
                    10f,
                    ProjectileTooltipType.Projectile,
                    DamageClass.Melee),
            };
        }

        public override string Texture => AssetDirectory.ArchiveItems + Name;
        public WeaponType WeaponType => WeaponType.Rapier;
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 102;
            Item.useAnimation = 120;
            Item.useTime = 120;
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.knockBack = 2;
            Item.shootSpeed = 5f;
            Item.autoReuse = true;
            Item.damage = 10;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.shoot = ModContent.ProjectileType<ChiaroscuroProjectile>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.projectile.Any(n => n.active && n.owner == player.whoAmI && (n.type == type || n.type == ModContent.ProjectileType<ChiaroscuroStance>())))
                return false;

            if (player.altFunctionUse == 2)
            {
                //if (!player.HasBuff(ModContent.BuffType<Buffs.ChiaroscuroStance>()))
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ChiaroscuroStance>(), damage, knockback, player.whoAmI);
            }
            else
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.3f), type, damage, knockback, player.whoAmI);

            return false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return !player.HasBuff(ModContent.BuffType<Buffs.ChiaroscuroStance>());
        }

        public override bool MeleePrefix()
        {
            return base.MeleePrefix();
        }
    }
}