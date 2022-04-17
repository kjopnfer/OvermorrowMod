using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Items.Materials;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.GemStaves
{
    public class DiamondStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greater Diamond Staff");
            Tooltip.SetDefault("'MINE- DIA- MOOOOOONDS'");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Orange;
            Item.mana = 11;
            Item.UseSound = SoundID.Item43;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 20;
            Item.useTurn = false;
            Item.useAnimation = 14;
            Item.useTime = 14;
            Item.width = 48;
            Item.height = 48;
            Item.shoot = ModContent.ProjectileType<DiamondProj>();
            Item.shootSpeed = 14f;
            Item.knockBack = 6f;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.sellPrice(gold: 1, silver: 75);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 86f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DiamondStaff)
                .AddIngredient<ManaBar>(7)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}