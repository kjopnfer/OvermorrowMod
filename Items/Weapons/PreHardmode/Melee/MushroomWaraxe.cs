using Microsoft.Xna.Framework;
using OvermorrowMod.Items.Materials;
using OvermorrowMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Buffs.Debuffs;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Melee
{
    public class MushroomWaraxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mushroom Waraxe");
            Tooltip.SetDefault("Has a one in five chance to inflict fungal infection");
        }

        public override void SetDefaults()
        {
            item.scale = 1.3f;
            item.rare = ItemRarityID.Blue;
            item.UseSound = SoundID.Item7;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.damage = 19;
            item.useTime = 25;
            item.useAnimation = 25;
            item.knockBack = 3f;
            item.melee = true;
            item.autoReuse = true;
            item.value = Item.sellPrice(gold: 1);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(183, 35);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

		public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit) 
		{
            if (Main.rand.Next(5) == 3)
            {
			    target.AddBuff(ModContent.BuffType<FungalInfection>(), 200);
            }
		}

		public override void MeleeEffects(Player player, Rectangle hitbox) 
        {
			if (Main.rand.NextBool(10)) 
            {
				int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 41);
			}
		}
	}
}


