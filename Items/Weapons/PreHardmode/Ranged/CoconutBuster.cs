using OvermorrowMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Ranged
{
    public class CoconutBuster : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Fires in spurts");
        }
        public override void SetDefaults()
        {
            item.damage = 7;
            item.ranged = true;
            item.width = 40;
            item.height = 25;
            item.useTime = 15;
            item.useAnimation = 45;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = 10000;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item40;
            item.shoot = ProjectileType<Coconut>();
            item.autoReuse = false;
            item.shootSpeed = 11f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
