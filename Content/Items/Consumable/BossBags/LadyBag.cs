using OvermorrowMod.Content.Items.Accessories.Expert;
using OvermorrowMod.Content.Items.Armor.Marble;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.Items.Placeable.Boss;
using OvermorrowMod.Content.Items.Weapons.Magic.MarbleBook;
using OvermorrowMod.Content.Items.Weapons.Ranged.MarbleBow;
using OvermorrowMod.Content.NPCs.Bosses.Apollus;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Consumable.BossBags
{
    public class LadyBag : ModItem
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
            Item.width = 36;
            Item.height = 36;
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

            int choice = Main.rand.Next(3);
            if (choice == 0) // Armor
            {
                player.QuickSpawnItem(source, ModContent.ItemType<MarblePlate>());
                player.QuickSpawnItem(source, ModContent.ItemType<MarbleHelm>());
                player.QuickSpawnItem(source, ModContent.ItemType<MarbleLegs>());
            }
            else if (choice == 1) // Magic
            {
                player.QuickSpawnItem(source, ModContent.ItemType<MarbleBook>());
            }
            else if (choice == 2) // Mage
            {
                player.QuickSpawnItem(source, ModContent.ItemType<MarbleBow>());
            }


            if (Main.rand.Next(10) == 0) // Trophy Dropchance
            {
                player.QuickSpawnItem(source, ModContent.ItemType<DrakeTrophy>());
            }

            player.QuickSpawnItem(source, ModContent.ItemType<HeartStone>(), 2);

            player.QuickSpawnItem(source, ModContent.ItemType<ArtemisAmulet>());
        }

        public override int BossBagNPC => ModContent.NPCType<ApollusBoss>();
    }
}