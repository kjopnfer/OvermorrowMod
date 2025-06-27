using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Weapons
{
    public class Chiaroscuro : ModItem
    {
        public override string Texture => AssetDirectory.ArchiveItems + Name;
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
            if (Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == type))
                return false;
            if (player.altFunctionUse == 2)
            {
                Main.NewText("spawn the cool");
            }

            Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.3f), type, damage, knockback, player.whoAmI);

            return false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool MeleePrefix()
        {
            return base.MeleePrefix();
        }
    }
}