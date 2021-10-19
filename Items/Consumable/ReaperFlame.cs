using Microsoft.Xna.Framework;
using OvermorrowMod.WardenClass.Weapons.Artifacts;
using OvermorrowMod.WardenClass.Weapons.ChainWeapons;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.Items.Consumable
{
    public class ReaperFlame : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaped Soul");
            ItemID.Sets.ItemNoGravity[item.type] = true;
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 9));
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 30;
            item.maxStack = 999;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override bool ItemSpace(Player player) => true;

        public override bool CanPickup(Player player)
        {
            // Can only pick up the item if the player is holding an Artifact or Chain Weapon
            if (player.HeldItem.modItem != null)
                player.GetModPlayer<WardenDamagePlayer>().UIToggled = player.HeldItem.modItem is Artifact || player.HeldItem.modItem is PiercingItem;

            return player.GetModPlayer<WardenDamagePlayer>().UIToggled;
        }

        public override bool OnPickup(Player player)
        {
            var modPlayer = player.GetModPlayer<WardenDamagePlayer>();
            if (modPlayer.soulPercentage < 100)
            {
                modPlayer.soulPercentage += 10;
            }

            return false;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(item.Center, new Color(187, 127, 128).ToVector3() * 0.55f * Main.essScale);
        }
    }
}