using OvermorrowMod.Content.Items.Accessories;
using OvermorrowMod.Content.Items.Accessories.Expert;
using OvermorrowMod.Content.Items.Armor.Masks;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.Items.Placeable.Boss;
using OvermorrowMod.Content.NPCs.Bosses.DripplerBoss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Consumable.BossBags
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

            int choice = Main.rand.Next(4);
            // Always drops one of:
            if (choice == 0) // Mage
            {
                player.QuickSpawnItem(ModContent.ItemType<ShatteredOrb>());
            }
            else if (choice == 1) // Warrior
            {
                player.QuickSpawnItem(ModContent.ItemType<BloodyTeeth>());
            }
            else if (choice == 2) // Ranger
            {
                player.QuickSpawnItem(ModContent.ItemType<DripplerEye>());
            }
            else if (choice == 3) // Summoner
            {
                player.QuickSpawnItem(ModContent.ItemType<SinisterBlood>());
            }

            if (Main.rand.Next(10) == 0) // Trophy Dropchance
            {
                player.QuickSpawnItem(ModContent.ItemType<DripplerTrophy>());
            }

            if (Main.rand.Next(7) == 0)
            {
                player.QuickSpawnItem(ModContent.ItemType<DripMask>());
            }

            player.QuickSpawnItem(ModContent.ItemType<CancerInABottle>(), Main.rand.Next(6, 10));

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