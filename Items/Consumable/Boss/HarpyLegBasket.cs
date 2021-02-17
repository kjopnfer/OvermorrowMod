using Microsoft.Xna.Framework;
using OvermorrowMod.Items.Materials;
using OvermorrowMod.NPCs.Bosses.StormDrake;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Items.Consumable.Boss
{
    public class HarpyLegBasket : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Basket of Harpy Legs");
            Tooltip.SetDefault("Summons The Storm Drake");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 38;
            item.rare = ItemRarityID.Green;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.maxStack = 20;
            item.noMelee = true;
            item.consumable = true;
            item.autoReuse = false;
        }

        public override bool CanUseItem(Player player)
        {
            // Make sure that the boss doesn't already exist and player is in correct zone
            return !NPC.AnyNPCs(ModContent.NPCType<StormDrake>()) && player.ZoneSkyHeight;
        }

        public override bool UseItem(Player player)
        {
            if (player.ZoneSkyHeight)
            {
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<StormDrake>());
                Main.PlaySound(SoundID.Roar, player.position, 0);
                return true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<HarpyLeg>(), 4);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}