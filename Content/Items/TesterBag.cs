using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Items.Accessories;
using OvermorrowMod.Content.Items.Accessories.BearTrap;
using OvermorrowMod.Content.Items.Accessories.CapturedMirage;
using OvermorrowMod.Content.Items.Accessories.PracticeTarget;
using OvermorrowMod.Content.Items.Ammo;
using OvermorrowMod.Content.Items.Weapons.Ranged;
using OvermorrowMod.Content.Items.Weapons.Ranged.GraniteLauncher;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items
{
    public class TesterBag : ModItem
    {
        public override bool CanRightClick() => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tester's Bag of Shit");
            Tooltip.SetDefault("Right Click to obtain literally everything");
        }

        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Purple;
            Item.expert = true;
        }

        public override void RightClick(Player player)
        {
            player.QuickSpawnItem(null, ItemID.CopperPickaxe);

            player.QuickSpawnItem(null, ItemID.CowboyHat);
            player.QuickSpawnItem(null, ItemID.CowboyJacket);
            player.QuickSpawnItem(null, ItemID.CowboyPants);

            player.QuickSpawnItem(null, ModContent.ItemType<BearTrap>());
            player.QuickSpawnItem(null, ModContent.ItemType<CapturedMirage>());
            player.QuickSpawnItem(null, ModContent.ItemType<PracticeTarget>());
            player.QuickSpawnItem(null, ModContent.ItemType<SnakeBite>());
            player.QuickSpawnItem(null, ModContent.ItemType<AnglerTooth>());
            player.QuickSpawnItem(null, ModContent.ItemType<SerpentTooth>());
            player.QuickSpawnItem(null, ModContent.ItemType<PredatorsTalisman>());
            player.QuickSpawnItem(null, ModContent.ItemType<ImbuementPouch>());
            player.QuickSpawnItem(null, ModContent.ItemType<SickeningSnack>());

            player.QuickSpawnItem(null, ModContent.ItemType<ObsidianArrow>(), 9999);
            player.QuickSpawnItem(null, ItemID.MusketBall, 999);
            player.QuickSpawnItem(null, ItemID.MusketBall, 999);
            player.QuickSpawnItem(null, ItemID.MusketBall, 999);

            player.QuickSpawnItem(null, ItemID.GoldBow);

            player.QuickSpawnItem(null, ItemID.Boomstick);
            player.QuickSpawnItem(null, ModContent.ItemType<Chicago>());
            player.QuickSpawnItem(null, ModContent.ItemType<Farlander>());
            player.QuickSpawnItem(null, ModContent.ItemType<GraniteLauncher>());
            player.QuickSpawnItem(null, ItemID.Handgun);
            player.QuickSpawnItem(null, ItemID.Minishark);
            player.QuickSpawnItem(null, ItemID.Musket);
            player.QuickSpawnItem(null, ItemID.PhoenixBlaster);
            player.QuickSpawnItem(null, ItemID.QuadBarrelShotgun);
            player.QuickSpawnItem(null, ItemID.Revolver);
            player.QuickSpawnItem(null, ItemID.TheUndertaker);
            player.QuickSpawnItem(null, ModContent.ItemType<WildEye>());
        }
    }
}