using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Summon;
using OvermorrowMod.Items.Materials;
using OvermorrowMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Summoner
{
    public class SkeletronSummStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Skull Staff");
            Tooltip.SetDefault("Summons a Skull Still to fight for you \nTo make it fire use the summon stick \nTakes a sentry slot \nDespawn other stills before using");
        }
        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Green;
            item.width = 32;
            item.height = 32;
            item.damage = 15;
            item.UseSound = SoundID.Item82;
            item.summon = true;
            item.noMelee = true;
            item.sentry = true;
            item.channel = true;
            item.useTime = 40;
            item.useAnimation = 40;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.autoReuse = false;
            item.buffType = ModContent.BuffType<SkullBuff>();
            item.knockBack = 0;
            item.shoot = ModContent.ProjectileType<SkeletronSumm>();
            item.shootSpeed = 0f;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                return player.ownedProjectileCounts[item.shoot] < 1;
            }
            else
            {
                return true;
            }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
            }
            else
            {
                return false;
            }
        }


        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Bone, 60);
            recipe.AddIngredient(ModContent.ItemType<SoulFire>());
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
