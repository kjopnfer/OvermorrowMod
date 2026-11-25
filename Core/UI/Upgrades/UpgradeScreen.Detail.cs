using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Items.Archives;
using OvermorrowMod.Core.MetaUpgrades;
using ReLogic.Graphics;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Ledger = OvermorrowMod.Core.UI.DeathScreen.DeathScreen;

namespace OvermorrowMod.Core.UI.Upgrades
{
    public partial class UpgradeScreen
    {
        private const int DetailPadding = 22;
        private const int SocketSize = 54;
        private const int RankRowHeight = 26;
        private const int ButtonHeight = 44;

        private static readonly Color BadColor = new(255, 107, 107);
        private static readonly Regex NumberPattern = new(@"^(\D*)(\d+)(.*)$");

        private void DrawDetail(SpriteBatch spriteBatch, Rectangle area)
        {
            float alpha = Appear(30, 30);
            area.Y += (int)((1f - alpha) * 6);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, area, Ledger.SlotColor * alpha);
            Ledger.DrawBorder(spriteBatch, area, Ledger.DarkSilverColor * alpha);

            int x = area.X + DetailPadding;
            int y = area.Y + 20;
            int innerWidth = area.Width - DetailPadding * 2;

            Rectangle iconBox = new(x, y, 76, 76);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, iconBox, Color.Black * 0.5f * alpha);
            Ledger.DrawBorder(spriteBatch, iconBox, Ledger.DarkSilverColor * alpha);
            DrawIconFit(spriteBatch, selected.IconItem, iconBox.Center.ToVector2(), 56, Color.White * alpha);

            DynamicSpriteFont titleFont = FontAssets.DeathText.Value;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, titleFont, selected.DisplayName, new Vector2(iconBox.Right + 16, y), Color.White * alpha, 0f, Vector2.Zero, new Vector2(0.6f));
            string description = FontAssets.MouseText.Value.CreateWrappedText(selected.Description, (innerWidth - 92) / 0.95f);
            Ledger.DrawText(spriteBatch, description, new Vector2(iconBox.Right + 16, y + 40), Ledger.LightSilverColor * alpha, 0.95f);

            y = iconBox.Bottom + 24;
            DrawSockets(spriteBatch, x, y, innerWidth, alpha);

            y += SocketSize + 30;
            DrawValues(spriteBatch, x, y, innerWidth, alpha);

            y += 58;
            DrawAction(spriteBatch, x, y, innerWidth, alpha);

            y += ButtonHeight + 22;
            DrawRanks(spriteBatch, x, y, innerWidth, alpha);
        }

        private void DrawSockets(SpriteBatch spriteBatch, int x, int y, int width, float alpha)
        {
            int count = selected.MaxRank;
            int rank = ShownRank;
            int wire = count > 1 ? Math.Min(40, (width - count * SocketSize) / (count - 1)) : 0;
            bool affordable = Upgrades.CanAfford(selected);
            Texture2D glow = ModContent.Request<Texture2D>(AssetDirectory.Textures + "spotlight").Value;
            float pulse = 0.5f + 0.5f * MathF.Sin(openTicks / 96f * MathHelper.TwoPi);

            for (int i = 0; i < count; i++)
            {
                MonkeyStoneTier tier = selected.Costs[i][0].Tier;
                Color color = TierColor(tier);
                bool filled = i < rank;
                bool next = i == rank && pendingSocket < 0;

                float landScale = i == rank - 1 && landTicks > 0 ? 1f + 0.25f * EasingUtils.EaseInCubic(landTicks / 30f) : 1f;
                int size = (int)(SocketSize * landScale);
                int left = x + i * (SocketSize + wire);
                Rectangle socket = new(left + SocketSize / 2 - size / 2, y + SocketSize / 2 - size / 2, size, size);
                sockets[i] = new Rectangle(left, y, SocketSize, SocketSize);

                if (i > 0)
                {
                    Rectangle line = new(left - wire, y + SocketSize / 2 - 2, wire, 4);
                    if (filled)
                    {
                        Color from = TierColor(selected.Costs[i - 1][0].Tier);
                        spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(line.X, line.Y, line.Width / 2, line.Height), from * alpha);
                        spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(line.X + line.Width / 2, line.Y, line.Width - line.Width / 2, line.Height), color * alpha);
                    }
                    else
                    {
                        spriteBatch.Draw(TextureAssets.MagicPixel.Value, line, Color.White * 0.08f * alpha);
                    }
                }

                if (filled)
                    spriteBatch.Draw(glow, socket.Center.ToVector2(), null, color with { A = 0 } * 0.45f * alpha, 0f, glow.Size() / 2f, size * 1.7f / glow.Width, SpriteEffects.None, 0f);
                else if (next && affordable)
                    spriteBatch.Draw(glow, socket.Center.ToVector2(), null, color with { A = 0 } * 0.35f * pulse * alpha, 0f, glow.Size() / 2f, size * 1.6f / glow.Width, SpriteEffects.None, 0f);

                spriteBatch.Draw(TextureAssets.MagicPixel.Value, socket, Color.Black * 0.7f * alpha);
                spriteBatch.Draw(glow, socket.Center.ToVector2() - new Vector2(0, 3), null, Color.White with { A = 0 } * 0.06f * alpha, 0f, glow.Size() / 2f, size * 0.9f / glow.Width, SpriteEffects.None, 0f);

                Color frame = filled ? color : next ? (affordable ? color * 0.7f : BadColor * 0.4f) : Color.White * 0.1f;
                DrawFrame(spriteBatch, socket, frame * alpha, 2);

                Color stoneColor = filled ? Color.White : next ? Color.Black * 0.9f : Color.Black * 0.7f;
                DrawStone(spriteBatch, tier, socket.Center.ToVector2(), 36 * landScale, stoneColor * alpha);

                string label = (i + 1).ToString();
                Ledger.DrawCentered(spriteBatch, label, new Vector2(left + SocketSize / 2f, y + SocketSize + 12), Ledger.DarkSilverColor * alpha, 0.7f);
            }
        }

        private void DrawValues(SpriteBatch spriteBatch, int x, int y, int width, float alpha)
        {
            int rank = ShownRank;
            string now = selected.ValueText(rank);
            if (countTicks < CountDuration && pendingSocket < 0) now = CountingText(now);
            bool hasNext = rank < selected.MaxRank;
            string next = hasNext ? selected.ValueText(rank + 1) : "";

            const float chevronGap = 50f;
            float textWidth = Ledger.TextWidth(now, 1f) + Ledger.TextWidth(next, 1f);
            float scale = Math.Min(1.3f, (width - (hasNext ? chevronGap : 0f)) / textWidth);

            Ledger.DrawText(spriteBatch, Text("Now").ToUpperInvariant(), new Vector2(x, y), Ledger.DarkSilverColor * alpha, 0.75f);
            Color nowColor = Color.Lerp(Color.White, Ledger.GainColor, landTicks / 30f);
            Ledger.DrawText(spriteBatch, now, new Vector2(x, y + 18), nowColor * alpha, scale);

            if (!hasNext) return;

            float chevronX = x + Ledger.TextWidth(now, scale) + 18;
            Ledger.DrawText(spriteBatch, ">", new Vector2(chevronX, y + 18), Ledger.DarkSilverColor * alpha, scale);

            float nextX = x + Ledger.TextWidth(now, scale) + chevronGap;
            Ledger.DrawText(spriteBatch, Text("Next").ToUpperInvariant(), new Vector2(nextX, y), Ledger.DarkSilverColor * alpha, 0.75f);
            Ledger.DrawText(spriteBatch, next, new Vector2(nextX, y + 18), Ledger.GainColor * alpha, scale);
        }

        private string CountingText(string target)
        {
            Match from = NumberPattern.Match(countFrom ?? "");
            Match to = NumberPattern.Match(target);
            if (!from.Success || !to.Success || from.Groups[1].Value != to.Groups[1].Value || from.Groups[3].Value != to.Groups[3].Value) return target;

            float t = EasingUtils.EaseOutQuart(countTicks / (float)CountDuration);
            int value = (int)MathF.Round(MathHelper.Lerp(int.Parse(from.Groups[2].Value), int.Parse(to.Groups[2].Value), t));
            return to.Groups[1].Value + value + to.Groups[3].Value;
        }

        private void DrawAction(SpriteBatch spriteBatch, int x, int y, int width, float alpha)
        {
            if (Upgrades.IsMaxed(selected) && pendingSocket < 0)
            {
                Ledger.DrawTextCentered(spriteBatch, Text("Max"), x, y + ButtonHeight / 2f, Ledger.GoldColor * alpha, 1.3f);
                return;
            }

            StoneCost[] cost = selected.Costs[ShownRank];
            bool affordable = Upgrades.CanAfford(selected) && !Busy;
            string label = ShownRank == 0 ? Text("Unlock") : Text("Upgrade");
            float costWidth = cost.Sum(stone => 26 + Ledger.TextWidth(stone.Amount.ToString(), 1f) + 10);
            int buttonWidth = (int)(Ledger.TextWidth(label, 1.1f) + costWidth + 52);

            int shake = shakeTicks > 0 ? (int)(MathF.Sin(shakeTicks * 1.4f) * 4f) : 0;
            Rectangle button = new(x + shake, y, buttonWidth, ButtonHeight);
            bool hovered = button.Contains(Main.MouseScreen.ToPoint());

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, button, Ledger.SlotColor * alpha);
            Ledger.DrawBorder(spriteBatch, button, (hovered && affordable ? Ledger.LightSilverColor : Ledger.DarkSilverColor) * alpha);

            Color textColor = !affordable ? Ledger.DarkSilverColor : hovered ? Color.White : Ledger.GoldColor;
            Ledger.DrawTextCentered(spriteBatch, label, button.X + 22, button.Center.Y, textColor * alpha, 1.1f);
            DrawCost(spriteBatch, cost, button.X + 22 + Ledger.TextWidth(label, 1.1f) + 14, button.Center.Y, Busy ? Ledger.DarkSilverColor : Color.White, alpha);

            if (!Upgrades.CanAfford(selected) && !Busy)
                Ledger.DrawTextCentered(spriteBatch, Text("NotEnough"), button.Right + 14, button.Center.Y, BadColor * alpha, 0.9f);

            if (hovered && !Busy && Main.mouseLeft && Main.mouseLeftRelease)
            {
                Main.mouseLeftRelease = false;
                Buy();
            }
        }

        private void DrawRanks(SpriteBatch spriteBatch, int x, int y, int width, float alpha)
        {
            int rank = ShownRank;
            for (int i = 0; i < selected.MaxRank; i++)
            {
                int rowY = y + i * RankRowHeight;
                bool owned = i < rank;
                Color color = owned ? Ledger.GainColor : i == rank ? Color.White : Ledger.DarkSilverColor;

                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(x, rowY, width, 1), Ledger.DarkSilverColor * 0.25f * alpha);
                float centerY = rowY + RankRowHeight / 2f + 1;
                Ledger.DrawTextCentered(spriteBatch, Text("Rank") + " " + (i + 1), x + 8, centerY, color * alpha, 0.85f);
                Ledger.DrawTextCentered(spriteBatch, selected.ValueText(i + 1), x + 82, centerY, color * alpha, 0.85f);

                if (owned)
                {
                    string unlocked = Text("Unlocked");
                    Ledger.DrawTextCentered(spriteBatch, unlocked, x + width - 8 - Ledger.TextWidth(unlocked, 0.85f), centerY, color * alpha, 0.85f);
                }
                else
                {
                    StoneCost[] cost = selected.Costs[i];
                    float costWidth = cost.Sum(stone => 20 + Ledger.TextWidth(stone.Amount.ToString(), 0.85f) + 8);
                    DrawCost(spriteBatch, cost, x + width - 8 - costWidth, centerY, color, alpha, 16, 0.85f);
                }
            }
        }

        private void DrawCost(SpriteBatch spriteBatch, StoneCost[] cost, float x, float centerY, Color color, float alpha, float iconSize = 20, float scale = 1f)
        {
            foreach (StoneCost stone in cost)
            {
                DrawStone(spriteBatch, stone.Tier, new Vector2(x + iconSize / 2f, centerY), iconSize, Color.White * alpha);
                x += iconSize + 4;
                string amount = stone.Amount.ToString();
                Ledger.DrawTextCentered(spriteBatch, amount, x, centerY, color * alpha, scale);
                x += Ledger.TextWidth(amount, scale) + 8;
            }
        }
    }
}
