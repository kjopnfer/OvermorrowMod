using OvermorrowMod.Content.Items.Accessories.Expert;
using OvermorrowMod.Content.Items.Armor;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.Items.Placeable.Boss;
using OvermorrowMod.Content.Items.Weapons.PreHardmode.Magic;
using OvermorrowMod.Content.Items.Weapons.PreHardmode.Melee;
using OvermorrowMod.Content.Items.Weapons.PreHardmode.Summoner.GraniteStaff;
using OvermorrowMod.Content.NPCs.Bosses.GraniteMini;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.BossBags
{
    public class KnightBag : ModItem
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
            if (choice == 0) // Armor
            {
                player.QuickSpawnItem(ModContent.ItemType<GraniteHelmet>());
                player.QuickSpawnItem(ModContent.ItemType<GraniteBreastplate>());
                player.QuickSpawnItem(ModContent.ItemType<GraniteLeggings>());
            }
            else if (choice == 1) // Melee
            {
                player.QuickSpawnItem(ModContent.ItemType<GraniteSpear>());
            }
            else if (choice == 2) // Mage
            {
                player.QuickSpawnItem(ModContent.ItemType<GraniteBook>());
            }
            else if (choice == 3) // Summoner
            {
                player.QuickSpawnItem(ModContent.ItemType<GraniteStaff>());
            }

            if (Main.rand.Next(10) == 0) // Trophy Dropchance
            {
                player.QuickSpawnItem(ModContent.ItemType<DrakeTrophy>());
            }

            player.QuickSpawnItem(ModContent.ItemType<HeartStone>(), 2);

            player.QuickSpawnItem(ModContent.ItemType<GraniteShield>());
        }

        public override int BossBagNPC => ModContent.NPCType<AngryStone>();
    }
}