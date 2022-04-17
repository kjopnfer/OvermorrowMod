using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.SpikeStaff
{
    public class SpikeStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stalagmite Staff");
            Tooltip.SetDefault("Holding down shoot creates spikes in a circle around you\nWhen released the spikes fire outwards\nYou can only have 9 spikes at a time\nDamage increases by the item damage for each spike");
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {

            Item.width = 54;
            Item.height = 54;
            Item.damage = 6;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 6;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item43;
            Item.noMelee = true;
            Item.useTime = 33;
            Item.channel = true;
            Item.useAnimation = 33;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.knockBack = 0;
            Item.shoot = ModContent.ProjectileType<Stalagmite>();
            Item.shootSpeed = 0f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Spike, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }
    }
}
