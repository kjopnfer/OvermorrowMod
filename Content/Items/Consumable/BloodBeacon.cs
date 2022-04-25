using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
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
            Item.width = 30;
            Item.height = 34;
            Item.rare = ItemRarityID.Green;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.maxStack = 20;
            Item.noMelee = true;
            Item.consumable = true;
            Item.autoReuse = false;
        }

        public override bool CanUseItem(Player player)
        {
            // Make sure that the boss doesn't already exist and player is in correct zone
            return !Main.dayTime && !Main.bloodMoon;
        }

        public override bool? UseItem(Player player)
        {
            if (!Main.dayTime)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position, 0);
                Main.bloodMoon = true;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText("The Blood Moon is rising...", 50, 255, 130);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("The Blood Moon is rising..."), new Color(50, 255, 130));
                }
                return true;
            }
            return false;
        }
    }
}