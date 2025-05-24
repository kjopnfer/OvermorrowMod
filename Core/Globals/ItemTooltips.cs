using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Tooltips;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
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
            // Initialize set bonuses
            InitializeSetBonuses(item);

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

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            // Add weapon type
            int index = tooltips.FindIndex(tip => tip.Name.StartsWith("ItemName"));
            string type = item.GetWeaponType();
            if (type != "None")
                tooltips.Insert(index + 1, new TooltipLine(Mod, "ItemType", $"[c/FAD5A5:{type} Type]"));

            // Add tooltips from interface
            var interfaceTooltips = TooltipSystem.GetTooltipLines(item);
            foreach (var tooltipText in interfaceTooltips)
            {
                tooltips.Insert(3, new TooltipLine(Mod, "InterfaceTooltip", tooltipText));
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