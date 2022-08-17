using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
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
    public abstract class ReforgeStone : ModItem
    {
        public int DPSFloor = 0;
        public int DPSCeiling = 50;
        public override bool CanRightClick() => true;
        public override bool CanStack(Item item2) => false;
        public override bool CanUseItem(Player player) => false;
        public override bool ConsumeItem(Player player) => false;
        public virtual string WorldTexture => AssetDirectory.Textures + "Items/Reforge_Dropped_1";

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
        }

        public static int[] meleePrefixes = new int[] {
            PrefixID.Savage, PrefixID.Legendary, PrefixID.Murderous, PrefixID.Deadly,
            PrefixID.Godly, PrefixID.Demonic, PrefixID.Superior, PrefixID.Unpleasant };

        public static int[] knockbackPrefixesMelee = new int[] {
            PrefixID.Savage, PrefixID.Legendary, PrefixID.Godly, PrefixID.Superior, PrefixID.Unpleasant };

        public static int[] critPrefixesMelee = new int[] {
            PrefixID.Legendary, PrefixID.Murderous, PrefixID.Godly, PrefixID.Demonic, PrefixID.Superior };

        public static int[] rangedPrefixes = new int[]
        {
            PrefixID.Intimidating, PrefixID.Rapid, PrefixID.Hasty, PrefixID.Deadly2, PrefixID.Staunch, PrefixID.Unreal
        };

        public static int[] magicPrefixes = new int[]
        {
            PrefixID.Mystic, PrefixID.Masterful, PrefixID.Celestial, PrefixID.Mythical
        };

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int lines = 0; lines < tooltips.Count; lines++)
            {
                if (tooltips[lines].Name == "Damage") tooltips.RemoveAt(lines);
                if (tooltips[lines].Name == "UseMana") tooltips.RemoveAt(lines);
                if (tooltips[lines].Name == "CritChance") tooltips.RemoveAt(lines);
                if (tooltips[lines].Name == "Speed") tooltips.RemoveAt(lines);
                if (tooltips[lines].Name == "Knockback") tooltips.RemoveAt(lines);
            }

            base.ModifyTooltips(tooltips);
        }

        public override bool? PrefixChance(int pre, UnifiedRandom rand)
        {
            if (pre == -3) return false;

            return base.PrefixChance(pre, rand);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>(WorldTexture).Value;
            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, null, lightColor, rotation, texture.Size() / 2f, scale, SpriteEffects.None, 1);

            return false;
        }

        // Bro I don't even know why I need to do this, Prefix(-1) just doesn't wanna work half the time
        public virtual int GetRandomPrefix()
        {
            return meleePrefixes[Main.rand.Next(0, meleePrefixes.Length)];
        }

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            return GetRandomPrefix();
        }
    }

    public class MeleeReforge : ReforgeStone
    {
        public override string Texture => AssetDirectory.Textures + "Items/Reforge_Melee_1";
        public override int GetRandomPrefix() => meleePrefixes[Main.rand.Next(0, meleePrefixes.Length)];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Melee Reforge Stone");
            Tooltip.SetDefault("Used to apply modifiers to Melee weapons");
            ItemID.Sets.CanGetPrefixes[Type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 10f;
        }
    }

    public class RangedReforge : ReforgeStone
    {
        public override string Texture => AssetDirectory.Textures + "Items/Reforge_Ranged_1";
        public override int GetRandomPrefix() => rangedPrefixes[Main.rand.Next(0, rangedPrefixes.Length)];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ranged Reforge Stone");
            Tooltip.SetDefault("Used to apply modifiers to Ranged weapons");
            ItemID.Sets.CanGetPrefixes[Type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DamageType = DamageClass.Ranged;
        }
    }

    public class MagicReforge : ReforgeStone
    {
        public override string Texture => AssetDirectory.Textures + "Items/Reforge_Magic_1";
        public override int GetRandomPrefix() => magicPrefixes[Main.rand.Next(0, magicPrefixes.Length)];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magic Reforge Stone");
            Tooltip.SetDefault("Used to apply modifiers to Ranged weapons");
            ItemID.Sets.CanGetPrefixes[Type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
        }
    }
}