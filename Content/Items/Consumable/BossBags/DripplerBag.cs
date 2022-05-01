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
            var source = player.GetSource_OpenItem(Type);
            player.TryGettingDevArmor(source);
            player.TryGettingDevArmor(source);

            int choice = Main.rand.Next(4);
            // Always drops one of:
            if (choice == 0) // Mage
            {
                player.QuickSpawnItem(source, ModContent.ItemType<ShatteredOrb>());
            }
            else if (choice == 1) // Warrior
            {
                player.QuickSpawnItem(source, ModContent.ItemType<BloodyTeeth>());
            }
            else if (choice == 2) // Ranger
            {
                player.QuickSpawnItem(source, ModContent.ItemType<DripplerEye>());
            }
            else if (choice == 3) // Summoner
            {
                player.QuickSpawnItem(source, ModContent.ItemType<SinisterBlood>());
            }

            if (Main.rand.NextBool(10)) // Trophy Dropchance
            {
                player.QuickSpawnItem(source, ModContent.ItemType<DripplerTrophy>());
            }

            if (Main.rand.NextBool(7))
            {
                player.QuickSpawnItem(source, ModContent.ItemType<DripMask>());
            }

            player.QuickSpawnItem(source, ModContent.ItemType<CancerInABottle>(), Main.rand.Next(6, 10));

            int necklaceChance = Main.rand.Next(5);
            if (necklaceChance == 0)
            {
                player.QuickSpawnItem(source, ItemID.SharkToothNecklace);
            }

            player.QuickSpawnItem(source, ModContent.ItemType<BloodyHeart>());
        }

        public override int BossBagNPC => ModContent.NPCType<DripplerBoss>();
    }
}