using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.TankRemote
{
    public class TankStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tank Remote");
            Tooltip.SetDefault("Summons a tank to fight for you");
        }
        public override void SetDefaults()
        {

            Item.width = 32;
            Item.height = 32;
            Item.damage = 25;
            Item.DamageType = DamageClass.Summon;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item82;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.knockBack = 0;
            Item.buffType = ModContent.BuffType<TankBuff>();
            Item.shoot = ModContent.ProjectileType<SkeleTank>();
            Item.shootSpeed = 1f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Spike, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }


        public override void UseStyle(Player player, Rectangle rect)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600, true);
            }
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.
            position = Main.MouseWorld;
        }
    }
}