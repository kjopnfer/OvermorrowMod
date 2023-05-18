using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Consumable
{
    public abstract class AdventurerBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Guide's Rucksack");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.consumable = true;
        }

        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            GetItem(player);
            GetValuableItem(player);
            GetUsefulItem(player);
        }

        private void GetItem(Player player)
        {
            switch (Main.rand.Next(3))
            {
                case 0:
                    player.QuickSpawnItem(new EntitySource_Misc("LootBag"), ItemID.RopeCoil, 10);
                    player.QuickSpawnItem(new EntitySource_Misc("LootBag"), ItemID.LesserHealingPotion, 2);
                    break;
                case 1:
                    player.QuickSpawnItem(new EntitySource_Misc("LootBag"), ItemID.Apple, 2);
                    break;
                case 2:
                    player.QuickSpawnItem(new EntitySource_Misc("LootBag"), ItemID.Torch, 20);
                    player.QuickSpawnItem(new EntitySource_Misc("LootBag"), ItemID.SilverCoin, 25);
                    break;
                default:
                    player.QuickSpawnItem(new EntitySource_Misc("LootBag"), ItemID.Apple, 2);
                    break;
            }
        }

        private void GetValuableItem(Player player)
        {
            if (Main.rand.NextBool(4))
            {
                player.QuickSpawnItem(new EntitySource_Misc("LootBag"), ItemID.AdhesiveBandage);
            }
            else
            {
                switch (Main.rand.Next(4))
                {
                    case 0:
                        player.QuickSpawnItem(new EntitySource_Misc("LootBag"), ItemID.GoldCoin, 2);
                        break;
                    case 1:
                        player.QuickSpawnItem(new EntitySource_Misc("LootBag"), ItemID.Ruby, 6);
                        break;
                    case 2:
                        player.QuickSpawnItem(new EntitySource_Misc("LootBag"), ItemID.GoldCoin, 2);
                        break;
                    case 3:
                        player.QuickSpawnItem(new EntitySource_Misc("LootBag"), ItemID.GoldCoin, 2);
                        break;
                    default:
                        player.QuickSpawnItem(new EntitySource_Misc("LootBag"), ItemID.Apple, 2);
                        break;
                }
            }    
        }

        private void GetUsefulItem(Player player)
        {
            switch (Main.rand.Next(3))
            {
                case 0:
                    player.QuickSpawnItem(new EntitySource_Misc("LootBag"), ItemID.Aglet);
                    break;
                case 1:
                    player.QuickSpawnItem(new EntitySource_Misc("LootBag"), ItemID.ClimbingClaws);
                    break;
                case 2:
                    player.QuickSpawnItem(new EntitySource_Misc("LootBag"), ItemID.Umbrella);
                    break;
                default:
                    player.QuickSpawnItem(new EntitySource_Misc("LootBag"), ItemID.Aglet);
                    break;
            }
        }
    }
    
    public class GuideBag : AdventurerBag
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Guide's Rucksack");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 32;
            Item.rare = ItemRarityID.Green;
            Item.consumable = true;
        }
    }
}