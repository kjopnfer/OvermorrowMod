using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Tiles.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Materials
{
    public class EruditeOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Erudite Ore");
            Tooltip.SetDefault("Used to craft uncraftable items\n'A peculiar ore enriched with knowledge'");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.rare = ItemRarityID.Blue;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.createTile = ModContent.TileType<EruditeTile>();
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(item.Center, 0f, 0.5f, 0f);
        }
    }
}