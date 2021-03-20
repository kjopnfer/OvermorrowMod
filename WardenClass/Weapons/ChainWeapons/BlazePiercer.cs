using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Piercing;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.ChainWeapons
{
    public class BlazePiercer : PiercingItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blaze Binder");
            Tooltip.SetDefault("Attacks have a chance to set enemies on fire\n[c/00FF00:{ Special Ability }]\n" +
                            "[c/800080:Right Click] to launch a chain dealing increased damage and inflicting an additional fire debuff\nConsumes 1 Soul Essence");
        }

        public override void SafeSetDefaults()
        {
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 14;
            item.useTime = 14;
            item.knockBack = 0f;
            item.width = 30;
            item.height = 10;
            item.damage = 12;
            item.shootSpeed = 18f;
            item.shoot = mod.ProjectileType("BlazePiercerProjectile");
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item71;
            item.noUseGraphic = true;

            soulGainChance = 3;
        }

        public override bool CanUseItem(Player player)
        {
            // Get the class info from the player
            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            if (player.altFunctionUse == 2 && modPlayer.soulResourceCurrent > 0)
            {
                item.useStyle = ItemUseStyleID.SwingThrow;
                item.useAnimation = 14;
                item.useTime = 14;
                item.knockBack = 0f;
                item.damage = 24;
                item.shootSpeed = 32f;
                item.shoot = mod.ProjectileType("BlazePiercerProjectileAlt");

                ConsumeSouls(1, player);
            }
            else
            {
                item.autoReuse = true;
                item.useStyle = ItemUseStyleID.SwingThrow;
                item.useTurn = true;
                item.useAnimation = 14;
                item.useTime = 14;
                item.damage = 12;
                item.shootSpeed = 18f + modPlayer.modifyShootSpeed();
                item.shoot = mod.ProjectileType("BlazePiercerProjectile");
            }

            return base.CanUseItem(player);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HellstoneBar, 24);
            recipe.AddIngredient(ItemID.Chain, 12);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}