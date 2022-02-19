using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.Items.Dev
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
            item.CloneDefaults(ItemID.UnluckyYarn);

            item.shoot = ModContent.ProjectileType<LokiPet>();
            item.buffType = ModContent.BuffType<LokiPetBuff>();
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }

    }
}
