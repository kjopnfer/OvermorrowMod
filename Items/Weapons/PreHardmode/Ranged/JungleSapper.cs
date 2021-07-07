using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using OvermorrowMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using OvermorrowMod.Items.Materials;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Ranged
{
    public class JungleSapper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Jungle Saper");
            Tooltip.SetDefault("Uses gel as ammo \nHas a chance to inflict fungal infection");
        }
        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Blue;
            item.width = 32;
            item.height = 32;
            item.damage = 12;
            item.ranged = true;
            item.noMelee = true;
            item.useTime = 10;
            item.useAnimation = 10;
            item.UseSound = SoundID.Item34;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.channel = true;
            item.knockBack = 0.1f;
            item.shoot = ProjectileID.MoonlordTurretLaser;
            item.shootSpeed = 15f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(183, 25);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }


        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            NPC.NewNPC((int)player.Center.X, (int)player.Center.Y, mod.NPCType("SpikeSling"));
            return true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
