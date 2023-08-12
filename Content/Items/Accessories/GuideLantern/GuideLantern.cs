using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories.GuideLantern
{
    public class GuideLantern : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Guide's Lantern");
            // Tooltip.SetDefault("Automatically provides light when not performing other actions");
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 10;
            Item.height = 16;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 1, 0, 0);
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().GuideLantern = true;

            if (GuideLantern_Held.IsPerformingAction(player) || player.wet) return;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<GuideLantern_Held>()] < 1)   
                Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<GuideLantern_Held>(), 0, 0, player.whoAmI);
            
        }
    }
}