using Microsoft.Xna.Framework;
using OvermorrowMod.Tiles.Block;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Materials
{
    public class FakeGem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Ore");
            Tooltip.SetDefault("Used to craft cursed items\n'Mimicking common ores, they are the bane of the unwary'");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 28;
            item.rare = ItemRarityID.Blue;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
        }
        
        public override void PostUpdate()
        {
            Lighting.AddLight(item.Center, 1f, 0f, 0f);
        }
    }
}