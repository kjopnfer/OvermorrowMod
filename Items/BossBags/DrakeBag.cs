using OvermorrowMod.Items.Accessories.Expert;
using OvermorrowMod.Items.Placeable.Boss;
using OvermorrowMod.Items.Weapons.PreHardmode.Magic;
using OvermorrowMod.Items.Weapons.PreHardmode.Melee;
using OvermorrowMod.Items.Weapons.PreHardmode.Ranged;
using OvermorrowMod.Items.Weapons.PreHardmode.Summoner;
using OvermorrowMod.NPCs.Bosses.StormDrake;
using OvermorrowMod.WardenClass.Weapons.ChainWeapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.BossBags
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
                player.QuickSpawnItem(ModContent.ItemType<LightningPiercer>());
            }
            else if (choice == 1) // Mage
            {
                player.QuickSpawnItem(ModContent.ItemType<BoltStream>());
            }
            else if (choice == 2) // Warrior
            {
                player.QuickSpawnItem(ModContent.ItemType<StormTalon>());
            }
            else if (choice == 3) // Ranger
            {
                player.QuickSpawnItem(ModContent.ItemType<TempestGreatbow>());
            }
            else if (choice == 4) // Summoner
            {
                player.QuickSpawnItem(ModContent.ItemType<DrakeStaff>());
            }

            if (Main.rand.Next(10) == 0) // Trophy Dropchance
            {
                player.QuickSpawnItem(ModContent.ItemType<DrakeTrophy>());
            }

            player.QuickSpawnItem(ModContent.ItemType<StormScale>());
        }

        public override int BossBagNPC => ModContent.NPCType</*StormDrake*/StormDrake2>();
    }
}