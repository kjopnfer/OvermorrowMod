using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core.MetaUpgrades;
using OvermorrowMod.Core.UI.RunStats;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Ledger = OvermorrowMod.Core.UI.DeathScreen.DeathScreen;

namespace OvermorrowMod.Core.UI.Upgrades
{
    public partial class UpgradeScreen
    {
        private const int HeadingHeight = 24;
        private const int EntryHeight = 40;
        private const int GroupGap = 6;

        private static readonly UpgradeGroup[] Groups = Enum.GetValues<UpgradeGroup>();

        private void DrawList(SpriteBatch spriteBatch, Rectangle area)
        {
            Point mouse = Main.MouseScreen.ToPoint();
            int y = area.Y;
            int index = 0;
            foreach (UpgradeGroup group in Groups)
            {
                float headingAlpha = Appear(12 + index * 3, 18);
                Ledger.DrawText(spriteBatch, Text("Groups." + group).ToUpperInvariant(), new Vector2(area.X, y + 2), Ledger.GoldColor * headingAlpha, 0.85f);
                y += HeadingHeight;

                foreach (MetaUpgrade upgrade in MetaUpgrade.All.Where(upgrade => upgrade.Group == group))
                {
                    float alpha = Appear(12 + index * 3, 18);
                    Rectangle entry = new(area.X, y + (int)((1f - alpha) * 6), area.Width, EntryHeight);
                    DrawEntry(spriteBatch, upgrade, entry, alpha, entry.Contains(mouse));
                    y += EntryHeight + EntryGap;
                    index++;
                }

                y += GroupGap;
            }
        }

        private void DrawEntry(SpriteBatch spriteBatch, MetaUpgrade upgrade, Rectangle entry, float alpha, bool hovered)
        {
            bool isSelected = upgrade == selected;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, entry, (isSelected ? Ledger.GoldColor * 0.1f : Ledger.SlotColor) * alpha);
            Color border = isSelected ? Ledger.GoldColor : hovered ? Ledger.SilverColor : Ledger.DarkSilverColor;
            Ledger.DrawBorder(spriteBatch, entry, border * alpha);

            RunStatsPanel.DrawItemIcon(spriteBatch, upgrade.IconItem, new Vector2(entry.X + 23, entry.Center.Y), 26, Color.White * alpha);

            Color nameColor = isSelected ? Color.White : Ledger.LightSilverColor;
            Ledger.DrawText(spriteBatch, upgrade.DisplayName, new Vector2(entry.X + 46, entry.Y + 3), nameColor * alpha, 0.85f);
            if (Upgrades.IsMaxed(upgrade))
                Ledger.DrawText(spriteBatch, Text("Max"), new Vector2(entry.X + 52 + Ledger.TextWidth(upgrade.DisplayName, 0.85f), entry.Y + 5), Ledger.GoldColor * alpha, 0.7f);

            int rank = upgrade == selected ? ShownRank : Upgrades.Rank(upgrade);
            for (int i = 0; i < upgrade.MaxRank; i++)
                DrawPip(spriteBatch, new Rectangle(entry.X + 46 + i * 13, entry.Y + 26, 9, 9), TierColor(upgrade.Costs[i][0].Tier), i < rank, alpha);

            if (hovered && !isSelected && !Busy && Main.mouseLeft && Main.mouseLeftRelease)
            {
                Main.mouseLeftRelease = false;
                selected = upgrade;
                countTicks = CountDuration;
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }

        private static void DrawPip(SpriteBatch spriteBatch, Rectangle pip, Color color, bool filled, float alpha)
        {
            if (filled)
            {
                Texture2D glow = ModContent.Request<Texture2D>(AssetDirectory.Textures + "spotlight").Value;
                spriteBatch.Draw(glow, pip.Center.ToVector2(), null, color with { A = 0 } * 0.5f * alpha, 0f, glow.Size() / 2f, pip.Width * 2.2f / glow.Width, SpriteEffects.None, 0f);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, pip, color * alpha);
                return;
            }

            DrawFrame(spriteBatch, pip, color * 0.55f * alpha, 2);
        }
    }
}
