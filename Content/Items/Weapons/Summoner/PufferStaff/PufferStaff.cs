using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.PufferStaff
{
    public class PufferStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pufferfish Staff");
            Tooltip.SetDefault("Summons a Pufferfish Still to fight for you \nTo make it fire use the summon stick \nTakes a sentry slot \nDespawn other stills before using");
        }
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Green;
            Item.width = 32;
            Item.height = 32;
            Item.damage = 8;
            Item.UseSound = SoundID.Item82;
            Item.DamageType = DamageClass.Summon;
            Item.noMelee = true;
            Item.channel = true;
            Item.sentry = true;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = false;
            Item.buffType = ModContent.BuffType<PufferBuff>();
            Item.knockBack = 0;
            Item.shoot = ModContent.ProjectileType<PufferFish>();
            Item.shootSpeed = 0f;
        }
        public override bool CanUseItem(Player player)
        {
            // Ensures no more than one spear can be thrown out, use this when using autoReuse
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Coral, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600, true);
            }
        }
    }
}
