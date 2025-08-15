using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Test;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Linq;

namespace OvermorrowMod.Content.Items.Archives.Weapons
{
    public class CarvingKnifeNew : ModItem
    {
        public override string Texture => AssetDirectory.ArchiveItems + "CarvingKnife";
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 102;
            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.HiddenAnimation;

            Item.knockBack = 2;
            Item.shootSpeed = 5f;
            Item.autoReuse = true;
            Item.damage = 13;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.shoot = ModContent.ProjectileType<TestSlashProjectile>();
        }

        public int ComboCount { get; private set; } = 0;
        int slashDirection = 1;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == type))
                return false;

            slashDirection = -slashDirection;
            if (ComboCount == 3)
                slashDirection = player.direction == 1 ? -1 : 1;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, slashDirection);

            int offhand = 1;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, slashDirection, offhand);

            ComboCount++;
            if (ComboCount > 3)
                ComboCount = 0;

            return false;
        }
    }
}