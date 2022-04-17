using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Summon;
using OvermorrowMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.SkeletonStaff
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
            Item.rare = ItemRarityID.Green;
            Item.width = 32;
            Item.height = 32;
            Item.damage = 15;
            Item.UseSound = SoundID.Item82;
            Item.DamageType = DamageClass.Summon;
            Item.noMelee = true;
            Item.sentry = true;
            Item.channel = true;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = false;
            Item.buffType = ModContent.BuffType<SkullBuff>();
            Item.knockBack = 0;
            Item.shoot = ModContent.ProjectileType<SkeletronSumm>();
            Item.shootSpeed = 0f;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                return player.ownedProjectileCounts[Item.shoot] < 1;
            }
            else
            {
                return true;
            }
        }

        public override bool CanShoot(Player player)
        {
            return player.altFunctionUse == 2 && base.CanShoot(player);
        }


        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600, true);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bone, 60)
                .AddIngredient<SoulFire>()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
