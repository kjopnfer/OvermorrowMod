using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace OvermorrowMod.Content.Items.Consumable
{
    public class ReforgeStone : ModItem
    {
        public override bool CanRightClick() => true;
        public override bool CanStack(Item item2) => false;
        public override bool CanUseItem(Player player) => false;
        public override bool ConsumeItem(Player player) => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Melee Reforge Stone");
            Tooltip.SetDefault("Used to apply modifiers to Melee weapons");
            ItemID.Sets.CanGetPrefixes[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 22;
            Item.damage = 10;
            Item.knockBack = 5f;
            Item.crit = 32;
            Item.useAnimation = Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 1f;
            Item.consumable = true;
            Item.rare = ItemRarityID.Green;
            Item.DamageType = DamageClass.Melee;
        }

        public static int[] meleePrefixes = new int[] {
            PrefixID.Savage, PrefixID.Legendary, PrefixID.Murderous, PrefixID.Deadly,
            PrefixID.Godly, PrefixID.Demonic, PrefixID.Superior, PrefixID.Unpleasant };

        public static int[] knockbackPrefixes = new int[] {
            PrefixID.Savage, PrefixID.Legendary, PrefixID.Godly, PrefixID.Superior, PrefixID.Unpleasant };

        public static int[] critPrefixes = new int[] {
            PrefixID.Legendary, PrefixID.Murderous, PrefixID.Godly, PrefixID.Demonic, PrefixID.Superior };

        // Bro I don't even know why I need to do this, Prefix(-1) just doesn't wanna work half the time
        public static int GetRandomPrefix()
        {
            return meleePrefixes[Main.rand.Next(0, meleePrefixes.Length)];
        }

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            return GetRandomPrefix();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int lines = 0; lines < tooltips.Count; lines++)
            {
                if (tooltips[lines].Name == "Damage") tooltips.RemoveAt(lines);
                if (tooltips[lines].Name == "CritChance") tooltips.RemoveAt(lines);
                if (tooltips[lines].Name == "Speed") tooltips.RemoveAt(lines);
                if (tooltips[lines].Name == "Knockback") tooltips.RemoveAt(lines);
            }

            base.ModifyTooltips(tooltips);
        }

        /*private static readonly int[] unwantedPrefixes = new int[] { 
            PrefixID.Large, PrefixID.Dangerous, PrefixID.Sharp, PrefixID.Pointy, PrefixID.Tiny, PrefixID.Terrible, PrefixID.Small,
            PrefixID.Dull, PrefixID.Unhappy, PrefixID.Bulky, PrefixID.Shameful, PrefixID.Heavy, PrefixID.Light, PrefixID.Keen, 
            PrefixID.Forceful, PrefixID.Broken, PrefixID.Damaged, PrefixID.Shoddy, PrefixID.Hurtful, PrefixID.Strong, PrefixID.Weak, 
            PrefixID.Zealous, PrefixID.Quick, PrefixID.Agile, PrefixID.Nimble, PrefixID.Slow, PrefixID.Sluggish, PrefixID.Lazy, 
            PrefixID.Annoying, PrefixID.Nasty };

        public override bool AllowPrefix(int pre)
        {
            if (Array.IndexOf(unwantedPrefixes, pre) > -1)
            {
                return false;
            }

            return true;
        }*/

        public override bool? PrefixChance(int pre, UnifiedRandom rand)
        {
            if (pre == -3) return false;
            //if (pre == -1) return true;

            return base.PrefixChance(pre, rand);
        }
    }
}