using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Terraria.Localization;

namespace OvermorrowMod.Core.Items
{
    /// <summary>
    /// Handles parsing of tooltip text and keywords
    /// </summary>
    public static class TooltipParser
    {
        /// <summary>
        /// Extracts keywords from tooltip text
        /// </summary>
        public static string[] GetKeywords(string text)
        {
            var matches = Regex.Matches(text, @"\{Keyword:(.+?)\}");
            return string.Join(";", matches.Cast<Match>().Select(m => m.Groups[1].Value)).Split(';');
        }

        /// <summary>
        /// Gets keyword description from XML
        /// </summary>
        public static string GetKeyword(string id)
        {
            var text = Language.GetTextValue(LocalizationPath.Keywords + id + ".Description");
            return text;
        }

        /// <summary>
        /// Searches for buffs or debuffs in text
        /// </summary>
        public static string[] GetBuff(string text, bool isBuff = true)
        {
            string pattern = isBuff ? @"\<Buff:(.+?)\>" : @"\<Debuff:(.+?)\>";
            var matches = Regex.Matches(text, pattern);
            return string.Join(";", matches.Cast<Match>().Select(m => m.Groups[1].Value)).Split(';');
        }
    }
}