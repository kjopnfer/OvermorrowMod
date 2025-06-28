using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Tooltips;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace OvermorrowMod.Core.Globals
{
    public class ItemTooltips : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public List<TooltipEntity> TooltipEntities = new List<TooltipEntity>();
        private readonly HashSet<string> KeyWords = new HashSet<string>();

        public override void SetDefaults(Item item)
        {
            // Initialize set bonuses for vanilla items, these items aren't able to have the interfaces attached
            InitializeSetBonuses(item);
            InitializeVanillaItems(item);

            // Initialize from interface
            if (item.ModItem is ITooltipEntities tooltipProvider)
            {
                TooltipEntities = tooltipProvider.TooltipObjects();
                tooltipProvider.UpdateTooltips(TooltipEntities);
            }

            base.SetDefaults(item);
        }

        public override bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
        {
            if (!TooltipSystem.IsTooltipKeyPressed)
                return base.PreDrawTooltip(item, lines, ref x, ref y);

            var orderedTooltips = TooltipEntities.OrderBy(obj => obj.Priority).ToList();
            string widest = lines.OrderBy(n => ChatManager.GetStringSize(FontAssets.MouseText.Value, n.Text, Vector2.One).X).Last().Text;

            // Use TooltipRenderer for all drawing logic
            TooltipRenderer.DrawTooltipEntities(Main.spriteBatch, orderedTooltips, widest, x, y);
            TooltipRenderer.DrawKeywordTooltips(Main.spriteBatch, KeyWords, widest, x, y, orderedTooltips.Count);

            return base.PreDrawTooltip(item, lines, ref x, ref y);
        }

        /// <summary>
        /// Determines whether or not the item is equipped in vanity based on if the 'Social' tooltip is displayed
        /// </summary>
        private bool CheckInVanity(List<TooltipLine> tooltips)
        {
            for (int lines = 0; lines < tooltips.Count; lines++)
            {
                if (tooltips[lines].Name == "Social") return true;
            }

            return false;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (CheckInVanity(tooltips)) return;
            var name = item.Name.Replace(" ", "");
            if (item.ModItem != null)
            {
                name = item.ModItem.Name;
            }

            // Add weapon type
            AddWeaponTypeTooltip(item, tooltips);

            // Add tooltips from interface
            var interfaceTooltips = TooltipSystem.GetTooltipLines(item);
            foreach (var tooltipText in interfaceTooltips)
            {
                //tooltips.Insert(3, new TooltipLine(Mod, "InterfaceTooltip", tooltipText));
                tooltips.Insert(3, new TooltipLine(Mod, "InterfaceTooltip", "BRUHHHHHHHHHH"));
                //tooltips.Add(new TooltipLine(Mod, "InterfaceTooltip", tooltipText));
            }

            // Hijack and remove the tooltips from vanilla
            List<TooltipLine> hijackedTooltips = new List<TooltipLine>();

            foreach (var line in tooltips.ToList())
            {
                if (line.Name.Contains("Tooltip"))
                {
                    //Main.NewText($"Removed: {line.Name}");
                    hijackedTooltips.Add(new TooltipLine(Mod, line.Name, line.Text));
                    tooltips.Remove(line);
                }
            }

            // Re-add modified tooltips using a for-loop
            for (int i = 0; i < hijackedTooltips.Count; i++)
            {
                TooltipLine tooltipLine = hijackedTooltips[i];
                tooltips.Add(new TooltipLine(Mod, $"Tooltip{i}", tooltipLine.Text));
            }

            // Draw flavor text
            var flavorKey = LocalizationPath.Items + name + ".Flavor";
            //Main.NewText(Language.GetTextValue(flavorKey));

            if (Language.Exists(flavorKey))
            {
                var flavor = Language.GetTextValue(flavorKey);
                var flavorTextLines = flavor.Split("\n");
                foreach (var flavorTextLine in flavorTextLines)
                {
                    tooltips.Add(new TooltipLine(Mod, "Flavor", $"[c/C19A6B:{flavorTextLine}]"));
                }
            }

            // Add shift prompt
            if (TooltipEntities.Count > 0 || KeyWords.Count > 0)
            {
                if (!TooltipSystem.IsTooltipKeyPressed)
                    tooltips.Add(new TooltipLine(Mod, "TooltipKey", "<Key:Hold SHIFT for more info>"));
            }

            // Process keywords and parse text
            ProcessTooltipKeywords(tooltips);
            ParseTooltipText(tooltips);

            base.ModifyTooltips(item, tooltips);
        }

        private static bool IsWoodArmor(int itemType) =>
            itemType == ItemID.WoodHelmet || itemType == ItemID.WoodBreastplate || itemType == ItemID.WoodGreaves;

        private static bool IsCowboyArmor(int itemType) =>
            itemType == ItemID.CowboyHat || itemType == ItemID.CowboyJacket || itemType == ItemID.CowboyPants;

        private void AddWeaponTypeTooltip(Item item, List<TooltipLine> tooltips)
        {
            WeaponType weaponType = item.GetWeaponType();
            if (weaponType == WeaponType.None)
                return;

            // Find where to insert the weapon type (after item name)
            int insertIndex = tooltips.FindIndex(tip => tip.Name.StartsWith("ItemName"));
            if (insertIndex == -1)
                insertIndex = 0;
            else
                insertIndex++;

            string typeText = FormatWeaponType(weaponType);
            tooltips.Insert(insertIndex, new TooltipLine(Mod, "WeaponType", $"[c/FAD5A5:{typeText}]"));
        }

        private string FormatWeaponType(WeaponType weaponType)
        {
            // TODO: Get localized text for these
            List<string> types = new List<string>();

            // Check each weapon type flag and add to the list
            if (weaponType.HasFlag(WeaponType.Sword))
                types.Add("Sword");
            if (weaponType.HasFlag(WeaponType.Broadsword))
                types.Add("Broadsword");
            if (weaponType.HasFlag(WeaponType.Rapier))
                types.Add("Rapier");
            if (weaponType.HasFlag(WeaponType.Greatsword))
                types.Add("Greatsword");
            if (weaponType.HasFlag(WeaponType.Shortsword))
                types.Add("Shortsword");
            if (weaponType.HasFlag(WeaponType.Dagger))
                types.Add("Dagger");
            if (weaponType.HasFlag(WeaponType.Bow))
                types.Add("Bow");
            if (weaponType.HasFlag(WeaponType.Staff))
                types.Add("Staff");
            if (weaponType.HasFlag(WeaponType.Whip))
                types.Add("Whip");

            // Gun types
            if (weaponType.HasFlag(WeaponType.Revolver))
                types.Add("Revolver");
            if (weaponType.HasFlag(WeaponType.Handgun))
                types.Add("Handgun");
            if (weaponType.HasFlag(WeaponType.Shotgun))
                types.Add("Shotgun");
            if (weaponType.HasFlag(WeaponType.Musket))
                types.Add("Musket");
            if (weaponType.HasFlag(WeaponType.Rifle))
                types.Add("Rifle");
            if (weaponType.HasFlag(WeaponType.SubMachineGun))
                types.Add("SMG");
            if (weaponType.HasFlag(WeaponType.MachineGun))
                types.Add("Machine Gun");
            if (weaponType.HasFlag(WeaponType.Launcher))
                types.Add("Launcher");
            if (weaponType.HasFlag(WeaponType.Sniper))
                types.Add("Sniper");

            // Combine weapon types
            string result = string.Join(" & ", types);

            return result;
        }

        private void InitializeSetBonuses(Item item)
        {
            // Wood armor set
            if (IsWoodArmor(item.type))
            {
                TooltipEntities.Add(TooltipEntity.CreateSetBonusTooltip(
                    "Wooden Warrior",
                    "Wood Armor",
                    [" + Increased defense by [c/58D68D:1]", " + Increased damage by [c/58D68D:5]", " + [c/58D68D:5%] chance to instantly kill all enemies"],
                    new List<int> { ItemID.WoodHelmet, ItemID.WoodBreastplate, ItemID.WoodGreaves }));
            }

            // Cowboy armor set
            if (IsCowboyArmor(item.type))
            {
                TooltipEntities.Add(TooltipEntity.CreateSetBonusTooltip(
                    "Wild West Deadeye",
                    "Cowboy Armor",
                    [" + Critical hits with [c/FAD5A5:Revolvers] rebound to the nearest enemy"],
                    new List<int> { ItemID.CowboyHat, ItemID.CowboyJacket, ItemID.CowboyPants }));
            }
        }

        private void InitializeVanillaItems(Item item)
        {
            if (item.type == ItemID.PhoenixBlaster)
            {
                TooltipEntities.Add(new BuffTooltip(ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "Default").Value,
                    "Phoenix Mark",
                    [" + Increases all incoming damage by [c/58D68D:15%]"],
                    6,
                    BuffTooltipType.Debuff));
                TooltipEntities.Add(new BuffTooltip(TextureAssets.Buff[BuffID.OnFire].Value,
                    "On Fire!",
                    [" - Prevents health regeneration", " - Loses [c/ff5555:4] health per second"],
                    4,
                    BuffTooltipType.Debuff));
            }
        }

        private void ProcessTooltipKeywords(List<TooltipLine> tooltips)
        {
            foreach (var line in tooltips)
            {
                var keywords = TooltipParser.GetKeywords(line.Text);
                foreach (var keyword in keywords.Where(k => !string.IsNullOrEmpty(k)))
                {
                    KeyWords.Add(keyword);
                }
            }
        }

        private void ParseTooltipText(List<TooltipLine> tooltips)
        {
            foreach (var tooltip in tooltips)
            {
                string newText = tooltip.Text;
                newText = TooltipRenderer.ParseTooltipText(newText);
                newText = TooltipRenderer.ParseTooltipObjects(newText);
                tooltip.Text = newText;
            }
        }
    }
}