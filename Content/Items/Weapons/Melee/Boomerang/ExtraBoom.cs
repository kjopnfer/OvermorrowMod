using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.Boomerang
{
    public class ExtraBoom : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("ExtraBoom-erang");
            Tooltip.SetDefault("Get it? EXTRA boom?");
        }
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.damage = 7;
            Item.DamageType = DamageClass.Melee;
            Item.width = 48;
            Item.height = 40;
            Item.useTime = 34;
            Item.useAnimation = 34;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.scale = 0.8f;
            Item.noMelee = true;
            Item.knockBack = 0.1f;
            Item.UseSound = SoundID.Item17;
            Item.shoot = ModContent.ProjectileType<PROJExtraBoom>();;
            Item.shootSpeed = 7f;
            Item.value = Item.sellPrice(0, 0, 15, 0);
        }
        public override bool CanUseItem(Player player) 
        {
            {
                return player.ownedProjectileCounts[ModContent.ProjectileType<PROJExtraBoom>()] < 1;
            }
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
