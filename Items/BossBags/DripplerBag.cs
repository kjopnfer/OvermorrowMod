using OvermorrowMod.Items.Accessories;
using OvermorrowMod.Items.Accessories.Expert;
using OvermorrowMod.Items.Placeable.Boss;
using OvermorrowMod.NPCs.Bosses.DripplerBoss;
using OvermorrowMod.WardenClass.Weapons.Artifacts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.BossBags
{
    public class DripplerBag : ModItem
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

            int choice = Main.rand.Next(5);
            // Always drops one of:
            if (choice == 0) // Warden
            {
                player.QuickSpawnItem(ModContent.ItemType<BloodyAntikythera>());
            }
            else if (choice == 1) // Mage
            {
                player.QuickSpawnItem(ModContent.ItemType<ShatteredOrb>());
            }
            else if (choice == 2) // Warrior
            {
                player.QuickSpawnItem(ModContent.ItemType<BloodyTeeth>());
            }
            else if (choice == 3) // Ranger
            {
                player.QuickSpawnItem(ModContent.ItemType<DripplerEye>());
            }
            else if (choice == 4) // Summoner
            {
                player.QuickSpawnItem(ModContent.ItemType<SinisterBlood>());
            }

            if (Main.rand.Next(10) == 0) // Trophy Dropchance
            {
                player.QuickSpawnItem(ModContent.ItemType<DripplerTrophy>());
            }

            int necklaceChance = Main.rand.Next(5);
            if (necklaceChance == 0)
            {
                player.QuickSpawnItem(ItemID.SharkToothNecklace);
            }

            player.QuickSpawnItem(ModContent.ItemType<BloodyHeart>());
        }

        public override int BossBagNPC => ModContent.NPCType<DripplerBoss>();
    }
}