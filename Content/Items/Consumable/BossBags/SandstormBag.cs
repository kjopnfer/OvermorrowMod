using OvermorrowMod.Content.Items.Accessories.Expert;
using OvermorrowMod.Content.Items.Placeable.Boss;
using OvermorrowMod.Content.Items.Weapons.Magic.SandStaff;
using OvermorrowMod.Content.Items.Weapons.Melee.SandSpinner;
using OvermorrowMod.Content.Items.Weapons.Ranged.SandThrower;
using OvermorrowMod.Content.Items.Weapons.Summoner.DustStaff;
using OvermorrowMod.Content.NPCs.Bosses.SandstormBoss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Consumable.BossBags
{
    public class SandstormBag : ModItem
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
            item.width = 32;
            item.height = 32;
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
                player.QuickSpawnItem(ModContent.ItemType<SandStaff>());
            }
            else if (choice == 1) // Warrior
            {
                player.QuickSpawnItem(ModContent.ItemType<SandstormSpinner>());
            }
            else if (choice == 2) // Ranger
            {
                player.QuickSpawnItem(ModContent.ItemType<SandThrower>());
            }
            else if (choice == 3) // Summoner
            {
                player.QuickSpawnItem(ModContent.ItemType<DustStaff>());
            }

            if (Main.rand.Next(10) == 0) // Trophy Dropchance
            {
                player.QuickSpawnItem(ModContent.ItemType<SandTrophy>());
            }

            player.QuickSpawnItem(ModContent.ItemType<ArmBracer>());
        }

        public override int BossBagNPC => ModContent.NPCType<SandstormBoss>();
    }
}