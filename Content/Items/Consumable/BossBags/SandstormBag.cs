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
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void OpenBossBag(Player player)
        {
            var source = player.GetItemSource_OpenItem(Type);
            player.TryGettingDevArmor(source);
            player.TryGettingDevArmor(source);

            int choice = Main.rand.Next(4);
            // Always drops one of:
            if (choice == 0) // Mage
            {
                player.QuickSpawnItem(source, ModContent.ItemType<SandStaff>());
            }
            else if (choice == 1) // Warrior
            {
                player.QuickSpawnItem(source, ModContent.ItemType<SandstormSpinner>());
            }
            else if (choice == 2) // Ranger
            {
                player.QuickSpawnItem(source, ModContent.ItemType<SandThrower>());
            }
            else if (choice == 3) // Summoner
            {
                player.QuickSpawnItem(source, ModContent.ItemType<DustStaff>());
            }

            if (Main.rand.Next(10) == 0) // Trophy Dropchance
            {
                player.QuickSpawnItem(source, ModContent.ItemType<SandTrophy>());
            }

            player.QuickSpawnItem(source, ModContent.ItemType<ArmBracer>());
        }

        public override int BossBagNPC => ModContent.NPCType<SandstormBoss>();
    }
}