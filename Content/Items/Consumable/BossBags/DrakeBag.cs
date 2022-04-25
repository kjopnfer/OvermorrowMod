using OvermorrowMod.Content.Items.Accessories.Expert;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.Items.Placeable.Boss;
using OvermorrowMod.Content.Items.Weapons.Magic.BoltStream;
using OvermorrowMod.Content.Items.Weapons.Melee.StormTalon;
using OvermorrowMod.Content.Items.Weapons.Ranged.TempestGreatbow;
using OvermorrowMod.Content.Items.Weapons.Summoner.DrakeStaff;
using OvermorrowMod.Content.NPCs.Bosses.StormDrake;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Consumable.BossBags
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

            int choice = Main.rand.Next(4);
            // Always drops one of:
            if (choice == 0) // Mage
            {
                player.QuickSpawnItem(source, ModContent.ItemType<BoltStream>());
            }
            else if (choice == 1) // Warrior
            {
                player.QuickSpawnItem(source, ModContent.ItemType<StormTalon>());
            }
            else if (choice == 2) // Ranger
            {
                player.QuickSpawnItem(source, ModContent.ItemType<TempestGreatbow>());
            }
            else if (choice == 3) // Summoner
            {
                player.QuickSpawnItem(source, ModContent.ItemType<DrakeStaff>());
            }

            if (Main.rand.Next(10) == 0) // Trophy Dropchance
            {
                player.QuickSpawnItem(source, ModContent.ItemType<DrakeTrophy>());
            }

            player.QuickSpawnItem(source, ModContent.ItemType<StormScale>());

            player.QuickSpawnItem(source, ModContent.ItemType<StormCore>(), Main.rand.Next(10, 16));
        }

        public override int BossBagNPC => ModContent.NPCType</*StormDrake*/StormDrake>();
    }
}