using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.DemonEye
{
    public class DemonEyeStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demon Eye Staff");
            Tooltip.SetDefault("Summons a Demon Eye to fight for you");
        }
        public override void SetDefaults()
        {

            Item.width = 32;
            Item.height = 32;
            Item.damage = 10;
            Item.DamageType = DamageClass.Summon;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item82;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.knockBack = 0;
            Item.buffType = ModContent.BuffType<DemEyeBuff>();
            Item.shoot = ModContent.ProjectileType<EyeSummon>();
            Item.shootSpeed = 11f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Lens, 10)
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

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.
            position = Main.MouseWorld;
        }

    }
}