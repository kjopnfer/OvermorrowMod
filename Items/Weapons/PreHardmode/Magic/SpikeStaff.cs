using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class SpikeStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stalagmite Staff");
            Tooltip.SetDefault("Holding down shoot creates spikes in a circle around you\nWhen released the spikes fire outwards\nYou can only have 9 spikes at a time\nDamage increases by the item damage for each spike");
            Item.staff[item.type] = true;
        }
        public override void SetDefaults()
        {

            item.width = 54;
            item.height = 54;
            item.damage = 6;
            item.magic = true;
            item.mana = 6;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item43;
            item.noMelee = true;
            item.useTime = 33;
            item.channel = true;
            item.useAnimation = 33;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.knockBack = 0;
            item.shoot = ModContent.ProjectileType<CircleMagic>();
            item.shootSpeed = 0f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Spike, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }
    }
}
