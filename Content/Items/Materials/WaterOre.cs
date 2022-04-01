using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Tiles.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Materials
{
    public class WaterOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lacusite Ore");
            Tooltip.SetDefault("'Drips with water'");
        }

        public override void SetDefaults()
        {
            item.width = 14;
            item.height = 14;
            item.rare = ItemRarityID.Green;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.createTile = ModContent.TileType<WaterCaveOre>();
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(item.Center, Color.White.ToVector3() * 0.55f * Main.essScale);
        }
    }
}