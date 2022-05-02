using OvermorrowMod.Content.Items.Accessories.Expert;
using OvermorrowMod.Content.Items.Armor.Granite;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.Items.Placeable.Boss;
using OvermorrowMod.Content.Items.Weapons.Magic.GraniteBook;
using OvermorrowMod.Content.Items.Weapons.Melee.GraniteSpear;
using OvermorrowMod.Content.Items.Weapons.Summoner.GraniteStaff;
using OvermorrowMod.Content.NPCs.Bosses.GraniteMini;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Consumable.BossBags
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
            if (choice == 0) // Armor
            {
                player.QuickSpawnItem(source, ModContent.ItemType<GraniteHelmet>());
                player.QuickSpawnItem(source, ModContent.ItemType<GraniteBreastplate>());
                player.QuickSpawnItem(source, ModContent.ItemType<GraniteLeggings>());
            }
            else if (choice == 1) // Melee
            {
                player.QuickSpawnItem(source, ModContent.ItemType<GraniteSpear>());
            }
            else if (choice == 2) // Mage
            {
                player.QuickSpawnItem(source, ModContent.ItemType<GraniteBook>());
            }
            else if (choice == 3) // Summoner
            {
                player.QuickSpawnItem(source, ModContent.ItemType<GraniteStaff>());
            }

            if (Main.rand.NextBool(10)) // Trophy Dropchance
            {
                player.QuickSpawnItem(source, ModContent.ItemType<DrakeTrophy>());
            }

            player.QuickSpawnItem(source, ModContent.ItemType<HeartStone>(), 2);

            player.QuickSpawnItem(source, ModContent.ItemType<GraniteShield>());
        }

        public override int BossBagNPC => ModContent.NPCType<AngryStone>();
    }
}