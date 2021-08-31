using OvermorrowMod.Items.Accessories.Expert;
using OvermorrowMod.Items.Materials;
using OvermorrowMod.Items.Placeable.Boss;
using OvermorrowMod.Items.Weapons.PreHardmode.Magic;
using OvermorrowMod.Items.Weapons.PreHardmode.Melee;
using OvermorrowMod.Items.Weapons.PreHardmode.Ranged;
using OvermorrowMod.Items.Weapons.PreHardmode.Summoner;
using OvermorrowMod.NPCs.Bosses.TreeBoss;
using OvermorrowMod.WardenClass.Weapons.Artifacts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.BossBags
{
    public class TreeBag : ModItem
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
            item.height = 38;
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

            int choice = Main.rand.Next(5);
            // Always drops one of:
            if (choice == 0) // Warden
            {
                player.QuickSpawnItem(ModContent.ItemType<EarthCrystal>());
            }
            else if (choice == 1) // Mage
            {
                player.QuickSpawnItem(ModContent.ItemType<IorichStaff>());
            }
            else if (choice == 2) // Warrior
            {
                player.QuickSpawnItem(ModContent.ItemType<IorichHarvester>());
            }
            else if (choice == 3) // Ranger
            {
                player.QuickSpawnItem(ModContent.ItemType<IorichBow>());
            }
            else if (choice == 4) // Summoner
            {
                player.QuickSpawnItem(ModContent.ItemType<IorichWand>());
            }

            if (Main.rand.Next(10) == 0) // Trophy Dropchance
            {
                player.QuickSpawnItem(ModContent.ItemType<TreeTrophy>());
            }

            player.QuickSpawnItem(ModContent.ItemType<TreeNecklace>());

            player.QuickSpawnItem(ModContent.ItemType<SapStone>(), Main.rand.Next(1, 3));
        }

        public override int BossBagNPC => ModContent.NPCType<TreeBossP2>();
    }
}