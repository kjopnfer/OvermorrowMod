using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Consumable
{
    public class BloodBeacon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sanguine Beacon");
            Tooltip.SetDefault("Summons The Blood Moon");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 34;
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
            return !Main.dayTime && !Main.bloodMoon;
        }

        public override bool UseItem(Player player)
        {
            if (!Main.dayTime)
            {
                Main.PlaySound(SoundID.Roar, player.position, 0);
                Main.bloodMoon = true;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText("The Blood Moon is rising...", 50, 255, 130);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("The Blood Moon is rising..."), new Color(50, 255, 130));
                }
                return true;
            }
            return false;
        }
    }
}