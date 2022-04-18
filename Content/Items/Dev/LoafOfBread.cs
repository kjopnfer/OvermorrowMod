using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Dev
{
    public class LoafOfBread : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Loaf of Bread");
            Tooltip.SetDefault("Is it purring?");
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.UnluckyYarn);

            Item.shoot = ModContent.ProjectileType<LokiPet>();
            Item.buffType = ModContent.BuffType<LokiPetBuff>();
        }

        public override void UseStyle(Player player, Rectangle rect)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600, true);
            }
        }

    }
}
