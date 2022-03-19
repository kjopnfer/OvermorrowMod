using OvermorrowMod.Content.Items.Accessories.Expert;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.Items.Placeable.Boss;
using OvermorrowMod.Content.Items.Weapons.Magic;
using OvermorrowMod.Content.Items.Weapons.Melee;
using OvermorrowMod.Content.Items.Weapons.Ranged;
using OvermorrowMod.Content.Items.Weapons.Summoner;
using OvermorrowMod.Content.NPCs.Bosses.StormDrake;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.BossBags
{
    public class DrakeBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Treasure Bag");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 36;
            item.height = 36;
            item.rare = ItemRarityID.Expert;
            item.expert = true;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();
            player.TryGettingDevArmor();

            int choice = Main.rand.Next(4);
            // Always drops one of:
            if (choice == 0) // Mage
            {
                player.QuickSpawnItem(ModContent.ItemType<BoltStream>());
            }
            else if (choice == 1) // Warrior
            {
                player.QuickSpawnItem(ModContent.ItemType<StormTalon>());
            }
            else if (choice == 2) // Ranger
            {
                player.QuickSpawnItem(ModContent.ItemType<TempestGreatbow>());
            }
            else if (choice == 3) // Summoner
            {
                player.QuickSpawnItem(ModContent.ItemType<DrakeStaff>());
            }

            if (Main.rand.Next(10) == 0) // Trophy Dropchance
            {
                player.QuickSpawnItem(ModContent.ItemType<DrakeTrophy>());
            }

            player.QuickSpawnItem(ModContent.ItemType<StormScale>());

            player.QuickSpawnItem(ModContent.ItemType<StormCore>(), Main.rand.Next(10, 16));
        }

        public override int BossBagNPC => ModContent.NPCType</*StormDrake*/StormDrake>();
    }
}