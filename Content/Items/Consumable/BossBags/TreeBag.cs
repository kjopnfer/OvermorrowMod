using OvermorrowMod.Content.Items.Accessories.Expert;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.Items.Placeable.Boss;
using OvermorrowMod.Content.Items.Weapons.Ranged.IorichBow;
using OvermorrowMod.Content.Items.Weapons.Melee.IorichHarvester;
using OvermorrowMod.Content.Items.Weapons.Summoner.IorichWand;
using OvermorrowMod.Content.Items.Weapons.Magic.IorichStaff;
using OvermorrowMod.Content.NPCs.Bosses.TreeBoss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Consumable.BossBags
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

            int choice = Main.rand.Next(4);
            // Always drops one of:
            if (choice == 0) // Mage
            {
                player.QuickSpawnItem(ModContent.ItemType<IorichStaff>());
            }
            else if (choice == 1) // Warrior
            {
                player.QuickSpawnItem(ModContent.ItemType<IorichHarvester>());
            }
            else if (choice == 2) // Ranger
            {
                player.QuickSpawnItem(ModContent.ItemType<IorichBow>());
            }
            else if (choice == 3) // Summoner
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